using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedBoolean : VersionedBase<VersionedBooleanVersion, Boolean> { }

    public class VersionedBooleanVersion : VersionBase<Boolean> {
        public VersionedBooleanVersion() : base() { }
        public VersionedBooleanVersion(Boolean value) : base(value) { }
        public static implicit operator VersionedBooleanVersion(Boolean value) {
            return new VersionedBooleanVersion { Value = value };
        }
    }
}