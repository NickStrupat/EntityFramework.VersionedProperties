using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedDecimal : VersionedBase<VersionedDecimalVersion, Decimal> { }

    public class VersionedDecimalVersion : VersionBase<Decimal> {
        public VersionedDecimalVersion() : base() { }
        public VersionedDecimalVersion(Decimal value) : base(value) { }
        public static implicit operator VersionedDecimalVersion(Decimal value) {
            return new VersionedDecimalVersion { Value = value };
        }
    }
}