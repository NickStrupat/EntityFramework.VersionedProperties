using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Triggers;
using System.Reflection;

namespace EntityFramework.VersionedProperties {
	public static class Extensions {
		private static Func<Versioned> GetCtor(Type type) {
			return Expression.Lambda<Func<Versioned>>(Expression.New(type)).Compile();
		}
		private static Action<IVersionedProperties, Versioned> GetValueSetter(this PropertyInfo propertyInfo) {
			var instance = Expression.Parameter(typeof(IVersionedProperties), "i");
			var argument = Expression.Parameter(typeof(Versioned), "a");
			var setterCall = Expression.Call(
				Expression.Convert(instance, propertyInfo.DeclaringType),
				propertyInfo.SetMethod,
				Expression.Convert(argument, propertyInfo.PropertyType)
			);
			return Expression.Lambda<Action<IVersionedProperties, Versioned>>(setterCall, instance, argument).Compile();
		}

		private struct VersionedPropertyMapping {
			public VersionedPropertyMapping(PropertyInfo propertyInfo) : this() {
				NewVersioned = GetCtor(propertyInfo.PropertyType);
				SetVersioned = GetValueSetter(propertyInfo);
			}
			public Func<Versioned> NewVersioned { get; private set; }
			public Action<IVersionedProperties, Versioned> SetVersioned { get; private set; }
		}
		private static readonly ConcurrentDictionary<Type, VersionedPropertyMapping[]> versionedPropertyMappingsCache = new ConcurrentDictionary<Type, VersionedPropertyMapping[]>();
		private static VersionedPropertyMapping[] GetVersionedPropertyMappings(Type versionedPropertiesType) {
			if (!typeof(IVersionedProperties).IsAssignableFrom(versionedPropertiesType))
				throw new ArgumentException("Argument must be a Type which implements IVersionedProperties", "versionedPropertiesType");
			return versionedPropertyMappingsCache.GetOrAdd(
				versionedPropertiesType,
				v => v.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(Versioned).IsAssignableFrom(x.PropertyType)).Select(x => new VersionedPropertyMapping(x)).ToArray()
			);
		}

		/// <summary>
		/// Initialize all versioned properties in this object. This should be the called before doing anything to the versioned properties (at the beginning of the constructor, for example)
		/// </summary>
		/// <typeparam name="TVersionedProperties"></typeparam>
		/// <typeparam name="TDbContext"></typeparam>
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
		public static void InitializeVersionedProperties<TVersionedProperties, TDbContext>(this TVersionedProperties versionedProperties)
			where TVersionedProperties : class, IVersionedProperties, ITriggerable, new()
			where TDbContext : DbContext, IDbContextWithVersionedProperties
		{
			var triggers = versionedProperties.Triggers<TVersionedProperties, TDbContext>();
			var versionedPropertyMappings = GetVersionedPropertyMappings(typeof(TVersionedProperties));
			foreach (var versionedPropertyMapping in versionedPropertyMappings) {
				var versioned = versionedPropertyMapping.NewVersioned();
				versionedPropertyMapping.SetVersioned(versionedProperties, versioned);
				triggers.Inserting += entry => versioned.AddVersionsToDbContextWithVersionedProperties(entry.Context);
				triggers.Updating += entry => versioned.AddVersionsToDbContextWithVersionedProperties(entry.Context);
				triggers.Deleted += entry => versioned.DeleteVersionsFromDbContextWithVersionedProperties(entry.Context);
			}
		}
	}
}
