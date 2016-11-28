using System.ComponentModel;
#if EF_CORE
using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Triggers;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
using EntityFramework.Triggers;
namespace EntityFramework.VersionedProperties {
#endif
	/// <summary>
	/// Inherit from this class to enable versioned properties in your <see cref="DbContext"/>. Call <see cref="DbContext.SaveChanges"/>, <see cref="DbContext.SaveChangesAsync()"/>, and
	/// <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> as you normally would. They are overridden in <see cref="DbContextWithTriggers"/> to handle the
	/// versioned properties functionality.
	/// </summary>
	public abstract class DbContextWithVersionedProperties : DbContextWithTriggers, IDbContextWithVersionedProperties {
#if EF_CORE
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);
			foreach (var versionType in this.GetType()
			                                .GetProperties()
			                                .Where(x => typeof(DbSet<>) == x.PropertyType.GetGenericTypeDefinition())
			                                .Select(x => x.PropertyType.GenericTypeArguments.Single())
			                                .Where(x => typeof(IVersion).IsAssignableFrom(x))
			                                .Select(x => x.GetTypeInfo().BaseType.GenericTypeArguments.Single())
			                                .Distinct())
			modelBuilder.Entity(versionType).HasIndex(nameof(VersionBase<Object>.Value));
		}
#endif
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<BooleanVersion               > BooleanVersions                { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<DateTimeVersion              > DateTimeVersions               { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<DateTimeOffsetVersion        > DateTimeOffsetVersions         { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<DecimalVersion               > DecimalVersions                { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<DoubleVersion                > DoubleVersions                 { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<GuidVersion                  > GuidVersions                   { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<Int32Version                 > Int32Versions                  { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<Int64Version                 > Int64Versions                  { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<NullableBooleanVersion       > NullableBooleanVersions        { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<NullableDateTimeVersion      > NullableDateTimeVersions       { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<NullableDecimalVersion       > NullableDecimalVersions        { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<NullableDoubleVersion        > NullableDoubleVersions         { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<NullableGuidVersion          > NullableGuidVersions           { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<NullableInt32Version         > NullableInt32Versions          { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<NullableInt64Version         > NullableInt64Versions          { get; set; }

#if !EF_CORE
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<DbGeographyVersion           > DbGeographyVersions            { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<DbGeometryVersion            > DbGeometryVersions             { get; set; }
#endif

		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<StringVersion                > StringVersions                 { get; set; }

#if !EF_CORE
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<RequiredDbGeographyVersion   > RequiredDbGeographyVersions    { get; set; }
		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<RequiredDbGeometryVersion    > RequiredDbGeometryVersions     { get; set; }
#endif

		[EditorBrowsable(EditorBrowsableState.Advanced)] public DbSet<RequiredStringVersion        > RequiredStringVersions         { get; set; }
	}
}