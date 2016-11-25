using System.ComponentModel.DataAnnotations;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
namespace EntityFramework.VersionedProperties {
#endif
	public abstract class VersionedRequiredValueBase<TVersioned, TValue, TVersion, TIVersions> : VersionedBase<TVersioned, TValue, TVersion, TIVersions>
	where TVersioned : VersionedBase<TVersioned, TValue, TVersion, TIVersions>
	where TValue : class
	where TVersion : RequiredValueVersionBase<TValue>, new()
	where TIVersions : class {
		[Required]
		public new TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}
}