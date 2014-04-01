using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedNullableBoolean : VersionedBase<VersionedNullableBooleanVersion, Boolean?> { }

    public class VersionedNullableBooleanVersion : VersionBase<Boolean?> {
        public VersionedNullableBooleanVersion() : base() { }
        public VersionedNullableBooleanVersion(Boolean? value) : base(value) { }
        public static implicit operator VersionedNullableBooleanVersion(Boolean? value) {
            return new VersionedNullableBooleanVersion { Value = value };
        }
    }
}