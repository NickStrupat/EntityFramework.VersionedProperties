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
	public class VersionedNullableDateTimeOffset : NullableValueVersionedBase<DateTimeOffset?, NullableDateTimeOffsetVersion, INullableDateTimeOffsetVersions> {
		protected override Func<INullableDateTimeOffsetVersions, DbSet<NullableDateTimeOffsetVersion>> VersionDbSet => x => x.NullableDateTimeOffsetVersions;
	}
}