using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedNullableGuid : VersionedBase<VersionedNullableGuidVersion, Guid?> { }

    public class VersionedNullableGuidVersion : VersionBase<Guid?> {
        public VersionedNullableGuidVersion() : base() { }
        public VersionedNullableGuidVersion(Guid? value) : base(value) { }
        public static implicit operator VersionedNullableGuidVersion(Guid? value) {
            return new VersionedNullableGuidVersion { Value = value };
        }
    }
}