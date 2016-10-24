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
	public sealed class VersionedNullableGuid : VersionedNullableValueBase<Guid, NullableGuidVersion, INullableGuidVersions> {
		protected override Func<INullableGuidVersions, DbSet<NullableGuidVersion>> VersionDbSet => x => x.NullableGuidVersions;
	}
}