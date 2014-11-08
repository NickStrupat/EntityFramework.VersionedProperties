using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using EntityFramework.Extensions;

namespace EntityFramework.VersionedProperties {
	public abstract class Versioned {
		internal Action<IDbContextWithVersionedProperties> AddVersionsToDbContextWithVersionedProperties { get; set; }
		internal Action<IDbContextWithVersionedProperties> DeleteVersionsFromDbContextWithVersionedProperties { get; set; }
	}
	public abstract class VersionedBase<TValue, TVersion> : Versioned
		where TVersion : VersionBase<TValue>, new()
	{
		public Guid Id { get; internal set; }
		public DateTime Modified { get; internal set; }
		private TValue value;
		public virtual TValue Value {
			get { return value; }
			set {
				if (!(this is NullableVersionedBase<TValue, TVersion>) && value == null)
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
		protected abstract Func<IDbContextWithVersionedProperties, DbSet<TVersion>> VersionsDbSet { get; }
		protected VersionedBase() {
			Modified = DateTime.Now;
			value = DefaultValue;
			AddVersionsToDbContextWithVersionedProperties = dbContext => {
																VersionsDbSet(dbContext).AddRange(InternalLocalVersions);
																InternalLocalVersions.Clear();
			                                                };
			DeleteVersionsFromDbContextWithVersionedProperties = dbContext => {
				                                                     VersionsDbSet(dbContext).Where(x => x.VersionedId == Id).Delete();
																	 InternalLocalVersions.Clear();
			                                                     };
		}
		internal readonly List<TVersion> InternalLocalVersions = new List<TVersion>();
		public IEnumerable<TVersion> LocalVersions {
			get { return InternalLocalVersions; }
		}
		public IQueryable<TVersion> Versions(IDbContextWithVersionedProperties dbContext) {
			return VersionsDbSet(dbContext).Where(x => x.VersionedId == Id).OrderByDescending(x => x.Added);
		}
	}

	public abstract class NullableVersionedBase<T, TVersion> : VersionedBase<T, TVersion> where TVersion : VersionBase<T>, new() {}

	public abstract class RequiredValueVersionedBase<TValue, TVersion> : VersionedBase<TValue, TVersion> where TVersion : VersionBase<TValue>, new() {
		[Required]
		public override TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}

	[ComplexType]
	public class VersionedString : RequiredValueVersionedBase<String, StringVersion> {
		protected override String DefaultValue {
			get { return String.Empty; }
		}
		protected override Func<IDbContextWithVersionedProperties, DbSet<StringVersion>> VersionsDbSet {
			get { return x => x.StringVersions; }
		}
	}

	[ComplexType]
	public class VersionedDbGeography : RequiredValueVersionedBase<DbGeography, DbGeographyVersion> {
		protected override DbGeography DefaultValue {
			get { return DbGeography.FromText("POINT EMPTY"); }
		}
		protected override Func<IDbContextWithVersionedProperties, DbSet<DbGeographyVersion>> VersionsDbSet {
			get { return x => x.DbGeographyVersions; }
		}
	}

	[ComplexType]
	public class VersionedDbGeometry : RequiredValueVersionedBase<DbGeometry, DbGeometryVersion> {
		protected override DbGeometry DefaultValue {
			get { return DbGeometry.FromText("POINT EMPTY"); }
		}
		protected override Func<IDbContextWithVersionedProperties, DbSet<DbGeometryVersion>> VersionsDbSet {
			get { return x => x.DbGeometryVersions; }
		}
	}

	[ComplexType]
	public class VersionedBoolean : VersionedBase<Boolean, BooleanVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<BooleanVersion>> VersionsDbSet {
			get { return x => x.BooleanVersions; }
		}
	}
	[ComplexType]
	public class VersionedDateTime : VersionedBase<DateTime, DateTimeVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<DateTimeVersion>> VersionsDbSet {
			get { return x => x.DateTimeVersions; }
		}
	}
	[ComplexType]
	public class VersionedDateTimeOffset : VersionedBase<DateTimeOffset, DateTimeOffsetVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<DateTimeOffsetVersion>> VersionsDbSet {
			get { return x => x.DateTimeOffsetVersions; }
		}
	}
	[ComplexType]
	public class VersionedDecimal : VersionedBase<Decimal, DecimalVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<DecimalVersion>> VersionsDbSet {
			get { return x => x.DecimalVersions; }
		}
	}
	[ComplexType]
	public class VersionedDouble : VersionedBase<Double, DoubleVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<DoubleVersion>> VersionsDbSet {
			get { return x => x.DoubleVersions; }
		}
	}
	[ComplexType]
	public class VersionedGuid : VersionedBase<Guid, GuidVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<GuidVersion>> VersionsDbSet {
			get { return x => x.GuidVersions; }
		}
	}
	[ComplexType]
	public class VersionedInt32 : VersionedBase<Int32, Int32Version> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<Int32Version>> VersionsDbSet {
			get { return x => x.Int32Versions; }
		}
	}
	[ComplexType]
	public class VersionedInt64 : VersionedBase<Int64, Int64Version> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<Int64Version>> VersionsDbSet {
			get { return x => x.Int64Versions; }
		}
	}

	[ComplexType]
	public class VersionedNullableBoolean : NullableVersionedBase<Boolean?, NullableBooleanVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableBooleanVersion>> VersionsDbSet {
			get { return x => x.NullableBooleanVersions; }
		}
	}
	[ComplexType]
	public class VersionedNullableDateTime : NullableVersionedBase<DateTime?, NullableDateTimeVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableDateTimeVersion>> VersionsDbSet {
			get { return x => x.NullableDateTimeVersions; }
		}
	}
	[ComplexType]
	public class VersionedNullableDateTimeOffset : NullableVersionedBase<DateTimeOffset?, NullableDateTimeOffsetVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableDateTimeOffsetVersion>> VersionsDbSet {
			get { return x => x.NullableDateTimeOffsetVersions; }
		}
	}
	[ComplexType]
	public class VersionedNullableDbGeography : NullableVersionedBase<DbGeography, NullableDbGeographyVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableDbGeographyVersion>> VersionsDbSet {
			get { return x => x.NullableDbGeographyVersions; }
		}
	}
	[ComplexType]
	public class VersionedNullableDbGeometry : NullableVersionedBase<DbGeometry, NullableDbGeometryVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableDbGeometryVersion>> VersionsDbSet {
			get { return x => x.NullableDbGeometryVersions; }
		}
	}
	[ComplexType]
	public class VersionedNullableDecimal : NullableVersionedBase<Decimal?, NullableDecimalVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableDecimalVersion>> VersionsDbSet {
			get { return x => x.NullableDecimalVersions; }
		}
	}
	[ComplexType]
	public class VersionedNullableDouble : NullableVersionedBase<Double?, NullableDoubleVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableDoubleVersion>> VersionsDbSet {
			get { return x => x.NullableDoubleVersions; }
		}
	}
	[ComplexType]
	public class VersionedNullableGuid : NullableVersionedBase<Guid?, NullableGuidVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableGuidVersion>> VersionsDbSet {
			get { return x => x.NullableGuidVersions; }
		}
	}
	[ComplexType]
	public class VersionedNullableInt32 : NullableVersionedBase<Int32?, NullableInt32Version> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableInt32Version>> VersionsDbSet {
			get { return x => x.NullableInt32Versions; }
		}
	}
	[ComplexType]
	public class VersionedNullableInt64 : NullableVersionedBase<Int64?, NullableInt64Version> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableInt64Version>> VersionsDbSet {
			get { return x => x.NullableInt64Versions; }
		}
	}
	[ComplexType]
	public class VersionedNullableString : NullableVersionedBase<String, NullableStringVersion> {
		protected override Func<IDbContextWithVersionedProperties, DbSet<NullableStringVersion>> VersionsDbSet {
			get { return x => x.NullableStringVersions; }
		}
	}
}