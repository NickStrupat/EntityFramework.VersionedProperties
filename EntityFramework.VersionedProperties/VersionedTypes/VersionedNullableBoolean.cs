using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableBoolean : NullableVersionedTypeBase<Boolean?, NullableBooleanVersion, INullableBooleanVersions> {
		protected override Func<INullableBooleanVersions, DbSet<NullableBooleanVersion>> VersionDbSet {
			get { return x => x.NullableBooleanVersions; }
		}
	}
}