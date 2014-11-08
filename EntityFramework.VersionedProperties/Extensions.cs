using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Triggers;
using System.Reflection;

namespace EntityFramework.VersionedProperties {
	public static class Extensions {
		private struct VersionedPropertyMapping {
			public Func<IVersionedProperties, Versioned> GetInstantiatedVersioned { get; private set; }
			public VersionedPropertyMapping(PropertyInfo propertyInfo) : this() {
				var getter = GetValueGetter(propertyInfo);
				var ctor = GetCtor(propertyInfo.PropertyType);
				var setter = GetValueSetter(propertyInfo);
				GetInstantiatedVersioned = vp => {
					               var versioned = getter(vp);
								   if (versioned == null) {
									   versioned = ctor();
									   setter(vp, versioned);
								   }
					               return versioned;
				               };
			}
			private static Func<Versioned> GetCtor(Type type) {
				return Expression.Lambda<Func<Versioned>>(Expression.New(type)).Compile();
			}
			private static Action<IVersionedProperties, Versioned> GetValueSetter(PropertyInfo propertyInfo) {
				var instance = Expression.Parameter(typeof(IVersionedProperties));
				var argument = Expression.Parameter(typeof(Versioned));
				var setterCall = Expression.Call(
					Expression.Convert(instance, propertyInfo.DeclaringType),
					propertyInfo.SetMethod,
					Expression.Convert(argument, propertyInfo.PropertyType)
				);
				return Expression.Lambda<Action<IVersionedProperties, Versioned>>(setterCall, instance, argument).Compile();
			}
			private static Func<IVersionedProperties, Versioned> GetValueGetter(PropertyInfo propertyInfo) {
				var instance = Expression.Parameter(typeof(IVersionedProperties));
				var setterCall = Expression.Call(
					Expression.Convert(instance, propertyInfo.DeclaringType),
					propertyInfo.GetMethod
				);
				return Expression.Lambda<Func<IVersionedProperties, Versioned>>(setterCall, instance).Compile();
			}
		}
		private static readonly ConcurrentDictionary<Type, VersionedPropertyMapping[]> versionedPropertyMappingsCache = new ConcurrentDictionary<Type, VersionedPropertyMapping[]>();
		private static IEnumerable<VersionedPropertyMapping> GetVersionedPropertyMappings(Type versionedPropertiesType) {
#if DEBUG
			if (!typeof(IVersionedProperties).IsAssignableFrom(versionedPropertiesType))
				throw new ArgumentException("Argument must be a Type which implements IVersionedProperties", "versionedPropertiesType");
#endif
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
		public static void InitializeVersionedProperties<TVersionedProperties>(this TVersionedProperties versionedProperties)
			where TVersionedProperties : class, IVersionedProperties, ITriggerable, new()
		{
			var triggers = versionedProperties.Triggers<TVersionedProperties>();
			var versionedPropertyMappings = GetVersionedPropertyMappings(typeof(TVersionedProperties));
			foreach (var versionedPropertyMapping in versionedPropertyMappings) {
				var versioned = versionedPropertyMapping.GetInstantiatedVersioned(versionedProperties);
				triggers.Inserting += entry => versioned.AddVersionsToDbContextWithVersionedProperties((IDbContextWithVersionedProperties) entry.Context);
				triggers.Updating += entry => versioned.AddVersionsToDbContextWithVersionedProperties((IDbContextWithVersionedProperties) entry.Context);
				triggers.Deleted += entry => versioned.DeleteVersionsFromDbContextWithVersionedProperties((IDbContextWithVersionedProperties) entry.Context);
			}
		}
	}
}
