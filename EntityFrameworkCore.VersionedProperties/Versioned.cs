using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
using System.Data.Entity.Spatial;
namespace EntityFramework.VersionedProperties {
#endif
	#region Primitives
	[ComplexType]
	public sealed class VersionedBoolean : VersionedBase<VersionedBoolean, Boolean, BooleanVersion, IBooleanVersions> {
		protected override DbSet<BooleanVersion> GetVersionDbSet(IBooleanVersions x) => x.BooleanVersions;
	}
	[ComplexType]
	public sealed class VersionedDateTime : VersionedBase<VersionedDateTime, DateTime, DateTimeVersion, IDateTimeVersions> {
		protected override DbSet<DateTimeVersion> GetVersionDbSet(IDateTimeVersions x) => x.DateTimeVersions;
	}
	[ComplexType]
	public sealed class VersionedDateTimeOffset : VersionedBase<VersionedDateTimeOffset, DateTimeOffset, DateTimeOffsetVersion, IDateTimeOffsetVersions> {
		protected override DbSet<DateTimeOffsetVersion> GetVersionDbSet(IDateTimeOffsetVersions x) => x.DateTimeOffsetVersions;
	}
	[ComplexType]
	public sealed class VersionedDecimal : VersionedBase<VersionedDecimal, Decimal, DecimalVersion, IDecimalVersions> {
		protected override DbSet<DecimalVersion> GetVersionDbSet(IDecimalVersions x) => x.DecimalVersions;
	}
	[ComplexType]
	public sealed class VersionedDouble : VersionedBase<VersionedDouble, Double, DoubleVersion, IDoubleVersions> {
		protected override DbSet<DoubleVersion> GetVersionDbSet(IDoubleVersions x) => x.DoubleVersions;
	}
	[ComplexType]
	public sealed class VersionedGuid : VersionedBase<VersionedGuid, Guid, GuidVersion, IGuidVersions> {
		protected override DbSet<GuidVersion> GetVersionDbSet(IGuidVersions x) => x.GuidVersions;
	}
	[ComplexType]
	public sealed class VersionedInt32 : VersionedBase<VersionedInt32, Int32, Int32Version, IInt32Versions> {
		protected override DbSet<Int32Version> GetVersionDbSet(IInt32Versions x) => x.Int32Versions;
	}
	[ComplexType]
	public sealed class VersionedInt64 : VersionedBase<VersionedInt64, Int64, Int64Version, IInt64Versions> {
		protected override DbSet<Int64Version> GetVersionDbSet(IInt64Versions x) => x.Int64Versions;
	}
	#endregion
	#region Nullable primitives
	[ComplexType]
	public sealed class VersionedNullableBoolean : VersionedBase<VersionedNullableBoolean, Boolean?, NullableBooleanVersion, INullableBooleanVersions> {
		protected override DbSet<NullableBooleanVersion> GetVersionDbSet(INullableBooleanVersions x) => x.NullableBooleanVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableDateTime : VersionedBase<VersionedNullableDateTime, DateTime?, NullableDateTimeVersion, INullableDateTimeVersions> {
		protected override DbSet<NullableDateTimeVersion> GetVersionDbSet(INullableDateTimeVersions x) => x.NullableDateTimeVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableDateTimeOffset : VersionedBase<VersionedNullableDateTimeOffset, DateTimeOffset?, NullableDateTimeOffsetVersion, INullableDateTimeOffsetVersions> {
		protected override DbSet<NullableDateTimeOffsetVersion> GetVersionDbSet(INullableDateTimeOffsetVersions x) => x.NullableDateTimeOffsetVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableDecimal : VersionedBase<VersionedNullableDecimal, Decimal?, NullableDecimalVersion, INullableDecimalVersions> {
		protected override DbSet<NullableDecimalVersion> GetVersionDbSet(INullableDecimalVersions x) => x.NullableDecimalVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableDouble : VersionedBase<VersionedNullableDouble, Double?, NullableDoubleVersion, INullableDoubleVersions> {
		protected override DbSet<NullableDoubleVersion> GetVersionDbSet(INullableDoubleVersions x) => x.NullableDoubleVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableGuid : VersionedBase<VersionedNullableGuid, Guid?, NullableGuidVersion, INullableGuidVersions> {
		protected override DbSet<NullableGuidVersion> GetVersionDbSet(INullableGuidVersions x) => x.NullableGuidVersions;
	}
	[ComplexType]
	public sealed class VersionedNullableInt32 : VersionedBase<VersionedNullableInt32, Int32?, NullableInt32Version, INullableInt32Versions> {
		protected override DbSet<NullableInt32Version> GetVersionDbSet(INullableInt32Versions x) => x.NullableInt32Versions;
	}
	[ComplexType]
	public sealed class VersionedNullableInt64 : VersionedBase<VersionedNullableInt64, Int64?, NullableInt64Version, INullableInt64Versions> {
		protected override DbSet<NullableInt64Version> GetVersionDbSet(INullableInt64Versions x) => x.NullableInt64Versions;
	}
	#endregion
	[ComplexType]
	public sealed class VersionedString : VersionedBase<VersionedString, String, StringVersion, IStringVersions> {
		protected override DbSet<StringVersion> GetVersionDbSet(IStringVersions x) => x.StringVersions;
	}
	[ComplexType]
	public sealed class VersionedRequiredString : VersionedRequiredValueBase<VersionedRequiredString, String, RequiredStringVersion, IRequiredStringVersions> {
		protected override String DefaultValue => String.Empty;
		protected override DbSet<RequiredStringVersion> GetVersionDbSet(IRequiredStringVersions x) => x.RequiredStringVersions;
	}
#if !EF_CORE
	[ComplexType]
	public sealed class VersionedDbGeography : VersionedBase<VersionedDbGeography, DbGeography, DbGeographyVersion, IDbGeographyVersions> {
		protected override DbSet<DbGeographyVersion> GetVersionDbSet(IDbGeographyVersions x) => x.DbGeographyVersions;
	}
	[ComplexType]
	public sealed class VersionedDbGeometry : VersionedBase<VersionedDbGeometry, DbGeometry, DbGeometryVersion, IDbGeometryVersions> {
		protected override DbSet<DbGeometryVersion> GetVersionDbSet(IDbGeometryVersions x) => x.DbGeometryVersions;
	}
	[ComplexType]
	public sealed class VersionedRequiredDbGeography : VersionedRequiredValueBase<VersionedRequiredDbGeography, DbGeography, RequiredDbGeographyVersion, IRequiredDbGeographyVersions> {
		protected override DbGeography DefaultValue => DbGeography.FromText("POINT EMPTY");
		protected override DbSet<RequiredDbGeographyVersion> GetVersionDbSet(IRequiredDbGeographyVersions x) => x.RequiredDbGeographyVersions;
	}
	[ComplexType]
	public sealed class VersionedRequiredDbGeometry : VersionedRequiredValueBase<VersionedRequiredDbGeometry, DbGeometry, RequiredDbGeometryVersion, IRequiredDbGeometryVersions> {
		protected override DbGeometry DefaultValue => DbGeometry.FromText("POINT EMPTY");
		protected override DbSet<RequiredDbGeometryVersion> GetVersionDbSet(IRequiredDbGeometryVersions x) => x.RequiredDbGeometryVersions;
	}
#endif
}
