using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDbGeography : NullableVersionedTypeBase<DbGeography, NullableDbGeographyVersion, INullableDbGeographyVersions> {
		protected override Func<INullableDbGeographyVersions, DbSet<NullableDbGeographyVersion>> VersionDbSet {
			get { return x => x.NullableDbGeographyVersions; }
		}
	}
}