using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedDecimal : VersionedBase<Decimal, DecimalVersion, IDecimalVersions> {
		protected override Func<IDecimalVersions, DbSet<DecimalVersion>> VersionDbSet => x => x.DecimalVersions;
	}
}