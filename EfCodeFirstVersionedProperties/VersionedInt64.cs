using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedInt64 : VersionedBase<VersionedInt64Version, Int64> { }

    public class VersionedInt64Version : VersionBase<Int64> {
        public VersionedInt64Version() : base() { }
        public VersionedInt64Version(Int64 value) : base(value) { }
        public static implicit operator VersionedInt64Version(Int64 value) {
            return new VersionedInt64Version { Value = value };
        }
    }
}