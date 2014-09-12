namespace EntityFrameworkVersionedProperties {
	class Versionable<T> : IVersionable<T> {
		protected Versionable() {
			this.InitializeVerionedProperties();
		} 
	}
}
