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
	public sealed class VersionedString : VersionedBase<String, StringVersion, INullableStringVersions> {
		protected override Func<INullableStringVersions, DbSet<StringVersion>> VersionDbSet => x => x.NullableStringVersions;
	}
}