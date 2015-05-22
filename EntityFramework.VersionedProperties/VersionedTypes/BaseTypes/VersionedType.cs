using System;

namespace EntityFramework.VersionedProperties {
	public abstract class VersionedType {
		internal Action<Object> AddVersionsToDbContextWithVersionedProperties { get; set; }
		internal Action<Object> RemoveVersionsFromDbContextWithVersionedProperties { get; set; }
	}
}