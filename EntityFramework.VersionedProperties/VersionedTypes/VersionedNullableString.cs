using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedNullableString : NullableVersionedBase<String, NullableStringVersion, INullableStringVersions> {
		protected override Func<INullableStringVersions, DbSet<NullableStringVersion>> VersionDbSet {
			get { return x => x.NullableStringVersions; }
		}
	}
}