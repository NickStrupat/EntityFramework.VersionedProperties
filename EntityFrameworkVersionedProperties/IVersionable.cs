using EntityFrameworkTriggers;

namespace EntityFrameworkVersionedProperties {
	public interface IVersionable {}
	public interface IVersionable<TVersionable> : IVersionable, ITriggerable<IVersionable<TVersionable>> { }
}
