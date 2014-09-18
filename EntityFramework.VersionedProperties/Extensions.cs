using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFramework.Extensions;
using EntityFrameworkTriggers;
using System.Reflection;

namespace EntityFramework.VersionedProperties {
	public static class Extensions {
		private static T ThrowIfNull<T>(this T @object) where T : class {
			if (@object == null)
				throw new ArgumentNullException();
			return @object;
		}

		internal struct VersionInfo {
			public PropertyInfo VersionDbSetPropertyInfo;
			public Type VersionType;
			public Type VersionValueType;
		}
		private static readonly Dictionary<Type, VersionInfo[]> versionDbSetPropertyInfoCache = new Dictionary<Type, VersionInfo[]>();
		private static VersionInfo[] GetVersionDbSetPropertyInfos(DbContext dbContext) {
			var dbContextType = dbContext.GetType();
			VersionInfo[] versionDbSetPropertyInfos;
			if (!versionDbSetPropertyInfoCache.TryGetValue(dbContextType, out versionDbSetPropertyInfos)) {
				var versionDbSetPropertyInfoQuery = from propertyInfo in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				                                    where typeof (DbSet<>) == (propertyInfo.PropertyType.IsGenericType ? propertyInfo.PropertyType.GetGenericTypeDefinition() : null)
				                                    where typeof (IVersion).IsAssignableFrom(propertyInfo.PropertyType.GenericTypeArguments.Single())
				                                    select new VersionInfo {
					                                                           VersionDbSetPropertyInfo = propertyInfo,
					                                                           VersionType = propertyInfo.PropertyType.GenericTypeArguments.Single(),
					                                                           VersionValueType = propertyInfo.PropertyType.GenericTypeArguments.Single().BaseType.GenericTypeArguments.Single()
				                                                           };
				versionDbSetPropertyInfos = versionDbSetPropertyInfoQuery.ToArray();
				versionDbSetPropertyInfoCache.Add(dbContextType, versionDbSetPropertyInfos);
			}
			return versionDbSetPropertyInfos;
		}
		private static readonly Dictionary<IVersionedProperties, PropertyInfo[]> versionedPropertiesPropertyInfoCache = new Dictionary<IVersionedProperties, PropertyInfo[]>();
		private static PropertyInfo[] GetVersionedPropertiesPropertyInfos(IVersionedProperties versionedProperties) {
			PropertyInfo[] versionPropertyInfos;
			if (!versionedPropertiesPropertyInfoCache.TryGetValue(versionedProperties, out versionPropertyInfos)) {
				versionPropertyInfos = versionedProperties.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(IVersioned).IsAssignableFrom(x.PropertyType)).ToArray();
				versionedPropertiesPropertyInfoCache.Add(versionedProperties, versionPropertyInfos);
			}
			return versionPropertyInfos;
		}
		internal struct Mapping {
			public PropertyInfo VersionedProperty { get; set; }
			public VersionInfo VersionInfo { get; set; }
		}
		private static readonly Dictionary<Tuple<IVersionedProperties, DbContext>, Mapping[]> mappingsCache = new Dictionary<Tuple<IVersionedProperties, DbContext>, Mapping[]>();
		private static Mapping[] GetMappings(IVersionedProperties versionedProperties, DbContext dbContext) {
			var pair = new Tuple<IVersionedProperties, DbContext>(versionedProperties, dbContext);
			Mapping[] mappings;
			if (!mappingsCache.TryGetValue(pair, out mappings)) {
				var mappingsQuery = from versionedPropertyPropertyInfo in GetVersionedPropertiesPropertyInfos(versionedProperties)
									from versionDbSetPropertyInfo in GetVersionDbSetPropertyInfos(dbContext)
									where versionDbSetPropertyInfo.VersionType == versionedPropertyPropertyInfo.PropertyType.BaseType.GenericTypeArguments[1]
									select new Mapping {
										VersionedProperty = versionedPropertyPropertyInfo,
										VersionInfo = versionDbSetPropertyInfo
									};
				mappings = mappingsQuery.ToArray();
				mappingsCache.Add(pair, mappings.ToArray());
			}
			return mappings;
		}

		/// <summary>
		/// Initialize all versioned properties in this object. This should be the called before doing anything to the versioned properties (at the beginning of the constructor, for example)
		/// </summary>
		/// <typeparam name="TVersionedProperties"></typeparam>
		/// <param name="versionedProperties"></param>
		/// <example>
		/// <code>
		///		class Person : IVersionedProperties&lt;Person&gt; {
		///			public String Name { get; set; }
		///			public VersionedString NickName { get; set; }
		///			public Person(String name, String nickName) {
		///				this.InitializeVersionedProperties();
		///				Name = name;
		///				NickName.Value = nickName;
		///			}
		///		}
		/// </code>
		/// </example>
		public static void InitializeVersionedProperties<TVersionedProperties>(this IVersionedProperties<TVersionedProperties> versionedProperties)
			where TVersionedProperties : class, IVersionedProperties<TVersionedProperties>, ITriggerable<TVersionedProperties>, new() {
			var versionedPropertyInfos = GetVersionedPropertiesPropertyInfos(versionedProperties);
			foreach (var propertyInfo in versionedPropertyInfos) {
				var versioned = Activator.CreateInstance(propertyInfo.PropertyType);
				propertyInfo.SetValue(versionedProperties, versioned);
			}
			var versionedPropertiesInstance = (TVersionedProperties) versionedProperties;
			var triggers = versionedPropertiesInstance.Triggers();
			triggers.Inserting += OnInsertingOrUpdating;
			triggers.Updating += OnInsertingOrUpdating;
			triggers.Deleted += OnDeleted;
		}

		private static readonly MethodInfo addAndClearVersionsMethodInfo = typeof(Extensions).GetMethod("AddAndClearVersions", BindingFlags.Static | BindingFlags.NonPublic).ThrowIfNull();
		private static void AddAndClearVersions<TValue, TVersion>(DbSet<TVersion> dbSet, VersionedBase<TValue, TVersion> versioned)
			where TVersion : VersionBase<TValue>, new() {
			dbSet.AddRange(versioned.Versions);
			versioned.Versions.Clear();
		}

		private static void OnInsertingOrUpdating<TVersionedProperties>(Triggers<TVersionedProperties>.Entry e)
			where TVersionedProperties : class, IVersionedProperties<TVersionedProperties>, ITriggerable<TVersionedProperties>, new() {
			foreach (var mapping in GetMappings(e.Entity, e.Context)) {
				var addAndClearVersionsMethodInfoGeneric = addAndClearVersionsMethodInfo.MakeGenericMethod(mapping.VersionInfo.VersionValueType, mapping.VersionInfo.VersionType);
				var versionedProperty = mapping.VersionedProperty.GetValue(e.Entity);
				var dbSet = mapping.VersionInfo.VersionDbSetPropertyInfo.GetValue(e.Context);
				addAndClearVersionsMethodInfoGeneric.Invoke(null, new[] { dbSet, versionedProperty });
			}
		}

		private static readonly MethodInfo deleteAndClearVersionsMethodInfo = typeof(Extensions).GetMethod("DeleteAndClearVersions", BindingFlags.Static | BindingFlags.NonPublic).ThrowIfNull();
		private static void DeleteAndClearVersions<TValue, TVersion>(DbSet<TVersion> dbSet, VersionedBase<TValue, TVersion> versioned)
			where TVersion : VersionBase<TValue>, new() {
			dbSet.Where(x => x.VersionedId == versioned.Id).Delete();
			versioned.Versions.Clear();
		}

		private static void OnDeleted<TVersionedProperties>(Triggers<TVersionedProperties>.Entry e)
			where TVersionedProperties : class, IVersionedProperties<TVersionedProperties>, ITriggerable<TVersionedProperties>, new() {
			foreach (var mapping in GetMappings(e.Entity, e.Context)) {
				var deleteAndClearVersionsMethodInfoGeneric = deleteAndClearVersionsMethodInfo.MakeGenericMethod(mapping.VersionInfo.VersionValueType, mapping.VersionInfo.VersionType);
				var versionedProperty = mapping.VersionedProperty.GetValue(e.Entity);
				var dbSet = mapping.VersionInfo.VersionDbSetPropertyInfo.GetValue(e.Context);
				deleteAndClearVersionsMethodInfoGeneric.Invoke(null, new[] { dbSet, versionedProperty });
			}
		}
	}
}
