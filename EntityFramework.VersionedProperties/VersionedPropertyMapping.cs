﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFramework.VersionedProperties {
	internal class VersionedPropertyMapping {
		public Func<IVersionedProperties, Versioned> GetInstantiatedVersioned { get; private set; }

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

		private static Func<Versioned> GetCtor(Type type) {
			return Expression.Lambda<Func<Versioned>>(Expression.New(type)).Compile();
		}

		private static Action<IVersionedProperties, Versioned> GetValueSetter(PropertyInfo propertyInfo) {
			var instance = Expression.Parameter(typeof (IVersionedProperties));
			var argument = Expression.Parameter(typeof (Versioned));
            var method = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name).GetSetMethod(true);
            if (method == null)
                throw new InvalidOperationException("Property setter not found");
            var setterCall = Expression.Call(
				Expression.Convert(instance, propertyInfo.DeclaringType),
                method,
				Expression.Convert(argument, propertyInfo.PropertyType)
				);
			return Expression.Lambda<Action<IVersionedProperties, Versioned>>(setterCall, instance, argument).Compile();
		}

		private static Func<IVersionedProperties, Versioned> GetValueGetter(PropertyInfo propertyInfo) {
			var instance = Expression.Parameter(typeof (IVersionedProperties));
			var setterCall = Expression.Call(
				Expression.Convert(instance, propertyInfo.DeclaringType),
				propertyInfo.GetGetMethod()
				);
			return Expression.Lambda<Func<IVersionedProperties, Versioned>>(setterCall, instance).Compile();
		}
	}
}