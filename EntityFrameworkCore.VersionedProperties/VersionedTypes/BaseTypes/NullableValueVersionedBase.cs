using System;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties {
#else
namespace EntityFramework.VersionedProperties {
#endif
	public abstract class NullableValueVersionedBase<TValue, TVersion, TIVersionedTypes> : VersionedBase<TValue, TVersion, TIVersionedTypes> where TVersion : VersionBase<TValue>, new() {
		internal sealed override Boolean ValueCanBeNull { get; } = true;
	}
}