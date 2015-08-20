using System;

namespace EntityFramework.VersionedProperties {
	public abstract class NullableVersionedBase<T, TVersion, TIVersionedTypes> : VersionedBase<T, TVersion, TIVersionedTypes> where TVersion : VersionBase<T>, new() {
		internal sealed override Boolean ValueCanBeNull { get; } = true;
	}
}