using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedDateTime : VersionedBase<DateTime, DateTimeVersion, IDateTimeVersions> {
		protected override Func<IDateTimeVersions, DbSet<DateTimeVersion>> VersionDbSet {
			get { return x => x.DateTimeVersions; }
		}
	}
}