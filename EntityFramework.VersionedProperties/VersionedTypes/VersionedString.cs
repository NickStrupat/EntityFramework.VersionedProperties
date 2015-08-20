using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedString : RequiredValueVersionedBase<String, StringVersion, IStringVersions> {
		protected override String DefaultValue => String.Empty;
		protected override Func<IStringVersions, DbSet<StringVersion>> VersionDbSet => x => x.StringVersions;
	}
}