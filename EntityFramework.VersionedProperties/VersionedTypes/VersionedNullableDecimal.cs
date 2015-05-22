using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDecimal : NullableVersionedTypeBase<Decimal?, NullableDecimalVersion, INullableDecimalVersions> {
		protected override Func<INullableDecimalVersions, DbSet<NullableDecimalVersion>> VersionDbSet {
			get { return x => x.NullableDecimalVersions; }
		}
	}
}