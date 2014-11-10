using EntityFramework.Triggers;

namespace EntityFramework.VersionedProperties {
	/// <summary>
	/// Implement this interface to support versioned properties and triggers in your entity class. You must call <see cref="Extensions.InitializeVersionedProperties{TVersionedProperties}"/> before accessing the versioned properties. Alternatively, inherit from <c>VersionedProperties&lt;T&gt;</c> which handles initialization automatically.
	/// </summary>
	public interface IVersionedProperties : ITriggerable {}
}
