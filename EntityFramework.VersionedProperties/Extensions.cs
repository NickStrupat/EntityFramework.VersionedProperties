using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.Triggers;
using System.Reflection;

namespace EntityFramework.VersionedProperties {
	public static class Extensions {
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
		/// Initialize all versioned properties in this object. This should be the called before doing anything with the versioned properties (at the beginning of the constructor, for example)
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
		public static void InitializeVersionedProperties<TVersionedProperties>(this TVersionedProperties versionedProperties)
			where TVersionedProperties : class, IVersionedProperties, ITriggerable, new()
		{
			var triggers = versionedProperties.Triggers();
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
