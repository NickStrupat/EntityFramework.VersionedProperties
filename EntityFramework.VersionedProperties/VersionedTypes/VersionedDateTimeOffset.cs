using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedDateTimeOffset : VersionedTypeBase<DateTimeOffset, DateTimeOffsetVersion, IDateTimeOffsetVersions> {
		protected override Func<IDateTimeOffsetVersions, DbSet<DateTimeOffsetVersion>> VersionDbSet {
			get { return x => x.DateTimeOffsetVersions; }
		}
	}
}