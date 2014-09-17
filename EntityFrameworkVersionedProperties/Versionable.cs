using EntityFrameworkTriggers;

namespace EntityFrameworkVersionedProperties {
	class VersionedProperties<T> : IVersionedProperties<T> where T : class, IVersionedProperties<T>, ITriggerable<T>, new() {
		protected VersionedProperties() {
			this.InitializeVersionedProperties();
		} 
	}
}
