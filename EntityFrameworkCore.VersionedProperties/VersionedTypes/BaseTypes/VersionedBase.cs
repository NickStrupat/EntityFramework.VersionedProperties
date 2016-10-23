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
		public Guid Id { get; private set; }
		public DateTime Modified { get; private set; }
		private TValue value;

		internal virtual Boolean ValueCanBeNull { get; } = false;
		private static readonly Boolean TValueIsValueType = typeof(TValue).GetTypeInfo().IsValueType;

		internal Boolean isDefaultValue;
		void IVersioned.SetIsDefaultValueFalse() => isDefaultValue = false;

		public virtual TValue Value {
			get { return value; }
			set {
				if (!TValueIsValueType && !ValueCanBeNull && value == null)
					throw new ArgumentNullException(nameof(value), "Value cannot be assigned null when ValueCanBeNull is false");

				if (isDefaultValue)
					isDefaultValue = false;
				else {
					if (EqualityComparer<TValue>.Default.Equals(this.value, value))
						return;
					if (Id == Guid.Empty)
						Id = Guid.NewGuid();
					AddToInternalLocalVersions();
				}
				Modified = DateTime.Now;
				this.value = value;
			}
		}
		
		public override String ToString() => Value == null ? String.Empty : Value.ToString();

		protected virtual TValue DefaultValue => TValueIsValueType || ValueCanBeNull ? default(TValue) : Activator.CreateInstance<TValue>();

		protected VersionedBase() {
			Modified = DateTime.Now;
			value = DefaultValue;
			isDefaultValue = true;
		}

		protected abstract Func<TIVersions, DbSet<TVersion>> VersionDbSet { get; }

		private readonly List<TVersion> internalLocalVersions = new List<TVersion>();

		private void AddToInternalLocalVersions() {
			var version = Activator.CreateInstance<TVersion>();
			version.VersionedId = Id;
			version.Added = Modified;
			version.Value = Value;
			internalLocalVersions.Add(version);
		}

		public IEnumerable<TVersion> LocalVersions => internalLocalVersions;

		public IOrderedQueryable<TVersion> Versions(TIVersions dbContext) => VersionDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);

		void IVersioned.AddVersionsToDbContext(DbContext dbContext) {
			CheckDbContext(dbContext);
			VersionDbSet((TIVersions)(Object)dbContext).AddRange(internalLocalVersions);
			internalLocalVersions.Clear();
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
	}
}