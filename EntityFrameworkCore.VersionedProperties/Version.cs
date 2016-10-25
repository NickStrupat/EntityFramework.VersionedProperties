using System;
using System.ComponentModel.DataAnnotations;

#if EF_CORE
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Triggers;
namespace EntityFrameworkCore.VersionedProperties {
#else
using System.Data.Entity;
namespace EntityFramework.VersionedProperties {
#endif
	internal interface IVersion {}

	public abstract class VersionBase<TValue> : IVersion {
		public Int64 Id { get; internal set; }
#if !EF_CORE
		// TODO: check it EF Core has IndexAttribute yet
		[Index]
#endif
		public Guid VersionedId { get; internal set; }
		public DateTime Added { get; internal set; }
		public virtual TValue Value { get; internal set; }
	}

	public abstract class RequiredValueVersionBase<TValue> : VersionBase<TValue>
	where TValue : class {
		[Required]
		public override TValue Value {
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