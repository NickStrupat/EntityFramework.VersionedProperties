using System.ComponentModel.DataAnnotations;

namespace EntityFramework.VersionedProperties {
	public abstract class RequiredValueVersionedTypeBase<TValue, TVersion, TIVersionedTypes> : VersionedTypeBase<TValue, TVersion, TIVersionedTypes> where TVersion : VersionBase<TValue>, new() {
		[Required]
		public override TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}
}