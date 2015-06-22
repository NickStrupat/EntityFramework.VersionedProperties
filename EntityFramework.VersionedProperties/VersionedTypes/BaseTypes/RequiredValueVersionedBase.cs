using System.ComponentModel.DataAnnotations;

namespace EntityFramework.VersionedProperties {
	public abstract class RequiredValueVersionedBase<TValue, TVersion, TIVersionedTypes> : VersionedBase<TValue, TVersion, TIVersionedTypes> where TVersion : VersionBase<TValue>, new() {
		[Required]
		public override TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}
}