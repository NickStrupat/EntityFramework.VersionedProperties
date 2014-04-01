using System;

namespace EntityFrameworkVersionedProperties {
    public class VersionedNullableInt64 : VersionedBase<VersionedNullableInt64Version, Int64?> { }

    public class VersionedNullableInt64Version : VersionBase<Int64?> {
        public VersionedNullableInt64Version() : base() { }
        public VersionedNullableInt64Version(Int64? value) : base(value) { }
        public static implicit operator VersionedNullableInt64Version(Int64? value) {
            return new VersionedNullableInt64Version { Value = value };
        }
    }
}