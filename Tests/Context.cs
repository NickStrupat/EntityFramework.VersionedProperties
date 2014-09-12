using System.Data.Entity;

namespace EntityFrameworkVersionedProperties.Tests {
    public class Context : DbContext, IVersionableDbContext {
        public DbSet<Person> People { get; set; }

	    public IDbSet<BooleanVersion> BooleanVersions { get; set; }
	    public IDbSet<DateTimeVersion> DateTimeVersions { get; set; }
	    public IDbSet<DateTimeOffsetVersion> DateTimeOffsetVersions { get; set; }
	    public IDbSet<DbGeographyVersion> DbGeographyVersions { get; set; }
	    public IDbSet<DbGeometryVersion> DbGeometryVersions { get; set; }
	    public IDbSet<DecimalVersion> DecimalVersions { get; set; }
	    public IDbSet<DoubleVersion> DoubleVersions { get; set; }
	    public IDbSet<GuidVersion> GuidVersions { get; set; }
	    public IDbSet<Int32Version> Int32Versions { get; set; }
	    public IDbSet<Int64Version> Int64Versions { get; set; }
	    public IDbSet<StringVersion> StringVersions { get; set; }
	    public IDbSet<NullableBooleanVersion> NullableBooleanVersions { get; set; }
	    public IDbSet<NullableDateTimeVersion> NullableDateTimeVersions { get; set; }
	    public IDbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; }
	    public IDbSet<NullableDbGeographyVersion> NullableDbGeographyVersions { get; set; }
	    public IDbSet<NullableDbGeometryVersion> NullableDbGeometryVersions { get; set; }
	    public IDbSet<NullableDecimalVersion> NullableDecimalVersions { get; set; }
	    public IDbSet<NullableDoubleVersion> NullableDoubleVersions { get; set; }
	    public IDbSet<NullableGuidVersion> NullableGuidVersions { get; set; }
	    public IDbSet<NullableInt32Version> NullableInt32Versions { get; set; }
	    public IDbSet<NullableInt64Version> NullableInt64Versions { get; set; }
	    public IDbSet<NullableStringVersion> NullableStringVersions { get; set; }
    }
}
