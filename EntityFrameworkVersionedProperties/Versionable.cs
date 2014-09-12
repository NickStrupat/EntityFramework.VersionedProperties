using System.Data.Entity;
using EntityFrameworkTriggers;

namespace EntityFrameworkVersionedProperties {
	class Versionable<T, TDbContext> : IVersionable<T, TDbContext> where T : class, IVersionable<T, TDbContext>, ITriggerable<T>, new() where TDbContext : DbContext {
		protected Versionable() {
			this.InitializeVersionedProperties();
		} 
	}
}
