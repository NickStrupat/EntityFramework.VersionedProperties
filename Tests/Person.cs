using System;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkVersionedProperties.Tests {
    public class Person : IVersionable<Person> {
        [Key]
        public Int64 Id { get; protected set; }
		public VersionedString FirstName { get; protected set; }
		public VersionedString LastName { get; protected set; }
		public VersionedDbGeography Location { get; set; }

		public Person() {
			this.InitializeVersionedProperties();
	    }
        public Person(String firstName, String lastName) : this() {
	        FirstName.Value = firstName;
	        LastName.Value = lastName;
        }
    }
}