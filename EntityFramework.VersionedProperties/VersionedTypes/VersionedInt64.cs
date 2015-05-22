using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedInt64 : VersionedTypeBase<Int64, Int64Version, IInt64Versions> {
		protected override Func<IInt64Versions, DbSet<Int64Version>> VersionDbSet {
			get { return x => x.Int64Versions; }
		}
	}
}