using System.ComponentModel.DataAnnotations;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
namespace EntityFramework.VersionedProperties {
#endif
	public abstract class VersionedRequiredValueBase<TValue, TVersion, TIVersions> : VersionedBase<TValue, TVersion, TIVersions>
	where TValue : class
	where TVersion : RequiredValueVersionBase<TValue>, new() {
		[Required]
		public new TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}
}