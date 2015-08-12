using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableGuid : NullableVersionedBase<Guid?, NullableGuidVersion, INullableGuidVersions> {
		protected override Func<INullableGuidVersions, DbSet<NullableGuidVersion>> VersionDbSet => x => x.NullableGuidVersions;
	}
}