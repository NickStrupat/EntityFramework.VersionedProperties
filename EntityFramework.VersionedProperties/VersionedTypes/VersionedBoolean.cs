using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedBoolean : VersionedTypeBase<Boolean, BooleanVersion, IBooleanVersions> {
		protected override Func<IBooleanVersions, DbSet<BooleanVersion>> VersionDbSet {
			get { return x => x.BooleanVersions; }
		}
	}
}