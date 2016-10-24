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
	public sealed class VersionedDbGeography : RequiredValueVersionedBase<DbGeography, DbGeographyVersion, IDbGeographyVersions> {
		protected override DbGeography DefaultValue => DbGeography.FromText("POINT EMPTY");
	    protected override Func<IDbGeographyVersions, DbSet<DbGeographyVersion>> VersionDbSet => x => x.DbGeographyVersions;
	}
#endif
}