﻿using System;
using System.Linq;
using System.Reflection;

#if EF_CORE
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
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

		public DbSet<BooleanVersion               > BooleanVersions                { get; set; }
		public DbSet<DateTimeVersion              > DateTimeVersions               { get; set; }
		public DbSet<DateTimeOffsetVersion        > DateTimeOffsetVersions         { get; set; }
#if !EF_CORE
		public DbSet<DbGeographyVersion           > DbGeographyVersions            { get; set; }
		public DbSet<DbGeometryVersion            > DbGeometryVersions             { get; set; }
#endif
		public DbSet<DecimalVersion               > DecimalVersions                { get; set; }
		public DbSet<DoubleVersion                > DoubleVersions                 { get; set; }
		public DbSet<GuidVersion                  > GuidVersions                   { get; set; }
		public DbSet<Int32Version                 > Int32Versions                  { get; set; }
		public DbSet<Int64Version                 > Int64Versions                  { get; set; }
		public DbSet<StringVersion                > StringVersions                 { get; set; }
		public DbSet<NullableBooleanVersion       > NullableBooleanVersions        { get; set; }
		public DbSet<NullableDateTimeVersion      > NullableDateTimeVersions       { get; set; }
		public DbSet<NullableDateTimeOffsetVersion> NullableDateTimeOffsetVersions { get; set; }
#if !EF_CORE
		public DbSet<NullableDbGeographyVersion   > NullableDbGeographyVersions    { get; set; }
		public DbSet<NullableDbGeometryVersion    > NullableDbGeometryVersions     { get; set; }
#endif
		public DbSet<NullableDecimalVersion       > NullableDecimalVersions        { get; set; }
		public DbSet<NullableDoubleVersion        > NullableDoubleVersions         { get; set; }
		public DbSet<NullableGuidVersion          > NullableGuidVersions           { get; set; }
		public DbSet<NullableInt32Version         > NullableInt32Versions          { get; set; }
		public DbSet<NullableInt64Version         > NullableInt64Versions          { get; set; }
		public DbSet<NullableStringVersion        > NullableStringVersions         { get; set; }
	}
}