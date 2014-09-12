using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkTriggers;
using System.Reflection;

namespace EntityFrameworkVersionedProperties {
	public static class Extensions {
		//public static Int32 SaveChangesWithVersioning(this DbContext dbContext) {
		//	dbContext.ConfigureVerionableEntities();
		//	return dbContext.SaveChangesWithTriggers();
		//}
		//public static Task<Int32> SaveChangesWithVersioningAsync(this DbContext dbContext) {
		//	return dbContext.SaveChangesWithTriggersAsync();
		//}
		//public static Task<Int32> SaveChangesWithVersioningAsync(this DbContext dbContext, CancellationToken cancellationToken) {
		//	return dbContext.SaveChangesWithTriggersAsync(cancellationToken);
		//}

		//private static void ConfigureVerionableEntities(this DbContext dbContext) {
		//	foreach (var entry in dbContext.ChangeTracker.Entries<IVersionable>()) {
		//		switch (entry.State) {
		//			case EntityState.Added:
		//				break;
		//			case EntityState.Deleted:
		//				break;
		//			case EntityState.Modified:
		//				break;
		//		}
		//	}
		//}

		public static void InitializeVerionedProperties(this IVersionable versionable) {
			foreach (var propertyInfo in versionable.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof (IVersioned).IsAssignableFrom(x.PropertyType))) {
				var versioned = (IVersioned)Activator.CreateInstance(propertyInfo.PropertyType);
				propertyInfo.SetValue(versionable, versioned);
				
			}
		}
	}
}
