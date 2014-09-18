using EntityFramework.Triggers;

namespace EntityFramework.VersionedProperties {
	/// <summary>
	/// Used internally by the library. Do not inherit this interface in any calling code.
	/// </summary>
	public interface IVersionedProperties {}

	/// <summary>
	/// Inherit from this interface to support versioned properties and triggers in your derived class. You must call <c>this.InitializeVersionedProperties();</c> before accessing the versioned properties. Alternatively, inherit from <c>VersionedProperties&lt;T&gt;</c> which handles initialization automatically.
	/// </summary>
	/// <typeparam name="TVersionedProperties"></typeparam>
	public interface IVersionedProperties<TVersionedProperties> : IVersionedProperties, ITriggerable<IVersionedProperties<TVersionedProperties>>, ITriggerable<TVersionedProperties>
		where TVersionedProperties : class, IVersionedProperties<TVersionedProperties>, ITriggerable<IVersionedProperties<TVersionedProperties>>, ITriggerable<TVersionedProperties>, new() { }
}
