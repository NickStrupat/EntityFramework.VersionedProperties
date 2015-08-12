using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedGuid : VersionedBase<Guid, GuidVersion, IGuidVersions> {
		protected override Func<IGuidVersions, DbSet<GuidVersion>> VersionDbSet => x => x.GuidVersions;
	}
}