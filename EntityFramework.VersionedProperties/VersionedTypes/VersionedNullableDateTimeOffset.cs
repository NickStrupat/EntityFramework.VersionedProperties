using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDateTimeOffset : NullableVersionedTypeBase<DateTimeOffset?, NullableDateTimeOffsetVersion, INullableDateTimeOffsetVersions> {
		protected override Func<INullableDateTimeOffsetVersions, DbSet<NullableDateTimeOffsetVersion>> VersionDbSet {
			get { return x => x.NullableDateTimeOffsetVersions; }
		}
	}
}