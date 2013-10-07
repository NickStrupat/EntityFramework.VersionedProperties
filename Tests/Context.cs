using System.Data.Entity;

namespace EfCodeFirstVersionedProperties.Tests {
    public class Context : DbContext {
        public DbSet<Person> People { get; set; }
    }
}
