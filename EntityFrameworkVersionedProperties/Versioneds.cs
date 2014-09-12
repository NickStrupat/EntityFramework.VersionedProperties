using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace EntityFrameworkVersionedProperties {
	internal interface IVersioned { }

	public abstract class VersionedBase<T, TVersion> : IVersioned
		where TVersion : VersionBase<T>, new()
	{
		public Guid Id { get; internal set; }
		public DateTime Modified { get; internal set; }
		private T value;
		public virtual T Value {
			get { return value; }
			set {
				if (!(this is NullableVersionedBase<T, TVersion>) && value == null)
					throw new ArgumentNullException("value");
				if (EqualityComparer<T>.Default.Equals(this.value, value))
					return;
				Modified = DateTime.Now;
				if (Id == Guid.Empty)
					Id = Guid.NewGuid();
				else
					Versions.Add(new TVersion { VersionedId = Id, Added = Modified, Value = Value });
				this.value = value;
			}
		}
		protected virtual T DefaultValue {
			get { return default(T); }
		}
		protected VersionedBase() {
			Modified = DateTime.Now;
			value = DefaultValue;
		}
		protected readonly List<TVersion> Versions = new List<TVersion>();
	}

	public abstract class NullableVersionedBase<T, TVersion> : VersionedBase<T, TVersion> where TVersion : VersionBase<T>, new() {}

	[ComplexType]
	public class VersionedString : VersionedBase<String, StringVersion> {
		[Required]
		public override String Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
		protected override String DefaultValue {
			get { return String.Empty; }
		}
	}

	[ComplexType]
	public class VersionedDbGeography : VersionedBase<DbGeography, DbGeographyVersion> {
		[Required]
		public override DbGeography Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
		protected override DbGeography DefaultValue {
			get { return empty; }
		}
		private static readonly DbGeography empty = DbGeography.FromText("POINT EMPTY");
	}

	[ComplexType]
	public class VersionedDbGeometry : VersionedBase<DbGeometry, DbGeometryVersion> {
		[Required]
		public override DbGeometry Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
		protected override DbGeometry DefaultValue {
			get { return empty; }
		}
		private static readonly DbGeometry empty = DbGeometry.FromText("POINT EMPTY");
	}

	[ComplexType]
	public class VersionedBoolean : VersionedBase<Boolean, BooleanVersion> { }
	[ComplexType]
	public class VersionedDateTime : VersionedBase<DateTime, DateTimeVersion> { }
	[ComplexType]
	public class VersionedDateTimeOffset : VersionedBase<DateTimeOffset, DateTimeOffsetVersion> { }
	[ComplexType]
	public class VersionedDecimal : VersionedBase<Decimal, DecimalVersion> { }
	[ComplexType]
	public class VersionedDouble : VersionedBase<Double, DoubleVersion> { }
	[ComplexType]
	public class VersionedGuid : VersionedBase<Guid, GuidVersion> { }
	[ComplexType]
	public class VersionedInt32 : VersionedBase<Int32, Int32Version> { }
	[ComplexType]
	public class VersionedInt64 : VersionedBase<Int64, Int64Version> { }

	[ComplexType]
	public class VersionedNullableBoolean : NullableVersionedBase<Boolean?, NullableBooleanVersion> { }
	[ComplexType]
	public class VersionedNullableDateTime : NullableVersionedBase<DateTime?, NullableDateTimeVersion> { }
	[ComplexType]
	public class VersionedNullableDateTimeOffset : NullableVersionedBase<DateTimeOffset?, NullableDateTimeOffsetVersion> { }
	[ComplexType]
	public class VersionedNullableDbGeography : NullableVersionedBase<DbGeography, NullableDbGeographyVersion> { }
	[ComplexType]
	public class VersionedNullableDbGeometry : NullableVersionedBase<DbGeometry, NullableDbGeometryVersion> { }
	[ComplexType]
	public class VersionedNullableDecimal : NullableVersionedBase<Decimal?, NullableDecimalVersion> { }
	[ComplexType]
	public class VersionedNullableDouble : NullableVersionedBase<Double?, NullableDoubleVersion> { }
	[ComplexType]
	public class VersionedNullableGuid : NullableVersionedBase<Guid?, NullableGuidVersion> { }
	[ComplexType]
	public class VersionedNullableInt32 : NullableVersionedBase<Int32?, NullableInt32Version> { }
	[ComplexType]
	public class VersionedNullableInt64 : NullableVersionedBase<Int64?, NullableInt64Version> { }
	[ComplexType]
	public class VersionedNullableString : NullableVersionedBase<String, NullableStringVersion> { }
}