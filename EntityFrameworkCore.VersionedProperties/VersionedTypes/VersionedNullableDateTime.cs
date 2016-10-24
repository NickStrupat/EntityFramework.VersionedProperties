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
	public sealed class VersionedNullableDateTime : NullableValueVersionedBase<DateTime?, NullableDateTimeVersion, INullableDateTimeVersions> {
		protected override Func<INullableDateTimeVersions, DbSet<NullableDateTimeVersion>> VersionDbSet => x => x.NullableDateTimeVersions;
	}
}