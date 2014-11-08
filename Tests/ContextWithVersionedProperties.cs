using System.Data.Entity;

namespace EntityFramework.VersionedProperties.Tests {
    public class Context : DbContextWithVersionedProperties {
        public DbSet<Person> People { get; set; }
    }
}
