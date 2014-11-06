using System;
using EntityFramework.Triggers;

namespace EntityFramework.VersionedProperties {
	/// <summary>
	/// Inherit from this interface to support versioned properties and triggers in your derived class. You must call <c>this.InitializeVersionedProperties();</c> before accessing the versioned properties. Alternatively, inherit from <c>VersionedProperties&lt;T&gt;</c> which handles initialization automatically.
	/// </summary>
	public interface IVersionedProperties : ITriggerable {}
}
