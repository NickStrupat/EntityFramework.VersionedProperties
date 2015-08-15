using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.Triggers;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EntityFramework.VersionedProperties {
	public static class Extensions {
		private static readonly ConcurrentDictionary<Type, VersionedPropertyMapping[]> versionedPropertyMappingsCache = new ConcurrentDictionary<Type, VersionedPropertyMapping[]>();

		private static IEnumerable<VersionedPropertyMapping> GetVersionedPropertyMappings(IVersionedProperties versionedProperties) {
			return versionedPropertyMappingsCache.GetOrAdd(
				versionedProperties.GetType(),
				v => v.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				      .Where(x => typeof(IVersioned).IsAssignableFrom(x.PropertyType))
				      .Select(x => new VersionedPropertyMapping(x))
				      .ToArray()
			);
		}

		private static readonly ConditionalWeakTable<IVersionedProperties, Object> initializedVersionedProperties = new ConditionalWeakTable<IVersionedProperties, Object>();

		private static Boolean TryMarkInitialized(IVersionedProperties versionedProperties) {
			lock (initializedVersionedProperties) {
				Object hold;
				if (initializedVersionedProperties.TryGetValue(versionedProperties, out hold))
					return true;
				initializedVersionedProperties.Add(versionedProperties, null);
				return false;
			}
		}

		/// <summary>
		/// Initialize all versioned properties in this object. This should be the called before doing anything with the versioned properties (at the beginning of the constructor, for example)
		/// </summary>
		/// <typeparam name="TVersionedProperties"></typeparam>
		/// <param name="versionedProperties"></param>
		/// <example>
		/// <code>
		/// 	class Person : IVersionedProperties&lt;Person&gt; {
		/// 		public String Name { get; set; }
		/// 		public VersionedString NickName { get; set; }
		/// 		public Person(String name, String nickName) {
		/// 			this.InitializeVersionedProperties();
		/// 			Name = name;
		/// 			NickName.Value = nickName;
		/// 		}
		/// 	}
		/// </code>
		/// </example>
		public static void InitializeVersionedProperties<TVersionedProperties>(this TVersionedProperties versionedProperties) where TVersionedProperties : class, IVersionedProperties, ITriggerable {
			if (versionedProperties == null)
				throw new ArgumentNullException(nameof(versionedProperties));
			if (TryMarkInitialized(versionedProperties))
				return;
			var triggers = versionedProperties.Triggers();
			var versionedPropertyMappings = GetVersionedPropertyMappings(versionedProperties);
			foreach (var versionedPropertyMapping in versionedPropertyMappings) {
				var versioned = versionedPropertyMapping.GetInstantiatedVersioned(versionedProperties);
				triggers.Inserting += entry => versioned.AddVersionsToDbContext(entry.Context);
				triggers.Updating += entry => versioned.AddVersionsToDbContext(entry.Context);
				triggers.Deleted += entry => versioned.RemoveVersionsFromDbContext(entry.Context);
			}
		}
	}
}
