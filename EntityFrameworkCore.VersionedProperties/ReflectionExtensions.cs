using System;
using System.Linq.Expressions;
using System.Reflection;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
namespace EntityFramework.VersionedProperties {
#endif
	internal static class ReflectionExtensions {
		public static Func<T, TProperty> GetPropertyGetter<T, TProperty>(this PropertyInfo propertyInfo) {
			var instance = Expression.Parameter(typeof(T));
			var convert = Expression.Convert(instance, propertyInfo.DeclaringType);
			var call = Expression.Call(convert, propertyInfo.GetGetMethod());
			return Expression.Lambda<Func<T, TProperty>>(call, instance).Compile();
		}
	}
}
