using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;

namespace EntityFrameworkVersionedProperties.Tests {
    public class Person : IVersionable<Person> {
        [Key]
        public Int64 Id { get; protected set; }
		public VersionedString FirstName { get; protected set; }
		public VersionedString LastName { get; protected set; }
		public VersionedDbGeometry Location { get; set; }

		public Person() {
			this.InitializeVerionedProperties();
	    }
        public Person(String firstName, String lastName) : this() {
	        FirstName.Value = firstName;
	        LastName.Value = lastName;
        }
    }
}