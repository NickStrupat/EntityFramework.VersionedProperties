using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
namespace EntityFramework.VersionedProperties {
#endif
	public abstract class VersionedRequiredValueBase<TVersioned, TValue, TVersion, TIVersions> : VersionedBase<TVersioned, TValue, TVersion, TIVersions>
	where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>, new()
	where TValue : class
	where TVersion : RequiredValueVersionBase<TValue>, new()
	where TIVersions : class {
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Required]
		public new TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}
}