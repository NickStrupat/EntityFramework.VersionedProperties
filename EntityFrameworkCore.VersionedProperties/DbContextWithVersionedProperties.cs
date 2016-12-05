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
			var versionTypes = this.GetType()
			                       .GetProperties()
			                       .Where(x => x.PropertyType.GetTypeInfo().IsGenericType && typeof(DbSet<>) == x.PropertyType.GetGenericTypeDefinition())
			                       .Select(x => x.PropertyType.GenericTypeArguments.Single())
			                       .Where(x => typeof(IVersion).IsAssignableFrom(x))
			                       //.Select(x => x.GetTypeInfo().BaseType.GenericTypeArguments.Single())
			                       .Distinct()
			                       .ToArray();
			foreach (var versionType in versionTypes)
				modelBuilder.Entity(versionType).HasIndex(nameof(VersionBase<Object>.Value));
		}
#endif
		public virtual DbSet<BooleanVersion               > BooleanVersions                { get; set; }
		public virtual DbSet<DateTimeVersion              > DateTimeVersions               { get; set; }
		public virtual DbSet<DateTimeOffsetVersion        > DateTimeOffsetVersions         { get; set; }
		public virtual DbSet<DecimalVersion               > DecimalVersions                { get; set; }
		public virtual DbSet<DoubleVersion                > DoubleVersions                 { get; set; }
		public virtual DbSet<SingleVersion                > SingleVersions                 { get; set; }
		public virtual DbSet<GuidVersion                  > GuidVersions                   { get; set; }
		public virtual DbSet<Int16Version                 > Int16Versions                  { get; set; }
		public virtual DbSet<Int32Version                 > Int32Versions                  { get; set; }
		public virtual DbSet<Int64Version                 > Int64Versions                  { get; set; }
		public virtual DbSet<ByteVersion                  > ByteVersions                   { get; set; }

		public virtual DbSet<NullableBooleanVersion       > NullableBooleanVersions        { get; set; }
		public virtual DbSet<NullableDateTimeVersion      > NullableDateTimeVersions       { get; set; }
		public virtual DbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; }
		public virtual DbSet<NullableDecimalVersion       > NullableDecimalVersions        { get; set; }
		public virtual DbSet<NullableDoubleVersion        > NullableDoubleVersions         { get; set; }
		public virtual DbSet<NullableSingleVersion        > NullableSingleVersions         { get; set; }
		public virtual DbSet<NullableGuidVersion          > NullableGuidVersions           { get; set; }
		public virtual DbSet<NullableInt16Version         > NullableInt16Versions          { get; set; }
		public virtual DbSet<NullableInt32Version         > NullableInt32Versions          { get; set; }
		public virtual DbSet<NullableInt64Version         > NullableInt64Versions          { get; set; }
		public virtual DbSet<NullableByteVersion          > NullableByteVersions           { get; set; }

		public virtual DbSet<StringVersion                > StringVersions                 { get; set; }
		public virtual DbSet<RequiredStringVersion        > RequiredStringVersions         { get; set; }
		public virtual DbSet<ByteArrayVersion             > ByteArrayVersions              { get; set; }
		public virtual DbSet<RequiredByteArrayVersion     > RequiredByteArrayVersions      { get; set; }
#if !EF_CORE
		public virtual DbSet<DbGeographyVersion           > DbGeographyVersions            { get; set; }
		public virtual DbSet<DbGeometryVersion            > DbGeometryVersions             { get; set; }
		public virtual DbSet<RequiredDbGeographyVersion   > RequiredDbGeographyVersions    { get; set; }
		public virtual DbSet<RequiredDbGeometryVersion    > RequiredDbGeometryVersions     { get; set; }
#endif
	}
}