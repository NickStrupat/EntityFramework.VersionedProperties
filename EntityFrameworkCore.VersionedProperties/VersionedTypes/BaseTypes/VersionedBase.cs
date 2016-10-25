using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	public abstract class VersionedBase<TValue, TVersion, TIVersions> : IVersioned where TVersion : VersionBase<TValue>, new() {
		public Guid Id { get; private set; } = Guid.Empty;
		public DateTime Modified { get; private set; } = DateTime.UtcNow;

		private Boolean isDefaultValue = true;
		private readonly List<TVersion> internalLocalVersions = new List<TVersion>();
		
		private TValue value;
		public TValue Value {
			get { return value; }
			set {
				if (isDefaultValue)
					isDefaultValue = false;
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
		protected abstract Func<TIVersions, DbSet<TVersion>> VersionDbSet { get; }
		
		public override String ToString() => Value == null ? String.Empty : Value.ToString();
		public IEnumerable<TVersion> LocalVersions => internalLocalVersions;
		public IOrderedQueryable<TVersion> Versions(TIVersions dbContext) => VersionDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);

		#region IVersioned implementations
		void IVersioned.AddVersionsToDbContext(DbContext dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions)(Object)dbContext).AddRange(internalLocalVersions);
		}

		void IVersioned.RemoveVersionsFromDbContext(DbContext dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions)(Object)dbContext).Where(x => x.VersionedId == Id).Delete();
			internalLocalVersions.Clear();
		}

		private static void CheckDbContext(DbContext dbContext) {
			if (dbContext is TIVersions)
				return;
			throw new InvalidOperationException("Your DbContext class must implement " + typeof(TIVersions).Name);
		}

		void IVersioned.SetIsDefaultValueFalse() => isDefaultValue = false;
		void IVersioned.ClearInternalLocalVersions() => internalLocalVersions.Clear();
		#endregion
	}
}