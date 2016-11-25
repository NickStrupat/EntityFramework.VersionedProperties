using System;
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;

#if EF_CORE
using System.Reflection;
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	public abstract class VersionedBase<TVersioned, TValue, TVersion, TIVersions> : IVersioned
	where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>
	where TVersion : VersionBase<TValue>, new()
	where TIVersions : class {
		/// <summary>Gets the unique identifier for this versioned property</summary>
		public Guid Id { get; private set; } = Guid.Empty;
		
		/// <summary>Gets the date-time representing when this versioned property was last modified</summary>
		public DateTime Modified { get; private set; } = DateTime.UtcNow;

		private Boolean isInitialValue = true;

		/// <summary>Gets the local versions (not yet persisted)</summary>
		public IEnumerable<TVersion> LocalVersions => internalLocalVersions;
		private readonly List<TVersion> internalLocalVersions = new List<TVersion>();

		/// <summary>Gets a boolean indicating the read-only state of <see cref="Value"/></summary>
		public Boolean IsReadOnly { get; internal set; }
		
		private TValue value;
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

		protected virtual TValue DefaultValue => default(TValue);
		protected virtual DbSet<TVersion> GetVersionDbSet(TIVersions x) => getVersionDbSetFunc(x);
		protected static readonly Func<TIVersions, DbSet<TVersion>> getVersionDbSetFunc = typeof(TIVersions)
#if NET40
		                                                                                             .GetTypeInfo()
#endif
		                                                                                             .GetProperties()
		                                                                                             .Single(x => x.PropertyType == typeof(DbSet<TVersion>))
		                                                                                             .GetPropertyGetter<TIVersions, DbSet<TVersion>>();

		public override String ToString() => Value?.ToString() ?? String.Empty;
		/// <summary>Gets the previous versions</summary>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public IOrderedQueryable<TVersion> GetVersions(TIVersions dbContext) => GetVersionDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);

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

	public static class VersionedExtensions {
		public static IQueryable<T> SelectSnapshots<T>(this IQueryable<T> source, DateTime dateTime)
		where T : class {
			return null;
		}
	}
}