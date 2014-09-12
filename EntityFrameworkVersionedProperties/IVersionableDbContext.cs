using System.Data.Entity;

namespace EntityFrameworkVersionedProperties {
	public interface IVersionableDbContext {
		IDbSet<BooleanVersion> BooleanVersions { get; set; }
		IDbSet<DateTimeVersion> DateTimeVersions { get; set; }
		IDbSet<DateTimeOffsetVersion> DateTimeOffsetVersions { get; set; }
		IDbSet<DbGeographyVersion> DbGeographyVersions { get; set; }
		IDbSet<DbGeometryVersion> DbGeometryVersions { get; set; }
		IDbSet<DecimalVersion> DecimalVersions { get; set; }
		IDbSet<DoubleVersion> DoubleVersions { get; set; }
		IDbSet<GuidVersion> GuidVersions { get; set; }
		IDbSet<Int32Version> Int32Versions { get; set; }
		IDbSet<Int64Version> Int64Versions { get; set; }
		IDbSet<StringVersion> StringVersions { get; set; }

		IDbSet<NullableBooleanVersion> NullableBooleanVersions { get; set; }
		IDbSet<NullableDateTimeVersion> NullableDateTimeVersions { get; set; }
		IDbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; }
		IDbSet<NullableDbGeographyVersion> NullableDbGeographyVersions { get; set; }
		IDbSet<NullableDbGeometryVersion> NullableDbGeometryVersions { get; set; }
		IDbSet<NullableDecimalVersion> NullableDecimalVersions { get; set; }
		IDbSet<NullableDoubleVersion> NullableDoubleVersions { get; set; }
		IDbSet<NullableGuidVersion> NullableGuidVersions { get; set; }
		IDbSet<NullableInt32Version> NullableInt32Versions { get; set; }
		IDbSet<NullableInt64Version> NullableInt64Versions { get; set; }
		IDbSet<NullableStringVersion> NullableStringVersions { get; set; }
	}
}