using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
using System.Data.Entity.Spatial;
namespace EntityFramework.VersionedProperties {
#endif
	[ComplexType]
	public sealed class VersionedNullableDecimal : NullableValueVersionedBase<Decimal?, NullableDecimalVersion, INullableDecimalVersions> {
		protected override Func<INullableDecimalVersions, DbSet<NullableDecimalVersion>> VersionDbSet => x => x.NullableDecimalVersions;
	}
}