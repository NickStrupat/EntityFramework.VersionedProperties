using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace EntityFramework.VersionedProperties {
#endif
	internal interface IVersion {}

	[DebuggerDisplay("Value = {Value}")]
	public abstract class VersionBase<TValue> : IVersion {
		/// <summary>Gets the unique identifier for this version of the data</summary>
		public Int64 Id { get; internal set; }

		/// <summary>Gets the identifier for the entity which this verion represents</summary>
#if !EF_CORE
		// TODO: check if EF Core has IndexAttribute yet
		[Index]
#endif
		public Guid VersionedId { get; internal set; }

		/// <summary>Gets the date-time representing when this version was first assigned</summary>
		public DateTime Added { get; internal set; }

		/// <summary>Gets the value of this verison of the data</summary>
		public TValue Value { get; internal set; }

		public override String ToString() => Value?.ToString() ?? String.Empty;
	}

	public abstract class RequiredValueVersionBase<TValue> : VersionBase<TValue>
	where TValue : class {
		[Required]
		public new TValue Value {
			get { return base.Value; }
			internal set { base.Value = value; }
		}
	}

	public sealed class BooleanVersion : VersionBase<Boolean> {}
	public sealed class DateTimeVersion : VersionBase<DateTime> {}
	public sealed class DateTimeOffsetVersion : VersionBase<DateTimeOffset> { }
#if !EF_CORE
	public sealed class RequiredDbGeographyVersion : RequiredValueVersionBase<DbGeography> {}
	public sealed class RequiredDbGeometryVersion : RequiredValueVersionBase<DbGeometry> {}
#endif
	public sealed class DecimalVersion : VersionBase<Decimal> { }
	public sealed class DoubleVersion : VersionBase<Double> {}
	public sealed class GuidVersion : VersionBase<Guid> {}
	public sealed class Int32Version : VersionBase<Int32> {}
	public sealed class Int64Version : VersionBase<Int64> {}
	public sealed class RequiredStringVersion : RequiredValueVersionBase<String> {}

	public sealed class NullableBooleanVersion : VersionBase<Boolean?> {}
	public sealed class NullableDateTimeVersion : VersionBase<DateTime?> {}
	public sealed class NullableDateTimeOffsetVersion : VersionBase<DateTimeOffset?> { }
#if !EF_CORE
	public sealed class DbGeographyVersion : VersionBase<DbGeography> { }
	public sealed class DbGeometryVersion : VersionBase<DbGeometry> {}
#endif
	public sealed class NullableDecimalVersion : VersionBase<Decimal?> { }
	public sealed class NullableDoubleVersion : VersionBase<Double?> {}
	public sealed class NullableGuidVersion : VersionBase<Guid?> {}
	public sealed class NullableInt32Version : VersionBase<Int32?> {}
	public sealed class NullableInt64Version : VersionBase<Int64?> {}
	public sealed class StringVersion : VersionBase<String> {}
}