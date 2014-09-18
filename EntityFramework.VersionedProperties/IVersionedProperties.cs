using EntityFrameworkTriggers;

namespace EntityFramework.VersionedProperties {
	public interface IVersionedProperties {}
	public interface IVersionedProperties<TVersionedProperties> : IVersionedProperties, ITriggerable<IVersionedProperties<TVersionedProperties>>, ITriggerable<TVersionedProperties>
		where TVersionedProperties : class, IVersionedProperties<TVersionedProperties>, ITriggerable<IVersionedProperties<TVersionedProperties>>, ITriggerable<TVersionedProperties>, new() { }
}
