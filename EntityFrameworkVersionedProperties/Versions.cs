using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace EntityFrameworkVersionedProperties {
	public abstract class VersionBase<T> {
		public Int64 Id { get; internal set; }
		[Index]
		public Guid VersionedId { get; internal set; }
		public DateTime Added { get; internal set; }
		public virtual T Value { get; internal set; }
	}

	public class BooleanVersion : VersionBase<Boolean> {}
	public class DateTimeVersion : VersionBase<DateTime> {}
	public class DateTimeOffsetVersion : VersionBase<DateTimeOffset> { }
	public class DbGeographyVersion : VersionBase<DbGeography> {
		[Required]
		public override DbGeography Value { get; internal set; }
	}
	public class DbGeometryVersion : VersionBase<DbGeometry> {
		[Required]
		public override DbGeometry Value { get; internal set; }
	}
	public class DecimalVersion : VersionBase<Decimal> {}
	public class DoubleVersion : VersionBase<Double> {}
	public class GuidVersion : VersionBase<Guid> {}
	public class Int32Version : VersionBase<Int32> {}
	public class Int64Version : VersionBase<Int64> {}
	public class StringVersion : VersionBase<String> {
		[Required]
		public override String Value { get; internal set; }
	}

	public class NullableBooleanVersion : VersionBase<Boolean?> {}
	public class NullableDateTimeVersion : VersionBase<DateTime?> {}
	public class NullableDateTimeOffsetVersion : VersionBase<DateTimeOffset?> { }
	public class NullableDbGeographyVersion : VersionBase<DbGeography> { }
	public class NullableDbGeometryVersion : VersionBase<DbGeometry> {}
	public class NullableDecimalVersion : VersionBase<Decimal?> {}
	public class NullableDoubleVersion : VersionBase<Double?> {}
	public class NullableGuidVersion : VersionBase<Guid> {}
	public class NullableInt32Version : VersionBase<Int32> {}
	public class NullableInt64Version : VersionBase<Int64> {}
	public class NullableStringVersion : VersionBase<String> {}
}
