using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mutuples;
using Z.EntityFramework.Plus;

#if !NET40
using System.Threading.Tasks;
#endif

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	[DebuggerDisplay("Value = {Value}")]
	public abstract class VersionedBase<TVersioned, TValue, TVersion, TIVersions> : IVersioned, INotifyPropertyChanging, INotifyPropertyChanged
	where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>
	where TVersion : VersionBase<TValue>, new()
	where TIVersions : class {
		protected VersionedBase() {
			id = Guid.Empty;
			modified = default(DateTime);
			isInitialValue = true;
			value = DefaultValue;
			internalLocalVersions = new ObservableCollection<TVersion>();
			LocalVersions = new ReadOnlyObservableCollection<TVersion>(internalLocalVersions);
		}

		public override String ToString() => Value?.ToString() ?? String.Empty;

		/// <summary>Gets the unique identifier for this versioned property</summary>
		public Guid Id {
			get { return id; }
			private set { NotifyChangeIfNotEqual(ref id, value, idPropertyChangingEventArgs, idPropertyChangedEventArgs); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Guid id;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PropertyChangingEventArgs idPropertyChangingEventArgs = new PropertyChangingEventArgs(nameof(Id));
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PropertyChangedEventArgs idPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Id));

		/// <summary>Gets the date-time representing when this versioned property was last modified</summary>
		public DateTime Modified {
			get { return modified; }
			internal set { NotifyChangeIfNotEqual(ref modified, value, modifiedPropertyChangingEventArgs, modifiedPropertyChangedEventArgs); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private DateTime modified;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PropertyChangingEventArgs modifiedPropertyChangingEventArgs = new PropertyChangingEventArgs(nameof(Modified));
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PropertyChangedEventArgs modifiedPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Modified));

		/// <summary>Gets a boolean indicating the read-only state of <see cref="Value"/></summary>
		[NotMapped]
		public Boolean IsReadOnly {
			get { return isReadOnly; }
			private set { NotifyChangeIfNotEqual(ref isReadOnly, value, isReadOnlyPropertyChangingEventArgs, isReadOnlyPropertyChangedEventArgs); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Boolean isReadOnly;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PropertyChangingEventArgs isReadOnlyPropertyChangingEventArgs = new PropertyChangingEventArgs(nameof(IsReadOnly));
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PropertyChangedEventArgs isReadOnlyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsReadOnly));

		/// <summary>Gets or sets the value of this versioned property (the previous value is pushed into the versions collection for <see cref="TVersion"/> in <see cref="TIVersions"/>)</summary>
		public TValue Value {
			get { return value; }
			set {
				if (IsReadOnly)
					throw new InvalidOperationException("This object is in a read-only state, possibly because it is a snapshot of a previous state");
				if (isInitialValue || Id != Guid.Empty)
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
				NotifyChange(ref this.value, value, valuePropertyChangingEventArgs, valuePropertyChangedEventArgs);
				Modified = DateTime.UtcNow;
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TValue value;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PropertyChangingEventArgs valuePropertyChangingEventArgs = new PropertyChangingEventArgs(nameof(Value));
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PropertyChangedEventArgs valuePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Boolean isInitialValue;
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ObservableCollection<TVersion> internalLocalVersions;
		
		/// <summary>Gets the local versions (not yet persisted)</summary>
		public ReadOnlyObservableCollection<TVersion> LocalVersions { get; }
		
#if !DEBUG
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
		protected virtual TValue DefaultValue => default(TValue);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Func<TIVersions, DbSet<TVersion>> GetVersionDbSetFunc = typeof(TIVersions).GetProperties()
		                                                                                                  .Single(x => x.PropertyType == typeof(DbSet<TVersion>))
		                                                                                                  .GetPropertyGetter<TIVersions, DbSet<TVersion>>();
		public static DbSet<TVersion> GetVersionDbSet(TIVersions dbContext) => GetVersionDbSetFunc(dbContext);

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
		void IVersioned.OnInsertingOrUpdating(DbContext dbContext) {
			var versions = CheckDbContext(dbContext);
			GetVersionDbSet(versions).AddRange(internalLocalVersions);
		}

		void IVersioned.OnDeleted(DbContext dbContext) {
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

		void IVersioned.OnInserted() => Id = Guid.NewGuid();
		void IVersioned.OnInsertedOrUpdated() => internalLocalVersions.Clear();
		#endregion

#if DEBUG
		public void
#else
		void IVersioned.
#endif
		SetSnapshotVersion(IVersion version) {
			IsReadOnly = true;
			if (version == null)
				return;
			var v = (TVersion) version;
			NotifyChangeIfNotEqual(ref value, v.Value, nameof(Value));
			Modified = v.Added;
		}

		#region PropertyChange implementations
		protected void NotifyChange<T>(ref T backingField, T newValue, PropertyChangingEventArgs propertyChangingEventArgs, PropertyChangedEventArgs propertyChangedEventArgs) {
			OnPropertyChanging(propertyChangingEventArgs);
			backingField = newValue;
			OnPropertyChanged(propertyChangedEventArgs);
		}

		protected void NotifyChangeIfNotEqual<T>(ref T backingField, T newValue, PropertyChangingEventArgs propertyChangingEventArgs, PropertyChangedEventArgs propertyChangedEventArgs) {
			if (!EqualityComparer<T>.Default.Equals(backingField, newValue))
				NotifyChange(ref backingField, newValue, propertyChangingEventArgs, propertyChangedEventArgs);
		}

		protected void NotifyChange<T>(ref T backingField, T newValue, String propertyName) {
			OnPropertyChanging(propertyName);
			backingField = newValue;
			OnPropertyChanged(propertyName);
		}

		protected void NotifyChangeIfNotEqual<T>(ref T backingField, T newValue, String propertyName) {
			if (!EqualityComparer<T>.Default.Equals(backingField, newValue))
				NotifyChange(ref backingField, newValue, propertyName);
		}

		public event PropertyChangingEventHandler PropertyChanging;
		protected void OnPropertyChanging(PropertyChangingEventArgs propertyChangingEventArgs) => PropertyChanging?.Invoke(this, propertyChangingEventArgs);
		protected void OnPropertyChanging(String propertyName) => OnPropertyChanging(new PropertyChangingEventArgs(propertyName));

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs) => PropertyChanged?.Invoke(this, propertyChangedEventArgs);
		protected void OnPropertyChanged(String propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		#endregion
	}

	internal static class TypeExtensions {
#if NET40
		public static Type GetTypeInfo(this Type type) => type;
#else
		public static Type[] GetGenericArguments(this Type ti) => ti.GenericTypeArguments;
#endif
	}

	public static class VersionedExtensions {
		public static ICollection<T> ToSnapshots<T>(this IQueryable<T> source, DbContext context, DateTime dateTime)
		where T : class {
			var vpis = EntityVersionedTypeCache<T>.VersionedTypeInfos;
			if (!vpis.Any())
				return source.ToArray();
			
			IQueryable query = source.Select(x => new Mutuple<T> { Item1 = x });
			for (int i = 0; i < vpis.Length; i++) {
				var vpi = vpis[i];
				
				//var queryableVersions = genericTypes.Versioned.GetMethod()
				//query = query.ApplyGroupJoin(vpi, genericTypes);
			}

			// take Mutable<T, ...all the VPs of T...> and call SetSnapshotVersion(...) on all the VPs
			IQueryable<T> result = null;

			return result.ToArray();
		}

#if !NET40
		public static async Task<ICollection<T>> ToSnapshotsAsync<T>(this IQueryable<T> source, DbContext context, DateTime dateTime)
		where T : class {
			var vpis = EntityVersionedTypeCache<T>.VersionedProperties;
			if (!vpis.Any())
				return await source.ToArrayAsync();
			IQueryable query = source.Select(x => new Mutuple<T> { Item1 = x });
			return await ((IQueryable<T>) query).ToArrayAsync();
		}
#endif

		private static class EntityVersionedTypeCache<T>
		where T : class {
			public static readonly PropertyInfo[] VersionedProperties = typeof(T).GetProperties().Where(x => typeof(IVersioned).IsAssignableFrom(x.PropertyType)).ToArray();
			//public static readonly GenericTypes[] GenericTypes = Vpis.Select(GetGenericTypes).ToArray();
			public static readonly VersionedTypeInfo[] VersionedTypeInfos = VersionedProperties.Select((x,i) => new VersionedTypeInfo(x, GetGenericTypes(x), i)).ToArray();

			public class VersionedTypeInfo {
				public VersionedTypeInfo(PropertyInfo propertyInfo, GenericTypes genericTypes, Int32 vpIndex) {
					PropertyInfo = propertyInfo;
					GenericTypes = genericTypes;
					VpIndex = vpIndex;
					MutupleType = typeof(Mutuple<>).GetTypeInfo().Assembly.GetTypes().Single(x => x.GetGenericArguments().Length == VpIndex + 2);
				}

				public PropertyInfo PropertyInfo { get; }
				public GenericTypes GenericTypes { get; }
				public Int32        VpIndex      { get; }
				public Type         MutupleType  { get; }
			}
		}

		private static class VersionedBaseTypeCache<TVersioned, TValue, TVersion, TIVersions>
		where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>
		where TVersion : VersionBase<TValue>, new()
		where TIVersions : class {
			//public static readonly MethodInfo GetVersionDbSetStaticMethodInfo
			public static readonly Func<TIVersions, DbSet<TVersion>> GetVersionDbSetFunc = VersionedBase<TVersioned, TValue, TVersion, TIVersions>.GetVersionDbSet;


			public static void What(TVersioned versioned) {
				//var queryable = versioned.GetSnap
			}
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

		private static GenericTypes GetGenericTypes(PropertyInfo x) => new GenericTypes(GetVersionedBaseType(x.PropertyType).GetGenericArguments());

		private static Type GetVersionedBaseType(Type type) {
			while (!type.GetTypeInfo().IsGenericType || type.GetGenericTypeDefinition() != typeof(VersionedBase<,,,>))
				type = type.GetTypeInfo().BaseType;
			return type;
		}
	}
}