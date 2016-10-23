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
	public class VersionedNullableDouble : NullableValueVersionedBase<Double?, NullableDoubleVersion, INullableDoubleVersions> {
		protected override Func<INullableDoubleVersions, DbSet<NullableDoubleVersion>> VersionDbSet => x => x.NullableDoubleVersions;
	}
}