using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	[ComplexType]
	public class VersionedGuid : VersionedBase<Guid, GuidVersion, IGuidVersions> {
		protected override Func<IGuidVersions, DbSet<GuidVersion>> VersionDbSet => x => x.GuidVersions;
	}
}