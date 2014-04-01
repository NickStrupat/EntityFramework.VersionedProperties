using System;

namespace EntityFrameworkVersionedProperties {
    public class VersionedNullableDateTime : VersionedBase<VersionedNullableDateTimeVersion, DateTime?> { }

    public class VersionedNullableDateTimeVersion : VersionBase<DateTime?> {
        public VersionedNullableDateTimeVersion() : base() { }
        public VersionedNullableDateTimeVersion(DateTime? value) : base(value) { }
        public static implicit operator VersionedNullableDateTimeVersion(DateTime? value) {
            return new VersionedNullableDateTimeVersion { Value = value };
        }
    }
}