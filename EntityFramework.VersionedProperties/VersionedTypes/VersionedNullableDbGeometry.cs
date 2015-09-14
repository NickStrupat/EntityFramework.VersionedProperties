using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDbGeometry : NullableValueVersionedBase<DbGeometry, NullableDbGeometryVersion, INullableDbGeometryVersions> {
		protected override Func<INullableDbGeometryVersions, DbSet<NullableDbGeometryVersion>> VersionDbSet => x => x.NullableDbGeometryVersions;
	}
}