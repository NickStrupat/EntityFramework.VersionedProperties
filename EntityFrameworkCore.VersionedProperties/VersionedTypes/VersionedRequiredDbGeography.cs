using System;
using System.ComponentModel.DataAnnotations.Schema;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
#if !EF_CORE
	[ComplexType]
	public sealed class VersionedRequiredDbGeography : VersionedRequiredValueBase<DbGeography, RequiredDbGeographyVersion, IRequiredDbGeographyVersions> {
		protected override Func<IRequiredDbGeographyVersions, DbSet<RequiredDbGeographyVersion>> VersionDbSet => x => x.RequiredDbGeographyVersions;
	}
#endif
}