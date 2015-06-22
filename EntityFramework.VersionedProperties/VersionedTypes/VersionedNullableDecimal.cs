using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDecimal : NullableVersionedBase<Decimal?, NullableDecimalVersion, INullableDecimalVersions> {
		protected override Func<INullableDecimalVersions, DbSet<NullableDecimalVersion>> VersionDbSet {
			get { return x => x.NullableDecimalVersions; }
		}
	}
}