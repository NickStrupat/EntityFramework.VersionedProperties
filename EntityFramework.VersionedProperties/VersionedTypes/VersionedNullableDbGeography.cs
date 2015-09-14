using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDbGeography : NullableValueVersionedBase<DbGeography, NullableDbGeographyVersion, INullableDbGeographyVersions> {
		protected override Func<INullableDbGeographyVersions, DbSet<NullableDbGeographyVersion>> VersionDbSet => x => x.NullableDbGeographyVersions;
	}
}