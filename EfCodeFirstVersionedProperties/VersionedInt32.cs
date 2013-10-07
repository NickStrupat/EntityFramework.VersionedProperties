using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedInt32 : VersionedBase<VersionedInt32Version, Int32> { }

    public class VersionedInt32Version : VersionBase<Int32> {
        public VersionedInt32Version() : base() { }
        public VersionedInt32Version(Int32 value) : base(value) { }
        public static implicit operator VersionedInt32Version(Int32 value) {
            return new VersionedInt32Version { Value = value };
        }
    }
}