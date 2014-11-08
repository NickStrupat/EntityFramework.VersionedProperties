using System;
using System.ComponentModel.DataAnnotations;
using EntityFramework.Triggers;

namespace EntityFramework.VersionedProperties.Tests {
    public class Person : IVersionedProperties {
        [Key]
        public Int64 Id { get; protected set; }
		public DateTime Inserted { get; set; }
		public DateTime Updated { get; set; }
		public VersionedString FirstName { get; protected set; }
		public VersionedString LastName { get; protected set; }
		public VersionedDbGeography Location { get; protected set; }

		public Person() {
			this.InitializeVersionedProperties();
			this.Triggers().Inserting += e => e.Entity.Inserted = e.Entity.Updated = DateTime.Now;
			this.Triggers().Updating += e => e.Entity.Updated = DateTime.Now;
		}
    }
}