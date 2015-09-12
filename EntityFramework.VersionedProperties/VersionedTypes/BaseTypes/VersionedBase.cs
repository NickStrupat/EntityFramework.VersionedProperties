using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;

namespace EntityFramework.VersionedProperties {
	public abstract class VersionedBase<TValue, TVersion, TIVersions> : IVersioned where TVersion : VersionBase<TValue>, new() {
		public Guid Id { get; private set; }
		public DateTime Modified { get; private set; }
		private TValue value;
		internal virtual Boolean ValueCanBeNull { get; } = false;
		private static Boolean TValueIsValueType = typeof(TValue).IsValueType;
		public virtual TValue Value {
			get { return value; }
			set {
				if (!TValueIsValueType && !ValueCanBeNull && value == null)
					throw new ArgumentNullException(nameof(value));
				if (Id == Guid.Empty)
					Id = Guid.NewGuid();
				else {
					if (EqualityComparer<TValue>.Default.Equals(this.value, value))
						return;
					AddToInternalLocalVersions();
				}
				Modified = DateTime.Now;
				this.value = value;
			}
		}

		private static readonly Func<TVersion> VersionCtor = Expression.Lambda<Func<TVersion>>(Expression.New(typeof (TVersion))).Compile(); // C# 6 and older generate Activator.CreateInstance calls for `new T()`
		
		public override String ToString() => Value.ToString();

		protected virtual TValue DefaultValue => default(TValue);

		protected VersionedBase() {
			Modified = DateTime.Now;
			value = DefaultValue;
		}

		protected abstract Func<TIVersions, DbSet<TVersion>> VersionDbSet { get; }

		private readonly List<TVersion> InternalLocalVersions = new List<TVersion>();

		private void AddToInternalLocalVersions() {
			var version = VersionCtor();
			version.VersionedId = Id;
			version.Added = Modified;
			version.Value = Value;
			InternalLocalVersions.Add(version);
		}

		public IEnumerable<TVersion> LocalVersions => InternalLocalVersions;

		public IOrderedQueryable<TVersion> Versions(TIVersions dbContext) => VersionDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);

		void IVersioned.AddVersionsToDbContext(DbContext dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions)(Object)dbContext).AddRange(InternalLocalVersions);
			InternalLocalVersions.Clear();
		}

		void IVersioned.RemoveVersionsFromDbContext(DbContext dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions)(Object)dbContext).Where(x => x.VersionedId == Id).Delete();
			InternalLocalVersions.Clear();
		}

		private static void CheckDbContext(DbContext dbContext) {
			if (dbContext is TIVersions)
				return;
			throw new InvalidOperationException("Your DbContext class must implement " + typeof(TIVersions).Name);
		}
	}
}