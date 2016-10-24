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
	public sealed class VersionedDecimal : VersionedBase<Decimal, DecimalVersion, IDecimalVersions> {
		protected override Func<IDecimalVersions, DbSet<DecimalVersion>> VersionDbSet => x => x.DecimalVersions;
	}
}