using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableInt32 : NullableVersionedTypeBase<Int32?, NullableInt32Version, INullableInt32Versions> {
		protected override Func<INullableInt32Versions, DbSet<NullableInt32Version>> VersionDbSet {
			get { return x => x.NullableInt32Versions; }
		}
	}
}