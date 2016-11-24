using System;
using System.ComponentModel.DataAnnotations;

#if EF_CORE
using EntityFrameworkCore.Triggers;
namespace EntityFrameworkCore.VersionedProperties.Tests {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties.Tests {
#endif
	public abstract class PersonBase {
		[Key]
		public Int64 Id { get; private set; }
		public DateTime Inserted { get; private set; }
		public DateTime Updated { get; private set; }

		static PersonBase() {
			Triggers<PersonBase>.Inserting += e => e.Entity.Inserted = e.Entity.Updated = DateTime.UtcNow;
			Triggers<PersonBase>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
		}
	}
	public class Person : PersonBase {
		public VersionedString FirstName { get; private set; } = new VersionedString();
		public VersionedString LastName { get; private set; } = new VersionedString();
#if !EF_CORE
		public VersionedDbGeography Location { get; private set; }
#endif
		static Person() {
			VersionedProperties<Person>.Initialize();
		}
	}
}