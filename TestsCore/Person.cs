using System;
using System.ComponentModel.DataAnnotations;

#if EF_CORE
using EntityFrameworkCore.Triggers;
namespace EntityFrameworkCore.VersionedProperties.Tests {
#else
using System.Data.Entity;
using EntityFramework.Triggers;
namespace EntityFramework.VersionedProperties.Tests {
#endif
	public abstract class InsertUpdateStamps {
		public virtual DateTime Inserted { get; private set; }
		public virtual DateTime Updated { get; private set; }

		static InsertUpdateStamps() {
			Triggers<InsertUpdateStamps>.Inserting += e => e.Entity.Inserted = e.Entity.Updated = DateTime.UtcNow;
			Triggers<InsertUpdateStamps>.Updating += e => e.Entity.Updated = DateTime.UtcNow;
		}
	}
	public class Person : InsertUpdateStamps {
		[Key]
		public virtual Int64 Id { get; private set; }
		public virtual VersionedString FirstName { get; private set; } = new VersionedString();
		public virtual VersionedString LastName { get; private set; } = new VersionedString();
#if !EF_CORE
		public virtual VersionedDbGeography Location { get; private set; } = new VersionedDbGeography();
#endif
		static Person() {
			VersionedProperties<Person>.Initialize();
		}
	}
}