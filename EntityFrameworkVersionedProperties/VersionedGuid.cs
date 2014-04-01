using System;

namespace EntityFrameworkVersionedProperties {
    public class VersionedGuid : VersionedBase<VersionedGuidVersion, Guid> { }

    public class VersionedGuidVersion : VersionBase<Guid> {
        public VersionedGuidVersion() : base() { }
        public VersionedGuidVersion(Guid value) : base(value) { }
        public static implicit operator VersionedGuidVersion(Guid value) {
            return new VersionedGuidVersion { Value = value };
        }
    }
}