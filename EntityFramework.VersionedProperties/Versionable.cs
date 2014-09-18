using EntityFramework.Triggers;

namespace EntityFramework.VersionedProperties {
	/// <summary>
	/// Inherit from this class to support versioned properties and triggers in your derived class. Alternatively, inherit from <c>IVersionedProperties&lt;T&gt;</c> and call <c>this.InitializeVersionedProperties();</c> in the constructor
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class VersionedProperties<T> : IVersionedProperties<T> where T : class, IVersionedProperties<T>, ITriggerable<T>, new() {
		protected VersionedProperties() {
			this.InitializeVersionedProperties();
		} 
	}
}
