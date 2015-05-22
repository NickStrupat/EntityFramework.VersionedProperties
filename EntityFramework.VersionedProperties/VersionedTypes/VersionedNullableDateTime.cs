using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDateTime : NullableVersionedTypeBase<DateTime?, NullableDateTimeVersion, INullableDateTimeVersions> {
		protected override Func<INullableDateTimeVersions, DbSet<NullableDateTimeVersion>> VersionDbSet {
			get { return x => x.NullableDateTimeVersions; }
		}
	}
}