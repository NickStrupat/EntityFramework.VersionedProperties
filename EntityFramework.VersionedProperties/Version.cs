using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	internal interface IVersion {}
	public abstract class VersionBase<TValue> : IVersion {
		public Int64 Id { get; internal set; }
		[Index]
		public Guid VersionedId { get; internal set; }
		public DateTime Added { get; internal set; }
		public virtual TValue Value { get; internal set; }
	}

	public sealed class BooleanVersion : VersionBase<Boolean> {}
	public sealed class DateTimeVersion : VersionBase<DateTime> {}
	public sealed class DateTimeOffsetVersion : VersionBase<DateTimeOffset> { }
	public sealed class DbGeographyVersion : VersionBase<DbGeography> {
		[Required]
		public override DbGeography Value { get; internal set; }
	}
	public sealed class DbGeometryVersion : VersionBase<DbGeometry> {
		[Required]
		public override DbGeometry Value { get; internal set; }
	}
	public sealed class DecimalVersion : VersionBase<Decimal> {}
	public sealed class DoubleVersion : VersionBase<Double> {}
	public sealed class GuidVersion : VersionBase<Guid> {}
	public sealed class Int32Version : VersionBase<Int32> {}
	public sealed class Int64Version : VersionBase<Int64> {}
	public sealed class StringVersion : VersionBase<String> {
		[Required]
		public override String Value { get; internal set; }
	}

	public sealed class NullableBooleanVersion : VersionBase<Boolean?> {}
	public sealed class NullableDateTimeVersion : VersionBase<DateTime?> {}
	public sealed class NullableDateTimeOffsetVersion : VersionBase<DateTimeOffset?> { }
	public sealed class NullableDbGeographyVersion : VersionBase<DbGeography> { }
	public sealed class NullableDbGeometryVersion : VersionBase<DbGeometry> {}
	public sealed class NullableDecimalVersion : VersionBase<Decimal?> {}
	public sealed class NullableDoubleVersion : VersionBase<Double?> {}
	public sealed class NullableGuidVersion : VersionBase<Guid?> {}
	public sealed class NullableInt32Version : VersionBase<Int32?> {}
	public sealed class NullableInt64Version : VersionBase<Int64?> {}
	public sealed class NullableStringVersion : VersionBase<String> {}
}
