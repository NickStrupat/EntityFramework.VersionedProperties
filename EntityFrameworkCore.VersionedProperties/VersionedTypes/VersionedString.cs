using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
using System.Data.Entity.Spatial;
namespace EntityFramework.VersionedProperties {
#endif
	[ComplexType]
	public class VersionedString : RequiredValueVersionedBase<String, StringVersion, IStringVersions> {
		protected override String DefaultValue => String.Empty;
		protected override Func<IStringVersions, DbSet<StringVersion>> VersionDbSet => x => x.StringVersions;
	}
}