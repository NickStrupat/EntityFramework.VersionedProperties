using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Triggers;
using System;

namespace EntityFramework.VersionedProperties {
	/// <summary>
	/// Inherit from this class to enable versioned properties in your DbContext. Call SaveChanges and SaveChangesAsync as you normally would. They are overridden to handle the versioned properties behaviour.
	/// </summary>
	public abstract class DbContextWithVersionedProperties : DbContext, IDbContextWithVersionedProperties {
		public DbSet<BooleanVersion> BooleanVersions { get; set; }
		public DbSet<DateTimeVersion> DateTimeVersions { get; set; }
		public DbSet<DateTimeOffsetVersion> DateTimeOffsetVersions { get; set; }
		public DbSet<DbGeographyVersion> DbGeographyVersions { get; set; }
		public DbSet<DbGeometryVersion> DbGeometryVersions { get; set; }
		public DbSet<DecimalVersion> DecimalVersions { get; set; }
		public DbSet<DoubleVersion> DoubleVersions { get; set; }
		public DbSet<GuidVersion> GuidVersions { get; set; }
		public DbSet<Int32Version> Int32Versions { get; set; }
		public DbSet<Int64Version> Int64Versions { get; set; }
		public DbSet<StringVersion> StringVersions { get; set; }
		public DbSet<NullableBooleanVersion> NullableBooleanVersions { get; set; }
		public DbSet<NullableDateTimeVersion> NullableDateTimeVersions { get; set; }
		public DbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; }
		public DbSet<NullableDbGeographyVersion> NullableDbGeographyVersions { get; set; }
		public DbSet<NullableDbGeometryVersion> NullableDbGeometryVersions { get; set; }
		public DbSet<NullableDecimalVersion> NullableDecimalVersions { get; set; }
		public DbSet<NullableDoubleVersion> NullableDoubleVersions { get; set; }
		public DbSet<NullableGuidVersion> NullableGuidVersions { get; set; }
		public DbSet<NullableInt32Version> NullableInt32Versions { get; set; }
		public DbSet<NullableInt64Version> NullableInt64Versions { get; set; }
		public DbSet<NullableStringVersion> NullableStringVersions { get; set; }
	}
}