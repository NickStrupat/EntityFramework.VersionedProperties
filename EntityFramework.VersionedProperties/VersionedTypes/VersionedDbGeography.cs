using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedDbGeography : RequiredValueVersionedBase<DbGeography, DbGeographyVersion, IDbGeographyVersions> {
		protected override DbGeography DefaultValue => DbGeography.FromText("POINT EMPTY");
	    protected override Func<IDbGeographyVersions, DbSet<DbGeographyVersion>> VersionDbSet => x => x.DbGeographyVersions;
	}
}