using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedNullableInt32 : VersionedBase<VersionedNullableInt32Version, Int32?> { }

    public class VersionedNullableInt32Version : VersionBase<Int32?> {
        public VersionedNullableInt32Version() : base() { }
        public VersionedNullableInt32Version(Int32? value) : base(value) { }
        public static implicit operator VersionedNullableInt32Version(Int32? value) {
            return new VersionedNullableInt32Version { Value = value };
        }
    }
}