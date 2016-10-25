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
	public sealed class VersionedNullableBoolean : VersionedBase<Boolean?, NullableBooleanVersion, INullableBooleanVersions> {
		protected override Func<INullableBooleanVersions, DbSet<NullableBooleanVersion>> VersionDbSet => x => x.NullableBooleanVersions;
	}
}