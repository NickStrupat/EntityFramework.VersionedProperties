using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedString : RequiredValueVersionedTypeBase<String, StringVersion, IStringVersions> {
		protected override String DefaultValue {
			get { return String.Empty; }
		}
		protected override Func<IStringVersions, DbSet<StringVersion>> VersionDbSet {
			get { return x => x.StringVersions; }
		}
	}
}