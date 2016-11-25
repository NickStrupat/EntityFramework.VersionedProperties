using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace EntityFramework.VersionedProperties.Tests {
    public class Context : DbContextWithVersionedProperties {
        public DbSet<Person> People { get; set; }
	}

	internal sealed class Configuration : DbMigrationsConfiguration<Context> {
		public Configuration() {
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
		}
	}
}
