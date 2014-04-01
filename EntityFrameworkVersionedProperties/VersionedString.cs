using System;
using System.ComponentModel.DataAnnotations;

namespace EfCodeFirstVersionedProperties {
    public class VersionedString : VersionedBase<VersionedStringVersion, String> { }

    public class VersionedStringVersion : VersionBase<String> {
        public VersionedStringVersion() : base() { }
        public VersionedStringVersion(String value) : base(value) { }
        public static implicit operator VersionedStringVersion(String value) {
            return new VersionedStringVersion { Value = value };
        }

        // The [Required] attribute tells Entity Framework that we want this column to be non-nullable
        [Required]
        public override String Value { get; internal set; }
    }

    public static class VersionedStringExtensions {
        public static void SetCurrentValue(this VersionedString versionedString, String currentValue) {
            
        }
    }
}