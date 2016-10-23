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
	public class VersionedNullableString : NullableValueVersionedBase<String, NullableStringVersion, INullableStringVersions> {
		protected override Func<INullableStringVersions, DbSet<NullableStringVersion>> VersionDbSet => x => x.NullableStringVersions;
	}
}