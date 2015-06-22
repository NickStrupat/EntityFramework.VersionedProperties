using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDbGeometry : NullableVersionedBase<DbGeometry, NullableDbGeometryVersion, INullableDbGeometryVersions> {
		protected override Func<INullableDbGeometryVersions, DbSet<NullableDbGeometryVersion>> VersionDbSet {
			get { return x => x.NullableDbGeometryVersions; }
		}
	}
}