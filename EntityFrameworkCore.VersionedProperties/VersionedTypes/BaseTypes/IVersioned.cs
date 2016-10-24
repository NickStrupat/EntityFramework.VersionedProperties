#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	internal interface IVersioned {
		void AddVersionsToDbContext(DbContext dbContext);
		void RemoveVersionsFromDbContext(DbContext dbContext);
		void ClearInternalLocalVersions();
		void SetIsDefaultValueFalse();
	}
}