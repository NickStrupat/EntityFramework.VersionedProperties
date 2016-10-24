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
	public sealed class VersionedDouble : VersionedBase<Double, DoubleVersion, IDoubleVersions> {
		protected override Func<IDoubleVersions, DbSet<DoubleVersion>> VersionDbSet => x => x.DoubleVersions;
	}
}