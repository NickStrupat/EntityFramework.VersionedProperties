using System;
using System.Collections.Concurrent;

namespace EntityFramework.VersionedProperties {
	public static class Extensions {
		/// <summary>
		/// Initialize all versioned properties in this object. This should be the called before doing anything with the versioned properties (at the beginning of the constructor, for example)
		/// </summary>
		/// <typeparam name="TVersionedProperties"></typeparam>
		/// <param name="versionedProperties"></param>
		/// <example>
		/// <code>
		/// 	class Person : IVersionedProperties&lt;Person&gt; {
		/// 		public String Name { get; set; }
		/// 		public VersionedString NickName { get; set; }
		/// 		public Person(String name, String nickName) {
		/// 			this.InitializeVersionedProperties();
		/// 			Name = name;
		/// 			NickName.Value = nickName;
		/// 		}
		/// 	}
		/// </code>
		/// </example>
		public static void InitializeVersionedProperties(this IVersionedProperties versionedProperties) {
			if (versionedProperties == null)
				throw new ArgumentNullException(nameof(versionedProperties));
			var initializer = initializerCache.GetOrAdd(versionedProperties.GetType(), GetInitializerDelegate);
			initializer(versionedProperties);
		}

		private static Action<IVersionedProperties> GetInitializerDelegate(Type type) {
			return (Action<IVersionedProperties>) Delegate.CreateDelegate(typeof(Action<IVersionedProperties>), typeof(StaticCache<>).MakeGenericType(type).GetMethod(nameof(StaticCache<IVersionedProperties>.Initialize)));
		}

		private static readonly ConcurrentDictionary<Type, Action<IVersionedProperties>> initializerCache = new ConcurrentDictionary<Type, Action<IVersionedProperties>>();
	}
}
