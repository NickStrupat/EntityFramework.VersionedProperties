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
		private static readonly Boolean TValueIsValueType = typeof(TValue).GetTypeInfo().IsValueType;

		public Guid Id { get; private set; }
		public DateTime Modified { get; private set; }

		private readonly List<TVersion> internalLocalVersions = new List<TVersion>();

		internal virtual Boolean ValueCanBeNull => false;
		internal Boolean isDefaultValue;
		
		private TValue value;
		public virtual TValue Value {
			get { return value; }
			set {
				if (!TValueIsValueType && !ValueCanBeNull && value == null)
					throw new ArgumentNullException(nameof(value), $"{nameof(Value)} cannot be assigned null on this type");

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
				Modified = DateTime.Now;
				this.value = value;
			}
		}

		protected VersionedBase() {
			Modified = DateTime.Now;
			value = DefaultValue;
			isDefaultValue = true;
		}

		protected virtual TValue DefaultValue => TValueIsValueType || ValueCanBeNull ? default(TValue) : Activator.CreateInstance<TValue>();
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