using System;
using System.ComponentModel.DataAnnotations;
using EntityFramework.Triggers;

namespace EntityFramework.VersionedProperties.Tests {
	public abstract class PersonBase {
		[Key]
		public Int64 Id { get; private set; }
	}
    public class Person : PersonBase {
		public DateTime Inserted { get; set; }
		public DateTime Updated { get; set; }
		public VersionedString FirstName { get; private set; } = new VersionedString();
		public VersionedString LastName { get; private set; } = new VersionedString();
		public VersionedDbGeography Location { get; private set; } = new VersionedDbGeography();

		static Person() {
			VersionedProperties<Person>.Initialize();
			Triggers<Person>.Inserting += e => e.Entity.Inserted = e.Entity.Updated = DateTime.Now;
			Triggers<Person>.Updating += e => e.Entity.Updated = DateTime.Now;
		}
    }
}