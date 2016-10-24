using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
using System.Data.Entity.Spatial;
namespace EntityFramework.VersionedProperties {
#endif
	[ComplexType]
	public sealed class VersionedNullableInt32 : NullableValueVersionedBase<Int32?, NullableInt32Version, INullableInt32Versions> {
		protected override Func<INullableInt32Versions, DbSet<NullableInt32Version>> VersionDbSet => x => x.NullableInt32Versions;
	}
}