using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFramework.VersionedProperties {
	internal class VersionedPropertyMapping {
		private readonly Func<IVersionedProperties, IVersioned> getter;
		private readonly Func<IVersioned> ctor;
		private readonly Action<IVersionedProperties, IVersioned> setter;

		public VersionedPropertyMapping(PropertyInfo propertyInfo) {
			getter = GetValueGetter(propertyInfo);
			ctor = GetCtor(propertyInfo.PropertyType);
			setter = GetValueSetter(propertyInfo);
		}

		public IVersioned GetInstantiatedVersioned(IVersionedProperties vp) {
			var versioned = getter(vp);
			if (versioned == null) {
				versioned = ctor();
				setter(vp, versioned);
			}
			return versioned;
		}

		private static Func<IVersioned> GetCtor(Type type) => Expression.Lambda<Func<IVersioned>>(Expression.New(type)).Compile();

		private static Action<IVersionedProperties, IVersioned> GetValueSetter(PropertyInfo propertyInfo) {
			var instance = Expression.Parameter(typeof(IVersionedProperties));
			var argument = Expression.Parameter(typeof(IVersioned));
			var method = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name).GetSetMethod(true);
			if (method == null)
				throw new InvalidOperationException("Property setter not found");
			var setterCall = Expression.Call(
				Expression.Convert(instance, propertyInfo.DeclaringType),
				method,
				Expression.Convert(argument, propertyInfo.PropertyType)
				);
			return Expression.Lambda<Action<IVersionedProperties, IVersioned>>(setterCall, instance, argument).Compile();
		}

		private static Func<IVersionedProperties, IVersioned> GetValueGetter(PropertyInfo propertyInfo) {
			var instance = Expression.Parameter(typeof(IVersionedProperties));
			var call = Expression.Call(
				Expression.Convert(instance, propertyInfo.DeclaringType),
				propertyInfo.GetGetMethod()
				);
			return Expression.Lambda<Func<IVersionedProperties, IVersioned>>(call, instance).Compile();
		}
	}
}