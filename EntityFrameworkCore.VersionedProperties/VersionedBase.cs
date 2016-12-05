using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
	public abstract class VersionedBase<TVersioned, TValue, TVersion, TIVersions> : IVersioned, INotifyPropertyChanging, INotifyPropertyChanged, IEquatable<TVersioned>
	where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>, new()
	where TVersion : VersionBase<TValue>, new()
	where TIVersions : class {
		protected VersionedBase() {
			id = Guid.Empty;
			modified = default(DateTime?);
			isReadOnly = false;
			value = DefaultValue;
			internalLocalVersions = new ObservableCollection<TVersion>();
			LocalVersions = new ReadOnlyObservableCollection<TVersion>(internalLocalVersions);
		}

		internal TVersioned Clone() {
			return new TVersioned {
				id = id,
				modified = modified,
				isReadOnly = isReadOnly,
				value = value,
				internalLocalVersions = new ObservableCollection<TVersion>(internalLocalVersions),
				LocalVersions = new ReadOnlyObservableCollection<TVersion>(internalLocalVersions)
			};
		}

		public override String ToString() => Value?.ToString() ?? String.Empty;

		#region Equality
		public override Boolean Equals(Object versioned) => Equals(versioned as TVersioned);

		public Boolean Equals(TVersioned versioned) {
			if (ReferenceEquals(versioned, null))
				return false;
			if (ReferenceEquals(versioned, this))
				return true;
			return Id == versioned.Id
			    && Modified == versioned.Modified
			    && IsReadOnly == versioned.IsReadOnly
			    && ValueEqualityComparer.Equals(Value, versioned.Value)
			    && (ReferenceEquals(LocalVersions, versioned.LocalVersions) || LocalVersions.SequenceEqual(versioned.LocalVersions));
		}

		public override Int32 GetHashCode() {
			var hash = 1374496523;
			unchecked {
				hash = (hash * -1521134295) + Id.GetHashCode();
				hash = (hash * -1521134295) + Modified.GetHashCode();
				hash = (hash * -1521134295) + Id.GetHashCode();
				if (IsTValueAValueType || !ReferenceEquals(Value, null))
					hash = (hash * -1521134295) + ValueEqualityComparer.GetHashCode(Value);
			}
			return hash;
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Boolean IsTValueAValueType = typeof(TValue).GetTypeInfo().IsValueType;

		protected virtual IEqualityComparer<TValue> ValueEqualityComparer => EqualityComparer<TValue>.Default;
		#endregion
		#region Id
		/// <summary>Gets the unique identifier for this versioned property</summary>
		public Guid Id {
			get { return id; }
			internal set { NotifyChangeIfNotEqual(ref id, value, idPropertyChangingEventArgs, idPropertyChangedEventArgs); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private Guid id;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static readonly PropertyChangingEventArgs idPropertyChangingEventArgs = new PropertyChangingEventArgs(nameof(Id));
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static readonly PropertyChangedEventArgs idPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Id));
		#endregion
		#region Modified
		/// <summary>Gets the date-time representing when this versioned property was last modified</summary>
		public DateTime? Modified {
			get { return modified; }
			internal set { NotifyChangeIfNotEqual(ref modified, value, modifiedPropertyChangingEventArgs, modifiedPropertyChangedEventArgs); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private DateTime? modified;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static readonly PropertyChangingEventArgs modifiedPropertyChangingEventArgs = new PropertyChangingEventArgs(nameof(Modified));
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static readonly PropertyChangedEventArgs modifiedPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Modified));
		#endregion
		#region IsReadOnly
		/// <summary>Gets a boolean indicating the read-only state of <see cref="Value"/></summary>
		[NotMapped]
		public Boolean IsReadOnly {
			get { return isReadOnly; }
			private set { NotifyChangeIfNotEqual(ref isReadOnly, value, isReadOnlyPropertyChangingEventArgs, isReadOnlyPropertyChangedEventArgs); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private Boolean isReadOnly;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static readonly PropertyChangingEventArgs isReadOnlyPropertyChangingEventArgs = new PropertyChangingEventArgs(nameof(IsReadOnly));
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static readonly PropertyChangedEventArgs isReadOnlyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsReadOnly));
		#endregion
		#region Value
		/// <summary>Gets or sets the value of this versioned property (the previous value is pushed into <see cref="LocalVersions"/>, then into the versions set for <see cref="TVersion"/> in <see cref="TIVersions"/> once <see cref="DbContextWithTriggers.SaveChanges"/> is called)</summary>
		public TValue Value {
			get { return value; }
			set {
				if (IsReadOnly)
					throw new InvalidOperationException("This object is in a read-only state, possibly because it is a snapshot of a previous state");
				if (Modified.HasValue) {
					if (ValueEqualityComparer.Equals(this.value, value))
						return;
					if (Id == Guid.Empty)
						Id = Guid.NewGuid();
					internalLocalVersions.Add(new TVersion {
						VersionedId = Id,
						Added = Modified.Value,
						Value = Value
					});
				}
				NotifyChange(ref this.value, value, valuePropertyChangingEventArgs, valuePropertyChangedEventArgs);
				Modified = DateTime.UtcNow;
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private TValue value;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static readonly PropertyChangingEventArgs valuePropertyChangingEventArgs = new PropertyChangingEventArgs(nameof(Value));
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static readonly PropertyChangedEventArgs valuePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
		#endregion
		#region LocalVersions
		/// <summary>Gets the local versions (not yet persisted)</summary>
		[NotMapped]
		public ReadOnlyObservableCollection<TVersion> LocalVersions { get; private set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ObservableCollection<TVersion> internalLocalVersions;
		#endregion
		#region GetVersions
		/// <summary>Gets the previous versions</summary>
		public IOrderedQueryable<TVersion> GetVersions(TIVersions dbContext) => GetVersionDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);

		public static DbSet<TVersion> GetVersionDbSet(TIVersions dbContext) => GetVersionDbSetFunc(dbContext);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Func<TIVersions, DbSet<TVersion>> GetVersionDbSetFunc = typeof(TIVersions).GetProperties()
		                                                                                                  .Single(x => x.PropertyType == typeof(DbSet<TVersion>))
		                                                                                                  .GetPropertyGetter<TIVersions, DbSet<TVersion>>();
		#endregion
		
		/// <summary>Gets the overridable default value which is assigned to the <see cref="Value"/> property on construction</summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected virtual TValue DefaultValue => default(TValue);
		
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
		
		void IVersioned.OnInsertedOrUpdated() => internalLocalVersions.Clear();

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
			NotifyChangeIfNotEqual(ref value, v.Value, valuePropertyChangingEventArgs, valuePropertyChangedEventArgs);
			Modified = v.Added;
		}
		#endregion
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
		/// <summary>Sets the value of a versioned property (use this to set virtual VersionedX values so that change tracking is efficient)</summary>
		public static void Set<TEntity, TVersioned, TValue, TVersion, TIVersions>(
			this TEntity entity,
			Expression<Func<TEntity, VersionedBase<TVersioned, TValue, TVersion, TIVersions>>> versionedProperty,
			TValue newValue
			)
		where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>, new()
		where TVersion : VersionBase<TValue>, new()
		where TIVersions : class
		where TEntity : class {
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));
			if (versionedProperty == null)
				throw new ArgumentNullException(nameof(versionedProperty));

			var propertyInfo = (versionedProperty.Body as MemberExpression)?.Member as PropertyInfo;
			if (propertyInfo == null || propertyInfo.DeclaringType != typeof(TEntity))
				throw new ArgumentException($"Must be a property of type {typeof(TVersioned).FullName} with declaring type {typeof(TEntity).FullName}", nameof(versionedProperty));

			var accessors = VersionedSetterCache<TEntity, TVersioned>.GetAccessors(propertyInfo);
			var versioned = accessors.Getter(entity);
			accessors.Setter(entity, versioned.Clone());
		}

		private static class VersionedSetterCache<TEntity, TVersioned> {
			private static readonly ConcurrentDictionary<PropertyInfo, Accessors> cache = new ConcurrentDictionary<PropertyInfo, Accessors>();
			public static Accessors GetAccessors(PropertyInfo propertyInfo) => cache.GetOrAdd(propertyInfo, ValueFactory);

			private static Accessors ValueFactory(PropertyInfo propertyInfo) {
				return new Accessors {
					Getter = propertyInfo.GetPropertyGetter<TEntity, TVersioned>(),
					Setter = propertyInfo.GetPropertySetter<TEntity, TVersioned>()
				};
			}

			public struct Accessors {
				public Func<TEntity, TVersioned> Getter;
				public Action<TEntity, TVersioned> Setter;
			}
			
		}

		/// <summary>Returns a collection of entities with their versioned properties as they were at the specified date-time</summary>
		/// <remarks>Your source query is ammended with group joins, so that only one command is sent the to persistence layer</remarks>
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
			public static readonly VersionedTypeInfo[] VersionedTypeInfos = VersionedProperties.Select((x, i) => new VersionedTypeInfo(x, GenericTypes.GetGenericTypes(x), i)).ToArray();

			public class VersionedTypeInfo {
				public VersionedTypeInfo(PropertyInfo propertyInfo, GenericTypes genericTypes, Int32 vpIndex) {
					PropertyInfo = propertyInfo;
					GenericTypes = genericTypes;
					VpIndex = vpIndex;
					MutupleType = typeof(Mutuple<>).GetTypeInfo().Assembly.GetTypes().Single(x => x.GetGenericArguments().Length == VpIndex + 2);
				}

				public PropertyInfo PropertyInfo { get; }
				public GenericTypes GenericTypes { get; }
				public Int32 VpIndex { get; }
				public Type MutupleType { get; }
			}

			public struct GenericTypes {
				public Type Versioned;
				public Type Value;
				public Type Version;
				public Type IVersions;

				public GenericTypes(Type[] genericArguments) : this(genericArguments[0], genericArguments[1], genericArguments[2], genericArguments[3]) { }
				public GenericTypes(Type versioned, Type value, Type version, Type versions) {
					Versioned = versioned;
					Value = value;
					Version = version;
					IVersions = versions;
				}

				public static GenericTypes GetGenericTypes(PropertyInfo x) => new GenericTypes(GetVersionedBaseType(x.PropertyType).GetGenericArguments());

				private static Type GetVersionedBaseType(Type type) {
					while (!type.GetTypeInfo().IsGenericType || type.GetGenericTypeDefinition() != typeof(VersionedBase<,,,>))
						type = type.GetTypeInfo().BaseType;
					return type;
				}
			}
		}

		private static class VersionedBaseTypeCache<TVersioned, TValue, TVersion, TIVersions>
		where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>, new()
		where TVersion : VersionBase<TValue>, new()
		where TIVersions : class {
			//public static readonly MethodInfo GetVersionDbSetStaticMethodInfo
			public static readonly Func<TIVersions, DbSet<TVersion>> GetVersionDbSetFunc = VersionedBase<TVersioned, TValue, TVersion, TIVersions>.GetVersionDbSet;

			public static void What(TVersioned versioned) {
				//var queryable = versioned.GetSnap
			}
		}
	}
}