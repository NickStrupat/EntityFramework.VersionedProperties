using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	/// <summary>
	/// Inherit from this interface to add the standard versioned properties in your DbContext. You must call <c>dbContext.SaveChangesWithTriggers()</c> or <c>await dbContext.SaveChangesWithTriggersAsync()</c> to use versioned properties.
	/// </summary>
	public interface IDbContextWithVersionedProperties {
		DbSet<BooleanVersion> BooleanVersions { get; set; }
		DbSet<DateTimeVersion> DateTimeVersions { get; set; }
		DbSet<DateTimeOffsetVersion> DateTimeOffsetVersions { get; set; }
		DbSet<DbGeographyVersion> DbGeographyVersions { get; set; }
		DbSet<DbGeometryVersion> DbGeometryVersions { get; set; }
		DbSet<DecimalVersion> DecimalVersions { get; set; }
		DbSet<DoubleVersion> DoubleVersions { get; set; }
		DbSet<GuidVersion> GuidVersions { get; set; }
		DbSet<Int32Version> Int32Versions { get; set; }
		DbSet<Int64Version> Int64Versions { get; set; }
		DbSet<StringVersion> StringVersions { get; set; }

		DbSet<NullableBooleanVersion> NullableBooleanVersions { get; set; }
		DbSet<NullableDateTimeVersion> NullableDateTimeVersions { get; set; }
		DbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; }
		DbSet<NullableDbGeographyVersion> NullableDbGeographyVersions { get; set; }
		DbSet<NullableDbGeometryVersion> NullableDbGeometryVersions { get; set; }
		DbSet<NullableDecimalVersion> NullableDecimalVersions { get; set; }
		DbSet<NullableDoubleVersion> NullableDoubleVersions { get; set; }
		DbSet<NullableGuidVersion> NullableGuidVersions { get; set; }
		DbSet<NullableInt32Version> NullableInt32Versions { get; set; }
		DbSet<NullableInt64Version> NullableInt64Versions { get; set; }
		DbSet<NullableStringVersion> NullableStringVersions { get; set; }
	}
}