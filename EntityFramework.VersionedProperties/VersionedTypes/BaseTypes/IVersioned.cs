using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	internal interface IVersioned {
		void AddVersionsToDbContext(DbContext dbContext);
		void RemoveVersionsFromDbContext(DbContext dbContext);
		void SetIsDefaultValueFalse();
	}
}