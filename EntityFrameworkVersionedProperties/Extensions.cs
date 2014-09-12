using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFrameworkTriggers;
using System.Reflection;

namespace EntityFrameworkVersionedProperties {
	public static class Extensions {
		public static void InitializeVersionedProperties<TVersionable, TDbContext>(this IVersionable<TVersionable, TDbContext> versionable)
			where TVersionable : class, IVersionable<TVersionable, TDbContext>, ITriggerable<TVersionable>, new()
			where TDbContext : DbContext
		{
			var versionedPropertyInfos = versionable.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof (IVersioned).IsAssignableFrom(x.PropertyType)).ToArray();
			var versionPropertyInfos = typeof(TDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => {
				return typeof (IDbSet<>) == (x.PropertyType.IsGenericType ? x.PropertyType.GetGenericTypeDefinition() : null) &&
					   typeof (IVersion).IsAssignableFrom(x.PropertyType.GenericTypeArguments.Single());
			}).ToArray();
			var mappings = (from versionedPropertyInfo in versionedPropertyInfos
							from versionPropertyInfo in versionPropertyInfos
							where versionPropertyInfo.PropertyType.GenericTypeArguments.Single() == versionedPropertyInfo.PropertyType.BaseType.GenericTypeArguments[1]
							select new { VersionedProperty = versionedPropertyInfo, Versions = versionPropertyInfo }).ToArray();
			
			foreach (var propertyInfo in versionedPropertyInfos) {
				var versioned = Activator.CreateInstance(propertyInfo.PropertyType);
				propertyInfo.SetValue(versionable, versioned);
			}
			var versionableInstance = (TVersionable) versionable;
			var triggers = versionableInstance.Triggers();
			versionableInstance.Triggers().Inserting += e => {
				                                            foreach (var mapping in mappings) {
					                                            var versionedProperty = mapping.VersionedProperty.GetValue(e.Entity);
					                                            var huh = mapping.VersionedProperty.PropertyType.GetField("Versions", BindingFlags.Instance | BindingFlags.NonPublic);
																var what = (IEnumerable<Object>)huh.GetValue(versionedProperty);
					                                            var dbSet = mapping.Versions.GetValue(e.Context);
					                                            var addMethodInfo = mapping.Versions.PropertyType.GetMethod("Add");
					                                            foreach (var version in what) {
						                                            addMethodInfo.Invoke(dbSet, new []{ version });
					                                            }
				                                            }
			};
		}
	}
}
