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
	public sealed class VersionedRequiredString : VersionedRequiredValueBase<String, RequiredStringVersion, IStringVersions> {
		protected override String DefaultValue => String.Empty;
		protected override Func<IStringVersions, DbSet<RequiredStringVersion>> VersionDbSet => x => x.StringVersions;
	}
}