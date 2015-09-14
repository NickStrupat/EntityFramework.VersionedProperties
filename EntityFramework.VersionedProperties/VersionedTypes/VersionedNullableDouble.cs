using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDouble : NullableValueVersionedBase<Double?, NullableDoubleVersion, INullableDoubleVersions> {
		protected override Func<INullableDoubleVersions, DbSet<NullableDoubleVersion>> VersionDbSet => x => x.NullableDoubleVersions;
	}
}