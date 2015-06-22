using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedInt32 : VersionedBase<Int32, Int32Version, IInt32Versions> {
		protected override Func<IInt32Versions, DbSet<Int32Version>> VersionDbSet {
			get { return x => x.Int32Versions; }
		}
	}
}