using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableDouble : NullableVersionedTypeBase<Double?, NullableDoubleVersion, INullableDoubleVersions> {
		protected override Func<INullableDoubleVersions, DbSet<NullableDoubleVersion>> VersionDbSet {
			get { return x => x.NullableDoubleVersions; }
		}
	}
}