using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedDbGeography : RequiredValueVersionedBase<DbGeography, DbGeographyVersion, IDbGeographyVersions> {
		protected override DbGeography DefaultValue {
			get { return DbGeography.FromText("POINT EMPTY"); }
		}
		protected override Func<IDbGeographyVersions, DbSet<DbGeographyVersion>> VersionDbSet {
			get { return x => x.DbGeographyVersions; }
		}
	}
}