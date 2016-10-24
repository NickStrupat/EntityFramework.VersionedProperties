using System.Data.Entity;
using System.Linq;
using System.Reflection;

using EntityFramework.Triggers;

namespace EntityFramework.VersionedProperties {
	internal static class StaticCache<TVersionedProperties> where TVersionedProperties : class, IVersionedProperties {
		public static void Initialize(IVersionedProperties versionedProperties) {
			foreach (var versionedPropertyMapping in versionedPropertyMappings)
				versionedPropertyMapping.GetInstantiatedVersioned(versionedProperties);
		}

		private static readonly VersionedPropertyMapping[] versionedPropertyMappings = typeof(TVersionedProperties).GetProperties(BindingFlags.Public | BindingFlags.Instance)
		                                                                                                           .Where(x => typeof(IVersioned).IsAssignableFrom(x.PropertyType))
		                                                                                                           .Select(x => new VersionedPropertyMapping(x))
		                                                                                                           .ToArray();

		static StaticCache() {
			Triggers<TVersionedProperties>.Inserting += OnInsertingOrUpdating;
			Triggers<TVersionedProperties>.Updating += OnInsertingOrUpdating;
			Triggers<TVersionedProperties>.Inserted += OnInserted;
			Triggers<TVersionedProperties>.Deleted += OnDeleted;
		}

		private static void OnInsertingOrUpdating(IBeforeEntry<TVersionedProperties, DbContext> entry) {
			foreach (var versionedPropertyMapping in versionedPropertyMappings)
				versionedPropertyMapping.GetInstantiatedVersioned(entry.Entity).AddVersionsToDbContext(entry.Context);
		}

		private static void OnInserted(IEntry<TVersionedProperties, DbContext> entry) {
			foreach (var versionedPropertyMapping in versionedPropertyMappings)
				versionedPropertyMapping.GetInstantiatedVersioned(entry.Entity).SetIsDefaultValueFalse();
		}

		private static void OnDeleted(IEntry<TVersionedProperties, DbContext> entry) {
			foreach (var versionedPropertyMapping in versionedPropertyMappings)
				versionedPropertyMapping.GetInstantiatedVersioned(entry.Entity).RemoveVersionsFromDbContext(entry.Context);
		}
	}
}