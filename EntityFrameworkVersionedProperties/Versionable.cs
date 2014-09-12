using EntityFrameworkTriggers;

namespace EntityFrameworkVersionedProperties {
	class Versionable<T> : IVersionable<T> where T : class, IVersionable<T>, ITriggerable<T>, new() {
		protected Versionable() {
			this.InitializeVersionedProperties();
		} 
	}
}
