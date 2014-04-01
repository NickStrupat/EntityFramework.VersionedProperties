using System;

namespace EntityFrameworkVersionedProperties {
    public class VersionedNullableDouble : VersionedBase<VersionedNullableDoubleVersion, Double?> { }

    public class VersionedNullableDoubleVersion : VersionBase<Double?> {
        public VersionedNullableDoubleVersion() : base() { }
        public VersionedNullableDoubleVersion(Double? value) : base(value) { }
        public static implicit operator VersionedNullableDoubleVersion(Double? value) {
            return new VersionedNullableDoubleVersion { Value = value };
        }
    }
}