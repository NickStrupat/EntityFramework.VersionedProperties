using EntityFrameworkTriggers;

namespace EntityFrameworkVersionedProperties {
	public interface IVersionable {}
	public interface IVersionable<TVersionable> : IVersionable, ITriggerable<IVersionable<TVersionable>>, ITriggerable<TVersionable>
		where TVersionable : class, IVersionable<TVersionable>, ITriggerable<IVersionable<TVersionable>>, ITriggerable<TVersionable>, new() { }
}
