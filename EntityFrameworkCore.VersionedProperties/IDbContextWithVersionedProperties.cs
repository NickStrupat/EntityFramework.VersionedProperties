using System.ComponentModel;
#if EF_CORE
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
using EntityFramework.Triggers;
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
		, ISingleVersions
		, IGuidVersions
		, IInt16Versions
		, IInt32Versions
		, IInt64Versions
		, IByteVersions

		, INullableBooleanVersions
		, INullableDateTimeVersions
		, INullableDateTimeOffsetVersions
		, INullableDecimalVersions
		, INullableSingleVersions
		, INullableDoubleVersions
		, INullableGuidVersions
		, INullableInt16Versions
		, INullableInt32Versions
		, INullableInt64Versions
		, INullableByteVersions

		, IStringVersions
		, IRequiredStringVersions
		, IByteArrayVersions
		, IRequiredByteArrayVersions
#if !EF_CORE
		, IDbGeographyVersions
		, IDbGeometryVersions
		, IRequiredDbGeographyVersions
		, IRequiredDbGeometryVersions
#endif
	{}
	
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IBooleanVersions                { DbSet<BooleanVersion               > BooleanVersions                { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IDateTimeVersions               { DbSet<DateTimeVersion              > DateTimeVersions               { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IDateTimeOffsetVersions         { DbSet<DateTimeOffsetVersion        > DateTimeOffsetVersions         { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IDecimalVersions                { DbSet<DecimalVersion               > DecimalVersions                { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IDoubleVersions                 { DbSet<DoubleVersion                > DoubleVersions                 { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface ISingleVersions                 { DbSet<SingleVersion                > SingleVersions                 { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IGuidVersions                   { DbSet<GuidVersion                  > GuidVersions                   { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IInt16Versions                  { DbSet<Int16Version                 > Int16Versions                  { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IInt32Versions                  { DbSet<Int32Version                 > Int32Versions                  { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IInt64Versions                  { DbSet<Int64Version                 > Int64Versions                  { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IByteVersions                   { DbSet<ByteVersion                  > ByteVersions                   { get; set; } }

	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableBooleanVersions        { DbSet<NullableBooleanVersion       > NullableBooleanVersions        { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableDateTimeVersions       { DbSet<NullableDateTimeVersion      > NullableDateTimeVersions       { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableDateTimeOffsetVersions { DbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableDecimalVersions        { DbSet<NullableDecimalVersion       > NullableDecimalVersions        { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableDoubleVersions         { DbSet<NullableDoubleVersion        > NullableDoubleVersions         { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableSingleVersions         { DbSet<NullableSingleVersion        > NullableSingleVersions         { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableGuidVersions           { DbSet<NullableGuidVersion          > NullableGuidVersions           { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableInt16Versions          { DbSet<NullableInt16Version         > NullableInt16Versions          { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableInt32Versions          { DbSet<NullableInt32Version         > NullableInt32Versions          { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableInt64Versions          { DbSet<NullableInt64Version         > NullableInt64Versions          { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface INullableByteVersions           { DbSet<NullableByteVersion          > NullableByteVersions           { get; set; } }

	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IStringVersions                 { DbSet<StringVersion                > StringVersions                 { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IRequiredStringVersions         { DbSet<RequiredStringVersion        > RequiredStringVersions         { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IByteArrayVersions              { DbSet<ByteArrayVersion             > ByteArrayVersions              { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IRequiredByteArrayVersions      { DbSet<RequiredByteArrayVersion     > RequiredByteArrayVersions      { get; set; } }
#if !EF_CORE
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IDbGeographyVersions            { DbSet<DbGeographyVersion           > DbGeographyVersions            { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IDbGeometryVersions             { DbSet<DbGeometryVersion            > DbGeometryVersions             { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IRequiredDbGeographyVersions    { DbSet<RequiredDbGeographyVersion   > RequiredDbGeographyVersions    { get; set; } }
	[EditorBrowsable(EditorBrowsableState.Advanced)] public interface IRequiredDbGeometryVersions     { DbSet<RequiredDbGeometryVersion    > RequiredDbGeometryVersions     { get; set; } }
#endif
}