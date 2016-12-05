#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	internal interface IVersioned {
		void OnInsertingOrUpdating(DbContext dbContext);
		void OnInsertedOrUpdated();
		void OnDeleted(DbContext dbContext);
		void SetSnapshotVersion(IVersion version);
	}
}