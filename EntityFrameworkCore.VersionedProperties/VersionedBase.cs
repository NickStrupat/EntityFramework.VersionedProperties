using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Z.EntityFramework.Plus;
using System.Reflection;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	[DebuggerDisplay("Value = {Value}")]
	public abstract class VersionedBase<TVersioned, TValue, TVersion, TIVersions> : IVersioned
	where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>
	where TVersion : VersionBase<TValue>, new()
	where TIVersions : class {
		/// <summary>Gets the unique identifier for this versioned property</summary>
		public Guid Id { get; private set; } = Guid.Empty;
		
		/// <summary>Gets the date-time representing when this versioned property was last modified</summary>
		public DateTime Modified { get; internal set; } = DateTime.UtcNow;

#if !DEBUG
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
		private Boolean isInitialValue = true;

		/// <summary>Gets the local versions (not yet persisted)</summary>
		public IEnumerable<TVersion> LocalVersions => internalLocalVersions;
#if !DEBUG
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
		private readonly List<TVersion> internalLocalVersions = new List<TVersion>();

		/// <summary>Gets a boolean indicating the read-only state of <see cref="Value"/></summary>
		[NotMapped]
#if DEBUG
		public Boolean IsReadOnly { get; set; }
#else
		public Boolean IsReadOnly { get; internal set; }
#endif

#if !DEBUG
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
#if DEBUG
		public TValue value;
#else
		internal TValue value;
#endif
		/// <summary>Gets or sets the value of this versioned property (the previous value is pushed into the versions collection for <see cref="TValue"/>)</summary>
		public TValue Value {
			get { return value; }
			set {
				if (IsReadOnly)
					throw new InvalidOperationException("This object is in a read-only state, possibly because it has been ");
				if (isInitialValue)
					isInitialValue = false;
				else {
					if (EqualityComparer<TValue>.Default.Equals(this.value, value))
						return;
					if (Id == Guid.Empty)
						Id = Guid.NewGuid();
					internalLocalVersions.Add(new TVersion {
						VersionedId = Id,
						Added = Modified,
						Value = Value
					});
				}
				Modified = DateTime.UtcNow;
				this.value = value;
			}
		}

		protected VersionedBase() {
			value = DefaultValue;
		}

#if !DEBUG
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
		protected virtual TValue DefaultValue => default(TValue);
		protected virtual DbSet<TVersion> GetVersionDbSet(TIVersions x) => GetVersionDbSetFunc(x);
		private static readonly Func<TIVersions, DbSet<TVersion>> GetVersionDbSetFunc = typeof(TIVersions).GetProperties()
		                                                                                                  .Single(x => x.PropertyType == typeof(DbSet<TVersion>))
		                                                                                                  .GetPropertyGetter<TIVersions, DbSet<TVersion>>();
#if DEBUG
		public static DbSet<TVersion> GetVersionDbSetStatic(TIVersions x) => GetVersionDbSetFunc(x);
#endif

		public override String ToString() => Value?.ToString() ?? String.Empty;
		/// <summary>Gets the previous versions</summary>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public IOrderedQueryable<TVersion> GetVersions(TIVersions dbContext) => GetVersionDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);

#if DEBUG && !NET_CORE
		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public IEnumerable<TVersion> Versions {
			get {
				var type = Assembly.GetCallingAssembly().GetTypes().Single(x => typeof(DbContext).IsAssignableFrom(x) && typeof(TIVersions).IsAssignableFrom(x));
				using (var dbContext = (DbContext) Activator.CreateInstance(type))
					return GetVersions(dbContext as TIVersions).ToArray();
			}
		}
#endif

#region IVersioned implementations
		void IVersioned.AddVersionsToDbContext(DbContext dbContext) {
			var versions = CheckDbContext(dbContext);
			GetVersionDbSet(versions).AddRange(internalLocalVersions);
		}

		void IVersioned.RemoveVersionsFromDbContext(DbContext dbContext) {
			var versions = CheckDbContext(dbContext);
			GetVersionDbSet(versions).Where(x => x.VersionedId == Id).Delete();
			internalLocalVersions.Clear();
		}

		private static TIVersions CheckDbContext(DbContext dbContext) {
			var iversions = dbContext as TIVersions;
			if (iversions != null)
				return iversions;
			throw new InvalidOperationException("Your DbContext class must implement " + typeof(TIVersions).FullName);
		}

		void IVersioned.SetIsInitialValueFalse() => isInitialValue = false;
		void IVersioned.ClearInternalLocalVersions() => internalLocalVersions.Clear();
#endregion
	}

	internal static class TypeExtensions {
#if NET40
		internal static Type GetTypeInfo(this Type type) => type;
#else
		public static Type[] GetGenericArguments(this TypeInfo ti) => ti.GenericTypeArguments;
#endif
	}

	public static class VersionedExtensions {
		public static ICollection<T> ToSnapshots<T>(this IQueryable<T> source, DbContext context, DateTime dateTime)
		where T : class {
			var vpis = typeof(T).GetTypeInfo().GetProperties().Where(x => typeof(IVersioned).IsAssignableFrom(x.PropertyType));
			var query = source;
			foreach (var vpi in vpis) {
				var vbt = GetVersionedBaseType(vpi.PropertyType);
				var gas = vpi.PropertyType.GetTypeInfo().GetGenericArguments();
				//query = query.GroupJoin()
			}

			return null;
		}

		private static class Snapshots<T>
		where T : class {
			private static readonly PropertyInfo[] Vpis = typeof(T).GetTypeInfo().GetProperties().Where(x => typeof(IVersioned).IsAssignableFrom(x.PropertyType)).ToArray();
			private static readonly GenericTypes[] GenericTypes = Vpis.Select(x => new GenericTypes(GetVersionedBaseType(x.PropertyType).GetTypeInfo().GetGenericArguments())).ToArray();
		}

		private struct GenericTypes {
			public Type Versioned;
			public Type Value;
			public Type Version;
			public Type IVersions;

			public GenericTypes(Type[] genericArguments) : this(genericArguments[0], genericArguments[1], genericArguments[2], genericArguments[3]) {}
			public GenericTypes(Type versioned, Type value, Type version, Type versions) {
				Versioned = versioned;
				Value = value;
				Version = version;
				IVersions = versions;
			}
		}

		private static Type GetVersionedBaseType(Type type) {
			while (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(VersionedBase<,,,>))
				type = type.BaseType;
			return type;
		}
	}
}