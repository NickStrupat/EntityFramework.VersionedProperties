using System.Data.Entity;

namespace EntityFrameworkVersionedProperties.Tests {
    public class Context : DbContext {
        public DbSet<Person> People { get; set; }
    }
}
