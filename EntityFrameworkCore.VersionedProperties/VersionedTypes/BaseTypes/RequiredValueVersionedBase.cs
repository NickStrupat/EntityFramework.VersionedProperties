using System.ComponentModel.DataAnnotations;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
namespace EntityFramework.VersionedProperties {
#endif
	public abstract class RequiredValueVersionedBase<TValue, TVersion, TIVersionedTypes> : VersionedBase<TValue, TVersion, TIVersionedTypes> where TVersion : VersionBase<TValue>, new() {
		[Required]
		public sealed override TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}
}