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
	public sealed class VersionedBoolean : VersionedBase<Boolean, BooleanVersion, IBooleanVersions> {
		protected override Func<IBooleanVersions, DbSet<BooleanVersion>> VersionDbSet => x => x.BooleanVersions;
	}
}