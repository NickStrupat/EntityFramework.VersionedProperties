using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedNullableString : VersionedBase<VersionedNullableStringVersion, String> { }

    public class VersionedNullableStringVersion : VersionBase<String> {
        public VersionedNullableStringVersion() : base() { }
        public VersionedNullableStringVersion(String value) : base(value) { }
        public static implicit operator VersionedNullableStringVersion(String value) {
            return new VersionedNullableStringVersion { Value = value };
        }
    }
}