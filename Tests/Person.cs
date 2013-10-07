using System;
using System.ComponentModel.DataAnnotations;

namespace EfCodeFirstVersionedProperties.Tests {
    public class Person {
        [Key]
        public Int64 Id { get; protected set; }
        public VersionedString FirstName { get; protected set; }
        public VersionedString LastName { get; protected set; }

        public Person(String firstName, String lastName) {
            FirstName = new VersionedString { Value = firstName };
            LastName = new VersionedString { Value = lastName }; ;
        }
    }
}
