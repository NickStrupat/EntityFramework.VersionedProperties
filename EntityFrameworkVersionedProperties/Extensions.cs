using System;
using System.Data.Entity;
using System.Linq;
using EntityFrameworkTriggers;
using System.Reflection;

namespace EntityFrameworkVersionedProperties {
	public static class Extensions {
		public static void InitializeVersionedProperties<TVersionable, TDbContext>(this IVersionable<TVersionable, TDbContext> versionable)
			where TVersionable : class, IVersionable<TVersionable, TDbContext>, ITriggerable<TVersionable>, new()
			where TDbContext : DbContext {
			var versionableInstance = (TVersionable) versionable;
			var triggers = versionableInstance.Triggers();
			versionableInstance.Triggers().Inserting += e => e.GetContext<TDbContext>();
			foreach (var propertyInfo in versionable.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof (IVersioned).IsAssignableFrom(x.PropertyType))) {
				var versioned = (IVersioned)Activator.CreateInstance(propertyInfo.PropertyType);
				propertyInfo.SetValue(versionable, versioned);
			}
		}
	}
}
