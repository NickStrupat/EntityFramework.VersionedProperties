using System;

namespace EfCodeFirstVersionedProperties {
    public class VersionedNullableDecimal : VersionedBase<VersionedNullableDecimalVersion, Decimal?> { }
        
    public class VersionedNullableDecimalVersion : VersionBase<Decimal?> {
        public VersionedNullableDecimalVersion() : base() { }
        public VersionedNullableDecimalVersion(Decimal? value) : base(value) { }
        public static implicit operator VersionedNullableDecimalVersion(Decimal? value) {
            return new VersionedNullableDecimalVersion { Value = value };
        }
    }
}