using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	[ComplexType]
	public class VersionedDouble : VersionedTypeBase<Double, DoubleVersion, IDoubleVersions> {
		protected override Func<IDoubleVersions, DbSet<DoubleVersion>> VersionDbSet {
			get { return x => x.DoubleVersions; }
		}
	}
}