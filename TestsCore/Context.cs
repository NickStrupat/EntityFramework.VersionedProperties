#if EF_CORE
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.VersionedProperties.Tests {
#else
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace EntityFramework.VersionedProperties.Tests {
#endif
	public class Context : DbContextWithVersionedProperties {
        public DbSet<Person> People { get; set; }

#if EF_CORE
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database={GetType().FullName};Trusted_Connection=True;");
		}
#endif
	}
#if !EF_CORE
	internal sealed class Configuration : DbMigrationsConfiguration<Context> {
		public Configuration() {
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
		}
	}
#endif
}