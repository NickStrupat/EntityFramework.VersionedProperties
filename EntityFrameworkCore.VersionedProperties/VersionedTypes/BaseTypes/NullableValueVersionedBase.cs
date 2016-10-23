using System;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
namespace EntityFramework.VersionedProperties {
#endif
	public abstract class NullableValueVersionedBase<T, TVersion, TIVersionedTypes> : VersionedBase<T, TVersion, TIVersionedTypes> where TVersion : VersionBase<T>, new() {
		internal sealed override Boolean ValueCanBeNull { get; } = true;
	}
}