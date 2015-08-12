using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDateTimeOffset : NullableVersionedBase<DateTimeOffset?, NullableDateTimeOffsetVersion, INullableDateTimeOffsetVersions> {
		protected override Func<INullableDateTimeOffsetVersions, DbSet<NullableDateTimeOffsetVersion>> VersionDbSet => x => x.NullableDateTimeOffsetVersions;
	}
}