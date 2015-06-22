using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFramework.Extensions;

namespace EntityFramework.VersionedProperties {
	public abstract class VersionedBase<TValue, TVersion, TIVersions> : Versioned
		where TVersion : VersionBase<TValue>, new()
	{
		public Guid Id { get; internal set; }
		public DateTime Modified { get; internal set; }
		private TValue value;
		public virtual TValue Value {
			get { return value; }
			set {
				if (!(this is NullableVersionedBase<TValue, TVersion, TIVersions>) && value == null)
					throw new ArgumentNullException("value");
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
		public sealed override String ToString() {
			return Value.ToString();
		}
		protected virtual TValue DefaultValue {
			get { return default(TValue); }
		}
		protected abstract Func<TIVersions, DbSet<TVersion>> VersionDbSet { get; }
		protected VersionedBase() {
			Modified = DateTime.Now;
			if (DefaultValue != null)
				value = DefaultValue;
			AddVersionsToDbContextWithVersionedProperties = AddVersions;
			RemoveVersionsFromDbContextWithVersionedProperties = RemoveVersions;
		}
		private void RemoveVersions(Object dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions) dbContext).Where(x => x.VersionedId == Id).Delete();
			InternalLocalVersions.Clear();
		}
		private void AddVersions(Object dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions) dbContext).AddRange(InternalLocalVersions);
			InternalLocalVersions.Clear();
		}
		private static void CheckDbContext(Object dbContext) {
			if (!(dbContext is TIVersions))
				throw new InvalidOperationException("Your DbContext class must implement " + typeof (TIVersions).Name);
		}
		internal readonly List<TVersion> InternalLocalVersions = new List<TVersion>();
		public IEnumerable<TVersion> LocalVersions {
			get { return InternalLocalVersions; }
		}
		public IOrderedQueryable<TVersion> Versions(TIVersions dbContext) {
			return VersionDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);
		}
	}
}