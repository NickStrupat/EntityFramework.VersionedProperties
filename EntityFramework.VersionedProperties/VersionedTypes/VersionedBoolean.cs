using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedBoolean : VersionedBase<Boolean, BooleanVersion, IBooleanVersions> {
		protected override Func<IBooleanVersions, DbSet<BooleanVersion>> VersionDbSet => x => x.BooleanVersions;
	}
}