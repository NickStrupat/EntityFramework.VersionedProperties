using System;
using System.ComponentModel.DataAnnotations;
using EntityFramework.Triggers;

namespace EntityFramework.VersionedProperties.Tests {
	public abstract class PersonBase : IVersionedProperties {
		[Key]
		public Int64 Id { get; private set; }

		protected PersonBase() {
			this.InitializeVersionedProperties();
		}
	}
    public class Person : PersonBase {
		public DateTime Inserted { get; set; }
		public DateTime Updated { get; set; }
		public VersionedString FirstName { get; private set; }
		public VersionedString LastName { get; private set; }
		public VersionedDbGeography Location { get; private set; }

		public Person() {
			this.Triggers().Inserting += e => e.Entity.Inserted = e.Entity.Updated = DateTime.Now;
			this.Triggers().Updating += e => e.Entity.Updated = DateTime.Now;
		}
    }
}