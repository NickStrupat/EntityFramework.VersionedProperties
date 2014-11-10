using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	/// <summary>
	/// Implement this interface in your <see cref="DbContext"/> to add the standard versioned properties. You must inherit from <see cref="EntityFramework.Triggers.DbContextWithTriggers"/>
	/// or override <see cref="DbContext.SaveChanges"/> and <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> by calling
	/// <see cref="Triggers.Extensions.SaveChangesWithTriggers"/> and <see cref="Triggers.Extensions.SaveChangesWithTriggersAsync"/> respectively to use versioned properties.
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