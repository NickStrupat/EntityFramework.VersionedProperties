using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	[ComplexType]
	public sealed class VersionedDateTime : VersionedBase<DateTime, DateTimeVersion, IDateTimeVersions> {
		protected override Func<IDateTimeVersions, DbSet<DateTimeVersion>> VersionDbSet => x => x.DateTimeVersions;
	}
}