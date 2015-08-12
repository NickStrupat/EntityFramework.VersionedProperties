using System;
using System.Data.Entity;

namespace EntityFramework.VersionedProperties {
	internal interface IVersioned {
		void AddVersionsToDbContextWithVersionedProperties(Object dbContext);
		void RemoveVersionsFromDbContextWithVersionedProperties(Object dbContext);
		//Action<DbContext> AddVersionsToDbContextWithVersionedProperties { get; set; }
		//Action<DbContext> RemoveVersionsFromDbContextWithVersionedProperties { get; set; }
	}
}