using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
#if EF_CORE
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Triggers;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
using EntityFramework.Triggers;
namespace EntityFramework.VersionedProperties {
#endif
	public static class VersionedProperties<TVersionedProperties>
	where TVersionedProperties : class {
		public static void Initialize() => VersionedProperties<TVersionedProperties, DbContext>.Initialize();
	}

	public static class VersionedProperties<TVersionedProperties, TDbContext>
	where TVersionedProperties : class
	where TDbContext : DbContext {
		private static Boolean initialized;
		private static readonly Object syncRoot = new Object();

		public static void Initialize() {
			if (initialized)
				return;
			lock (syncRoot) {
				if (initialized)
					return;
				Triggers<TVersionedProperties, TDbContext>.Inserting += OnInsertingOrUpdating;
				Triggers<TVersionedProperties, TDbContext>.Updating += OnInsertingOrUpdating;
				Triggers<TVersionedProperties, TDbContext>.Inserted += OnInsertedOrUpdated;
				Triggers<TVersionedProperties, TDbContext>.Updated += OnInsertedOrUpdated;
				Triggers<TVersionedProperties, TDbContext>.Deleted += OnDeleted;
				initialized = true;
			}
		}

		private static readonly Func<TVersionedProperties, IVersioned>[] versionedPropertyGetters = typeof(TVersionedProperties).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
		                                                                                                                        .Where(x => typeof(IVersioned).IsAssignableFrom(x.PropertyType))
		                                                                                                                        .Select(ReflectionExtensions.GetPropertyGetter<TVersionedProperties, IVersioned>)
		                                                                                                                        .ToArray();

		private static void OnInsertingOrUpdating(IBeforeEntry<TVersionedProperties, TDbContext> entry) {
			foreach (var versionedPropertyMapping in versionedPropertyGetters)
				versionedPropertyMapping(entry.Entity)?.OnInsertingOrUpdating(entry.Context);
		}

		private static void OnInsertedOrUpdated(IAfterEntry<TVersionedProperties, TDbContext> entry) {
			foreach (var versionedPropertyMapping in versionedPropertyGetters)
				versionedPropertyMapping(entry.Entity)?.OnInsertedOrUpdated();
		}

		private static void OnDeleted(IEntry<TVersionedProperties, TDbContext> entry) {
			foreach (var versionedPropertyMapping in versionedPropertyGetters)
				versionedPropertyMapping(entry.Entity)?.OnDeleted(entry.Context);
		}
	}
}
