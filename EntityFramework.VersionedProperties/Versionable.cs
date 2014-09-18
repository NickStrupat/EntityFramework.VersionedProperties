using EntityFrameworkTriggers;

namespace EntityFramework.VersionedProperties {
	class VersionedProperties<T> : IVersionedProperties<T> where T : class, IVersionedProperties<T>, ITriggerable<T>, new() {
		protected VersionedProperties() {
			this.InitializeVersionedProperties();
		} 
	}
}
