using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity.Spatial;
namespace EntityFramework.VersionedProperties {
#endif
	#region Primitives
	[ComplexType] public sealed class VersionedBoolean                : VersionedBase<VersionedBoolean, Boolean, BooleanVersion, IBooleanVersions> {}
	[ComplexType] public sealed class VersionedDateTime               : VersionedBase<VersionedDateTime, DateTime, DateTimeVersion, IDateTimeVersions> {}
	[ComplexType] public sealed class VersionedDateTimeOffset         : VersionedBase<VersionedDateTimeOffset, DateTimeOffset, DateTimeOffsetVersion, IDateTimeOffsetVersions> {}
	[ComplexType] public sealed class VersionedDecimal                : VersionedBase<VersionedDecimal, Decimal, DecimalVersion, IDecimalVersions> {}
	[ComplexType] public sealed class VersionedDouble                 : VersionedBase<VersionedDouble, Double, DoubleVersion, IDoubleVersions> {}
	[ComplexType] public sealed class VersionedGuid                   : VersionedBase<VersionedGuid, Guid, GuidVersion, IGuidVersions> {}
	[ComplexType] public sealed class VersionedInt32                  : VersionedBase<VersionedInt32, Int32, Int32Version, IInt32Versions> {}
	[ComplexType] public sealed class VersionedInt64                  : VersionedBase<VersionedInt64, Int64, Int64Version, IInt64Versions> {}
	#endregion
	#region Nullable primitives
	[ComplexType] public sealed class VersionedNullableBoolean        : VersionedBase<VersionedNullableBoolean, Boolean?, NullableBooleanVersion, INullableBooleanVersions> {}
	[ComplexType] public sealed class VersionedNullableDateTime       : VersionedBase<VersionedNullableDateTime, DateTime?, NullableDateTimeVersion, INullableDateTimeVersions> {}
	[ComplexType] public sealed class VersionedNullableDateTimeOffset : VersionedBase<VersionedNullableDateTimeOffset, DateTimeOffset?, NullableDateTimeOffsetVersion, INullableDateTimeOffsetVersions> {}
	[ComplexType] public sealed class VersionedNullableDecimal        : VersionedBase<VersionedNullableDecimal, Decimal?, NullableDecimalVersion, INullableDecimalVersions> {}
	[ComplexType] public sealed class VersionedNullableDouble         : VersionedBase<VersionedNullableDouble, Double?, NullableDoubleVersion, INullableDoubleVersions> {}
	[ComplexType] public sealed class VersionedNullableGuid           : VersionedBase<VersionedNullableGuid, Guid?, NullableGuidVersion, INullableGuidVersions> {}
	[ComplexType] public sealed class VersionedNullableInt32          : VersionedBase<VersionedNullableInt32, Int32?, NullableInt32Version, INullableInt32Versions> {}
	[ComplexType] public sealed class VersionedNullableInt64          : VersionedBase<VersionedNullableInt64, Int64?, NullableInt64Version, INullableInt64Versions> {}
	#endregion
	[ComplexType] public sealed class VersionedString                 : VersionedBase<VersionedString, String, StringVersion, IStringVersions> {}
	[ComplexType] public sealed class VersionedRequiredString         : VersionedRequiredValueBase<VersionedRequiredString, String, RequiredStringVersion, IRequiredStringVersions> {
		protected override String DefaultValue => String.Empty;
	}
#if !EF_CORE
	[ComplexType] public sealed class VersionedDbGeography            : VersionedBase<VersionedDbGeography, DbGeography, DbGeographyVersion, IDbGeographyVersions> {}
	[ComplexType] public sealed class VersionedDbGeometry             : VersionedBase<VersionedDbGeometry, DbGeometry, DbGeometryVersion, IDbGeometryVersions> {}
	[ComplexType] public sealed class VersionedRequiredDbGeography    : VersionedRequiredValueBase<VersionedRequiredDbGeography, DbGeography, RequiredDbGeographyVersion, IRequiredDbGeographyVersions> {
		protected override DbGeography DefaultValue => DbGeography.FromText("POINT EMPTY");
	}
	[ComplexType] public sealed class VersionedRequiredDbGeometry     : VersionedRequiredValueBase<VersionedRequiredDbGeometry, DbGeometry, RequiredDbGeometryVersion, IRequiredDbGeometryVersions> {
		protected override DbGeometry DefaultValue => DbGeometry.FromText("POINT EMPTY");
	}
#endif
}
