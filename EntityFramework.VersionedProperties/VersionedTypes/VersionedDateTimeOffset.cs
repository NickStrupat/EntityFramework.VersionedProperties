using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedDateTimeOffset : VersionedBase<DateTimeOffset, DateTimeOffsetVersion, IDateTimeOffsetVersions> {
		protected override Func<IDateTimeOffsetVersions, DbSet<DateTimeOffsetVersion>> VersionDbSet => x => x.DateTimeOffsetVersions;
	}
}