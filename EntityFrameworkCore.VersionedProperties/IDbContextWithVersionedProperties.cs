#if EF_CORE
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	/// <summary>
	/// Implement this interface in your <see cref="DbContext"/> to add the standard versioned properties. You must inherit from <see cref="DbContextWithTriggers"/>
	/// or override <see cref="DbContext.SaveChanges"/> and <see cref="DbContext.SaveChangesAsync"/> by calling
	/// <see cref="Extensions.SaveChangesWithTriggers"/> and <see cref="Extensions.SaveChangesWithTriggersAsync"/> respectively to use versioned properties.
	/// </summary>
	public interface IDbContextWithVersionedProperties
		: IBooleanVersions
		, IDateTimeVersions
		, IDateTimeOffsetVersions
		, IDecimalVersions
		, IDoubleVersions
		, IGuidVersions
		, IInt32Versions
		, IInt64Versions
		, INullableBooleanVersions
		, INullableDateTimeVersions
		, INullableDateTimeOffsetVersions
		, INullableDecimalVersions
		, INullableDoubleVersions
		, INullableGuidVersions
		, INullableInt32Versions
		, INullableInt64Versions
#if !EF_CORE
		, IDbGeographyVersions
		, IDbGeometryVersions
#endif
		, IStringVersions
#if !EF_CORE
		, IRequiredDbGeographyVersions
		, IRequiredDbGeometryVersions
#endif
		, IRequiredStringVersions
	{}
	
	public interface IBooleanVersions                { DbSet<BooleanVersion               > BooleanVersions                { get; set; } }
	public interface IDateTimeVersions               { DbSet<DateTimeVersion              > DateTimeVersions               { get; set; } }
	public interface IDateTimeOffsetVersions         { DbSet<DateTimeOffsetVersion        > DateTimeOffsetVersions         { get; set; } }
	public interface IDecimalVersions                { DbSet<DecimalVersion               > DecimalVersions                { get; set; } }
	public interface IDoubleVersions                 { DbSet<DoubleVersion                > DoubleVersions                 { get; set; } }
	public interface IGuidVersions                   { DbSet<GuidVersion                  > GuidVersions                   { get; set; } }
	public interface IInt32Versions                  { DbSet<Int32Version                 > Int32Versions                  { get; set; } }
	public interface IInt64Versions                  { DbSet<Int64Version                 > Int64Versions                  { get; set; } }
	public interface INullableBooleanVersions        { DbSet<NullableBooleanVersion       > NullableBooleanVersions        { get; set; } }
	public interface INullableDateTimeVersions       { DbSet<NullableDateTimeVersion      > NullableDateTimeVersions       { get; set; } }
	public interface INullableDateTimeOffsetVersions { DbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; } }
	public interface INullableDecimalVersions        { DbSet<NullableDecimalVersion       > NullableDecimalVersions        { get; set; } }
	public interface INullableDoubleVersions         { DbSet<NullableDoubleVersion        > NullableDoubleVersions         { get; set; } }
	public interface INullableGuidVersions           { DbSet<NullableGuidVersion          > NullableGuidVersions           { get; set; } }
	public interface INullableInt32Versions          { DbSet<NullableInt32Version         > NullableInt32Versions          { get; set; } }
	public interface INullableInt64Versions          { DbSet<NullableInt64Version         > NullableInt64Versions          { get; set; } }
	#if !EF_CORE
	public interface IDbGeographyVersions            { DbSet<DbGeographyVersion           > DbGeographyVersions            { get; set; } }
	public interface IDbGeometryVersions             { DbSet<DbGeometryVersion            > DbGeometryVersions             { get; set; } }
#endif
	public interface IStringVersions                 { DbSet<StringVersion                > NullableStringVersions         { get; set; } }
#if !EF_CORE
	public interface IRequiredDbGeographyVersions    { DbSet<RequiredDbGeographyVersion   > RequiredDbGeographyVersions    { get; set; } }
	public interface IRequiredDbGeometryVersions     { DbSet<RequiredDbGeometryVersion    > RequiredDbGeometryVersions     { get; set; } }
#endif
	public interface IRequiredStringVersions         { DbSet<RequiredStringVersion        > RequiredStringVersions         { get; set; } }
}