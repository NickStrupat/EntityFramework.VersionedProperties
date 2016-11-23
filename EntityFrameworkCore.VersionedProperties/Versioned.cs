using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	#region Primitives
	[ComplexType]
	public sealed class VersionedBoolean : VersionedBase<Boolean, BooleanVersion, IBooleanVersions> {
		protected override DbSet<BooleanVersion> VersionDbSet(IBooleanVersions x) => x.BooleanVersions;
	}
	[ComplexType]
	public sealed class VersionedDateTime : VersionedBase<DateTime, DateTimeVersion, IDateTimeVersions> {
		protected override DbSet<DateTimeVersion> VersionDbSet(IDateTimeVersions x) => x.DateTimeVersions;
	}
	[ComplexType]
	public sealed class VersionedDateTimeOffset : VersionedBase<DateTimeOffset, DateTimeOffsetVersion, IDateTimeOffsetVersions> {
		protected override DbSet<DateTimeOffsetVersion> VersionDbSet(IDateTimeOffsetVersions x) => x.DateTimeOffsetVersions;
	}
	[ComplexType]
	public sealed class VersionedDecimal : VersionedBase<Decimal, DecimalVersion, IDecimalVersions> {
		protected override DbSet<DecimalVersion> VersionDbSet(IDecimalVersions x) => x.DecimalVersions;
	}
	[ComplexType]
	public sealed class VersionedDouble : VersionedBase<Double, DoubleVersion, IDoubleVersions> {
		protected override DbSet<DoubleVersion> VersionDbSet(IDoubleVersions x) => x.DoubleVersions;
	}
	[ComplexType]
	public sealed class VersionedGuid : VersionedBase<Guid, GuidVersion, IGuidVersions> {
		protected override DbSet<GuidVersion> VersionDbSet(IGuidVersions x) => x.GuidVersions;
	}
	[ComplexType]
	public sealed class VersionedInt32 : VersionedBase<Int32, Int32Version, IInt32Versions> {
		protected override DbSet<Int32Version> VersionDbSet(IInt32Versions x) => x.Int32Versions;
	}
	[ComplexType]
	public sealed class VersionedInt64 : VersionedBase<Int64, Int64Version, IInt64Versions> {
		protected override DbSet<Int64Version> VersionDbSet(IInt64Versions x) => x.Int64Versions;
	}
	#endregion
	#region Nullable primitives
	[ComplexType]
	public sealed class VersionedNullableBoolean : VersionedBase<Boolean?, NullableBooleanVersion, INullableBooleanVersions> {
		protected override DbSet<NullableBooleanVersion> VersionDbSet(INullableBooleanVersions x) => x.NullableBooleanVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableDateTime : VersionedBase<DateTime?, NullableDateTimeVersion, INullableDateTimeVersions> {
		protected override DbSet<NullableDateTimeVersion> VersionDbSet(INullableDateTimeVersions x) => x.NullableDateTimeVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableDateTimeOffset : VersionedBase<DateTimeOffset?, NullableDateTimeOffsetVersion, INullableDateTimeOffsetVersions> {
		protected override DbSet<NullableDateTimeOffsetVersion> VersionDbSet(INullableDateTimeOffsetVersions x) => x.NullableDateTimeOffsetVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableDecimal : VersionedBase<Decimal?, NullableDecimalVersion, INullableDecimalVersions> {
		protected override DbSet<NullableDecimalVersion> VersionDbSet(INullableDecimalVersions x) => x.NullableDecimalVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableDouble : VersionedBase<Double?, NullableDoubleVersion, INullableDoubleVersions> {
		protected override DbSet<NullableDoubleVersion> VersionDbSet(INullableDoubleVersions x) => x.NullableDoubleVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableGuid : VersionedBase<Guid?, NullableGuidVersion, INullableGuidVersions> {
		protected override DbSet<NullableGuidVersion> VersionDbSet(INullableGuidVersions x) => x.NullableGuidVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableInt32 : VersionedBase<Int32?, NullableInt32Version, INullableInt32Versions> {
		protected override DbSet<NullableInt32Version> VersionDbSet(INullableInt32Versions x) => x.NullableInt32Versions;
	}
	[ComplexType]
	public sealed class VersionedNullableInt64 : VersionedBase<Int64?, NullableInt64Version, INullableInt64Versions> {
		protected override DbSet<NullableInt64Version> VersionDbSet(INullableInt64Versions x) => x.NullableInt64Versions;
	}
	#endregion
	[ComplexType]
	public sealed class VersionedString : VersionedBase<String, StringVersion, IStringVersions> {
		protected override DbSet<StringVersion> VersionDbSet(IStringVersions x) => x.StringVersions;
	}
	[ComplexType]
	public sealed class VersionedRequiredString : VersionedRequiredValueBase<String, RequiredStringVersion, IRequiredStringVersions> {
		protected override String DefaultValue => String.Empty;
		protected override DbSet<RequiredStringVersion> VersionDbSet(IRequiredStringVersions x) => x.RequiredStringVersions;
	}
#if !EF_CORE
	[ComplexType]
	public sealed class VersionedDbGeography : VersionedBase<DbGeography, DbGeographyVersion, IDbGeographyVersions> {
		protected override DbSet<DbGeographyVersion> VersionDbSet(IDbGeographyVersions x) => x.DbGeographyVersions;
	}
	[ComplexType]
	public sealed class VersionedDbGeometry : VersionedBase<DbGeometry, DbGeometryVersion, IDbGeometryVersions> {
		protected override DbSet<DbGeometryVersion> VersionDbSet(IDbGeometryVersions x) => x.DbGeometryVersions;
	}
	[ComplexType]
	public sealed class VersionedRequiredDbGeography : VersionedRequiredValueBase<DbGeography, RequiredDbGeographyVersion, IRequiredDbGeographyVersions> {
		protected override DbSet<RequiredDbGeographyVersion> VersionDbSet(IRequiredDbGeographyVersions x) => x.RequiredDbGeographyVersions;
	}
	[ComplexType]
	public sealed class VersionedRequiredDbGeometry : VersionedRequiredValueBase<DbGeometry, RequiredDbGeometryVersion, IRequiredDbGeometryVersions> {
		protected override DbSet<RequiredDbGeometryVersion> VersionDbSet(IRequiredDbGeometryVersions x) => x.RequiredDbGeometryVersions;
	}
#endif
}
