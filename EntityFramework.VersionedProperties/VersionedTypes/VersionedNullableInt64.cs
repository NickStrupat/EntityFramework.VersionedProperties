using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableInt64 : NullableVersionedBase<Int64?, NullableInt64Version, INullableInt64Versions> {
		protected override Func<INullableInt64Versions, DbSet<NullableInt64Version>> VersionDbSet {
			get { return x => x.NullableInt64Versions; }
		}
	}
}