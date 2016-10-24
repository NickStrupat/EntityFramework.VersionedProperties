using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
using System.Data.Entity.Spatial;
namespace EntityFramework.VersionedProperties {
#endif
#if !EF_CORE
	[ComplexType]
	public sealed class VersionedDbGeography : VersionedBase<DbGeography, NullableDbGeographyVersion, INullableDbGeographyVersions> {
		protected override Func<INullableDbGeographyVersions, DbSet<NullableDbGeographyVersion>> VersionDbSet => x => x.NullableDbGeographyVersions;
	}
#endif
}