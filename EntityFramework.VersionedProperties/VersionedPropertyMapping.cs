using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFramework.VersionedProperties {
	internal class VersionedPropertyMapping {
		public Func<IVersionedProperties, VersionedType> GetInstantiatedVersioned { get; private set; }

		public VersionedPropertyMapping(PropertyInfo propertyInfo) {
			var getter = GetValueGetter(propertyInfo);
			var ctor = GetCtor(propertyInfo.PropertyType);
			var setter = GetValueSetter(propertyInfo);
			GetInstantiatedVersioned = vp => {
				                           var versioned = getter(vp);
				                           if (versioned == null) {
					                           versioned = ctor();
					                           setter(vp, versioned);
				                           }
				                           return versioned;
			                           };
		}

		private static Func<VersionedType> GetCtor(Type type) {
			return Expression.Lambda<Func<VersionedType>>(Expression.New(type)).Compile();
		}

		private static Action<IVersionedProperties, VersionedType> GetValueSetter(PropertyInfo propertyInfo) {
			var instance = Expression.Parameter(typeof (IVersionedProperties));
			var argument = Expression.Parameter(typeof (VersionedType));
			var setterCall = Expression.Call(
				Expression.Convert(instance, propertyInfo.DeclaringType),
				propertyInfo.GetSetMethod(true),
				Expression.Convert(argument, propertyInfo.PropertyType)
				);
			return Expression.Lambda<Action<IVersionedProperties, VersionedType>>(setterCall, instance, argument).Compile();
		}

		private static Func<IVersionedProperties, VersionedType> GetValueGetter(PropertyInfo propertyInfo) {
			var instance = Expression.Parameter(typeof (IVersionedProperties));
			var setterCall = Expression.Call(
				Expression.Convert(instance, propertyInfo.DeclaringType),
				propertyInfo.GetGetMethod()
				);
			return Expression.Lambda<Func<IVersionedProperties, VersionedType>>(setterCall, instance).Compile();
		}
	}
}