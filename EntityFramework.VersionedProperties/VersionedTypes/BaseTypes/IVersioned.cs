using System;

namespace EntityFramework.VersionedProperties {
	internal interface IVersioned {
		void AddVersionsToDbContext(Object dbContext);
		void RemoveVersionsFromDbContext(Object dbContext);
	}
}