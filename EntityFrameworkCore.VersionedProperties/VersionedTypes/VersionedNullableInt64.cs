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
	public sealed class VersionedNullableInt64 : VersionedNullableValueBase<Int64, NullableInt64Version, INullableInt64Versions> {
		protected override Func<INullableInt64Versions, DbSet<NullableInt64Version>> VersionDbSet => x => x.NullableInt64Versions;
	}
}