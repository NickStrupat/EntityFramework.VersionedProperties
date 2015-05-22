using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableGuid : NullableVersionedTypeBase<Guid?, NullableGuidVersion, INullableGuidVersions> {
		protected override Func<INullableGuidVersions, DbSet<NullableGuidVersion>> VersionDbSet {
			get { return x => x.NullableGuidVersions; }
		}
	}
}