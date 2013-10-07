using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedDouble : VersionedBase<VersionedDoubleVersion, Double> { }

    public class VersionedDoubleVersion : VersionBase<Double> {
        public VersionedDoubleVersion() : base() { }
        public VersionedDoubleVersion(Double value) : base(value) { }
        public static implicit operator VersionedDoubleVersion(Double value) {
            return new VersionedDoubleVersion { Value = value };
        }
    }
}