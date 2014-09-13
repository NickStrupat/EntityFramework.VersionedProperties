using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using EntityFrameworkTriggers;
using System.Reflection;

namespace EntityFrameworkVersionedProperties {
	public static class Extensions {
		private static readonly Dictionary<DbContext, PropertyInfo[]> dbContextVersionedPropertyInfoCache = new Dictionary<DbContext, PropertyInfo[]>();
		private static PropertyInfo[] getVersionedPropertyInfos(DbContext dbContext) {
			PropertyInfo[] versionPropertyInfos;
			if (!dbContextVersionedPropertyInfoCache.TryGetValue(dbContext, out versionPropertyInfos)) {
				versionPropertyInfos = dbContext.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => {
					return typeof(DbSet<>) == (x.PropertyType.IsGenericType ? x.PropertyType.GetGenericTypeDefinition() : null) &&
						   typeof(IVersion).IsAssignableFrom(x.PropertyType.GenericTypeArguments.Single());
				}).ToArray();
				dbContextVersionedPropertyInfoCache.Add(dbContext, versionPropertyInfos);
			}
			return versionPropertyInfos;
		}
		private static readonly Dictionary<IVersionable, PropertyInfo[]> versionablePropertyInfoCache = new Dictionary<IVersionable, PropertyInfo[]>();
		private static PropertyInfo[] getVersionablePropertyInfos(IVersionable versionable) {
			PropertyInfo[] versionPropertyInfos;
			if (!versionablePropertyInfoCache.TryGetValue(versionable, out versionPropertyInfos)) {
				versionPropertyInfos = versionable.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(IVersioned).IsAssignableFrom(x.PropertyType)).ToArray();
				versionablePropertyInfoCache.Add(versionable, versionPropertyInfos);
			}
			return versionPropertyInfos;
		}
		internal struct Mapping {
			public PropertyInfo VersionedProperty { get; set; }
			public PropertyInfo Versions { get; set; }
		}
		private static readonly Dictionary<Tuple<IVersionable, DbContext>, Mapping[]> mappingsCache = new Dictionary<Tuple<IVersionable, DbContext>, Mapping[]>();
		private static Mapping[] getMappings(IVersionable versionable, DbContext dbContext) {
			var pair = new Tuple<IVersionable, DbContext>(versionable, dbContext);
			Mapping[] mappings;
			if (!mappingsCache.TryGetValue(pair, out mappings)) {
				var mappingsQuery = from versionedPropertyInfo in getVersionablePropertyInfos(versionable)
									from versionPropertyInfo in getVersionedPropertyInfos(dbContext)
				                    where versionPropertyInfo.PropertyType.GenericTypeArguments.Single() == versionedPropertyInfo.PropertyType.BaseType.GenericTypeArguments[1]
				                    select new Mapping {VersionedProperty = versionedPropertyInfo, Versions = versionPropertyInfo};
				mappings = mappingsQuery.ToArray();
				mappingsCache.Add(pair, mappings);
			}
			return mappings;
		}

		public static void InitializeVersionedProperties<TVersionable>(this IVersionable<TVersionable> versionable)
			where TVersionable : class, IVersionable<TVersionable>, ITriggerable<TVersionable>, new() {
			var versionedPropertyInfos = getVersionablePropertyInfos(versionable);
			foreach (var propertyInfo in versionedPropertyInfos) {
				var versioned = Activator.CreateInstance(propertyInfo.PropertyType);
				propertyInfo.SetValue(versionable, versioned);
			}
			var versionableInstance = (TVersionable) versionable;
			var triggers = versionableInstance.Triggers();
			triggers.Inserting += OnInsertingOrUpdating;
			triggers.Updating += OnInsertingOrUpdating;
			triggers.Deleting += OnDeleting;
		}
		private static void OnInsertingOrUpdating<TVersionable>(Triggers<TVersionable>.Entry e)
			where TVersionable : class, IVersionable<TVersionable>, ITriggerable<TVersionable>, new() {
			foreach (var mapping in getMappings(e.Entity, e.Context)) {
				var versionedProperty = mapping.VersionedProperty.GetValue(e.Entity);
				var versionedVersionsPropertyInfo = mapping.VersionedProperty.PropertyType.GetField("Versions", BindingFlags.Instance | BindingFlags.NonPublic);
				var versionedVersions = (IEnumerable<Object>) versionedVersionsPropertyInfo.GetValue(versionedProperty);
				var dbSet = mapping.Versions.GetValue(e.Context);
				var addRangeMethodInfo = mapping.Versions.PropertyType.GetMethod("AddRange");
				addRangeMethodInfo.Invoke(dbSet, new[] {versionedVersions});
				versionedVersionsPropertyInfo.FieldType.GetMethod("Clear").Invoke(versionedVersions, null);
			}
		}
		private static readonly MethodInfo deleteMethodInfo = typeof(BatchExtensions).GetMethods().Single(x => x.Name == "Delete" && x.GetParameters().Count() == 1 && typeof(IQueryable).IsAssignableFrom(x.GetParameters().Single().ParameterType));
		private static readonly MethodInfo whereMethodInfo = typeof(Queryable).GetMethods().Single(x => x.Name == "Where" && x.GetParameters().Count() == 2 && typeof(IQueryable).IsAssignableFrom(x.GetParameters()[0].ParameterType) && typeof(LambdaExpression).IsAssignableFrom(x.GetParameters()[1].ParameterType) && x.GetParameters()[1].ParameterType.GenericTypeArguments.Single().GenericTypeArguments.Count() == 2);
		private static void OnDeleting<TVersionable>(Triggers<TVersionable>.Entry e)
			where TVersionable : class, IVersionable<TVersionable>, ITriggerable<TVersionable>, new() {
			foreach (var mapping in getMappings(e.Entity, e.Context)) {
				var dbSet = mapping.Versions.GetValue(e.Context);
				var versionedProperty = mapping.VersionedProperty.GetValue(e.Entity);
				var versionParameter = Expression.Parameter(mapping.Versions.PropertyType.GenericTypeArguments.Single(), "version");
				var versionVersionedIdProperty = Expression.Property(versionParameter, "VersionedId");
				var versionedPropertyId = mapping.VersionedProperty.PropertyType.GetProperty("Id").GetValue(versionedProperty);
				var versionedIdProperty = Expression.Constant(versionedPropertyId, typeof(Guid));
				var equal = Expression.Equal(versionVersionedIdProperty, versionedIdProperty);
				var whereExpression = Expression.Lambda(equal, versionParameter);
				var whereGenericMethodInfo = whereMethodInfo.MakeGenericMethod(mapping.Versions.PropertyType.GenericTypeArguments.Single());
				var where = whereGenericMethodInfo.Invoke(null, new [] { dbSet, whereExpression });
				var deleteMethodInfoGeneric = deleteMethodInfo.MakeGenericMethod(mapping.Versions.PropertyType.GenericTypeArguments.Single());
				deleteMethodInfoGeneric.Invoke(null, new[] { where });

				var versionedVersionsPropertyInfo = mapping.VersionedProperty.PropertyType.GetField("Versions", BindingFlags.Instance | BindingFlags.NonPublic);
				var versionedVersions = (IEnumerable<Object>)versionedVersionsPropertyInfo.GetValue(versionedProperty);
				versionedVersionsPropertyInfo.FieldType.GetMethod("Clear").Invoke(versionedVersions, null);
			}
		}
	}
}
