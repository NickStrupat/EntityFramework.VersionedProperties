using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedDbGeometry : RequiredValueVersionedBase<DbGeometry, DbGeometryVersion, IDbGeometryVersions> {
		protected override DbGeometry DefaultValue => DbGeometry.FromText("POINT EMPTY");
	    protected override Func<IDbGeometryVersions, DbSet<DbGeometryVersion>> VersionDbSet => x => x.DbGeometryVersions;
	}
}