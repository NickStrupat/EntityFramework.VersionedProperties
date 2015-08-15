using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using EntityFramework.Extensions;

namespace EntityFramework.VersionedProperties {
	public abstract class VersionedBase<TValue, TVersion, TIVersions> : IVersioned where TVersion : VersionBase<TValue>, new() {
		public Guid Id { get; private set; }
		public DateTime Modified { get; private set; }
		private TValue value;
		public virtual TValue Value {
			get { return value; }
			set {
				if (value == null && !(this is NullableVersionedBase<TValue, TVersion, TIVersions>))
					throw new ArgumentNullException(nameof(value));
				if (EqualityComparer<TValue>.Default.Equals(this.value, value))
					return;
				Modified = DateTime.Now;
				if (Id == Guid.Empty)
					Id = Guid.NewGuid();
				else
					InternalLocalVersions.Add(new TVersion { VersionedId = Id, Added = Modified, Value = Value });
				this.value = value;
			}
		}

		public override String ToString() => Value.ToString();

		protected virtual TValue DefaultValue => default(TValue);

		protected VersionedBase() {
			Modified = DateTime.Now;
			if (DefaultValue != null)
				value = DefaultValue;
		}

		protected abstract Func<TIVersions, DbSet<TVersion>> VersionDbSet { get; }

		internal readonly List<TVersion> InternalLocalVersions = new List<TVersion>();

		public IEnumerable<TVersion> LocalVersions => InternalLocalVersions;

		public IOrderedQueryable<TVersion> Versions(TIVersions dbContext) => VersionDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);

		void IVersioned.AddVersionsToDbContext(Object dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions)dbContext).AddRange(InternalLocalVersions);
			InternalLocalVersions.Clear();
		}

		void IVersioned.RemoveVersionsFromDbContext(Object dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions)dbContext).Where(x => x.VersionedId == Id).Delete();
			InternalLocalVersions.Clear();
		}

		private static void CheckDbContext(Object dbContext) {
			if (!(dbContext is TIVersions))
				throw new InvalidOperationException("Your DbContext class must implement " + typeof(TIVersions).Name);
		}
	}
}