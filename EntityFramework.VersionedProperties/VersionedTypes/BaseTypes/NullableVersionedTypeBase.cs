namespace EntityFramework.VersionedProperties {
	public abstract class NullableVersionedTypeBase<T, TVersion, TIVersionedTypes> : VersionedTypeBase<T, TVersion, TIVersionedTypes> where TVersion : VersionBase<T>, new() { }
}