using System.Data.Entity;
using EntityFrameworkTriggers;

namespace EntityFrameworkVersionedProperties {
	public interface IVersionable {}
	public interface IVersionable<TVersionable, TDbContext> : IVersionable, ITriggerable<IVersionable<TVersionable, TDbContext>>, ITriggerable<TVersionable>
		where TVersionable : class, IVersionable<TVersionable, TDbContext>, ITriggerable<IVersionable<TVersionable, TDbContext>>, ITriggerable<TVersionable>, new()
		where TDbContext : DbContext { }
}
