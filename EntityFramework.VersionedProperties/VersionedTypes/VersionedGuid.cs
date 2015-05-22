using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedGuid : VersionedTypeBase<Guid, GuidVersion, IGuidVersions> {
		protected override Func<IGuidVersions, DbSet<GuidVersion>> VersionDbSet {
			get { return x => x.GuidVersions; }
		}
	}
}