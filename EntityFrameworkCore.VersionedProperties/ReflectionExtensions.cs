using System;
using System.Reflection;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
namespace EntityFramework.VersionedProperties {
#endif
	internal static class ReflectionExtensions {
		public static Func<T, TProperty> GetPropertyGetter<T, TProperty>(this PropertyInfo propertyInfo) {
#if NET40
			return (Func<T, TProperty>) Delegate.CreateDelegate(typeof(Func<T, TProperty>), propertyInfo?.GetGetMethod(nonPublic: true), throwOnBindFailure: true);
#else
			return (Func<T, TProperty>) propertyInfo.GetGetMethod(nonPublic: true).CreateDelegate(typeof(Func<T, TProperty>));
#endif
		}

		public static Action<T, TProperty> GetPropertySetter<T, TProperty>(this PropertyInfo propertyInfo) {
#if NET40
			return (Action<T, TProperty>) Delegate.CreateDelegate(typeof(Action<T, TProperty>), propertyInfo?.GetSetMethod(nonPublic: true), throwOnBindFailure:true);
#else
			return (Action<T, TProperty>) propertyInfo.GetSetMethod(nonPublic: true).CreateDelegate(typeof(Action<T, TProperty>));
#endif
		}

#if NET40
		public static MethodInfo GetMethodInfo(this Delegate del) => del.Method;
#endif
	}
}
