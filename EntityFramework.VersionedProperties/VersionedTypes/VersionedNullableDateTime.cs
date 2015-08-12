using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDateTime : NullableVersionedBase<DateTime?, NullableDateTimeVersion, INullableDateTimeVersions> {
		protected override Func<INullableDateTimeVersions, DbSet<NullableDateTimeVersion>> VersionDbSet => x => x.NullableDateTimeVersions;
	}
}