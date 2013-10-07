using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedDateTime : VersionedBase<VersionedDateTimeVersion, DateTime> { }

    public class VersionedDateTimeVersion : VersionBase<DateTime> {
        public VersionedDateTimeVersion() : base() { }
        public VersionedDateTimeVersion(DateTime value) : base(value) { }
        public static implicit operator VersionedDateTimeVersion(DateTime value) {
            return new VersionedDateTimeVersion { Value = value };
        }
    }
}