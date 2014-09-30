using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace EntityFramework.VersionedProperties {
	internal interface IVersioned { }

	public abstract class VersionedBase<TValue, TVersion> : IVersioned
		where TVersion : VersionBase<TValue>, new()
	{
		public Guid Id { get; internal set; }
		public DateTime Modified { get; internal set; }
		private TValue value;
		public virtual TValue Value {
			get { return value; }
			set {
				if (!(this is NullableVersionedBase<TValue, TVersion>) && value == null)
					throw new ArgumentNullException("value");
				if (EqualityComparer<TValue>.Default.Equals(this.value, value))
					return;
				Modified = DateTime.Now;
				if (Id == Guid.Empty)
					Id = Guid.NewGuid();
				else
					Versions.Add(new TVersion { VersionedId = Id, Added = Modified, Value = Value });
				this.value = value;
			}
		}
		public sealed override String ToString() {
			return Value.ToString();
		}
		protected virtual TValue DefaultValue {
			get { return default(TValue); }
		}
		protected VersionedBase() {
			Modified = DateTime.Now;
			value = DefaultValue;
		}
		internal readonly List<TVersion> Versions = new List<TVersion>();
	}

	public abstract class NullableVersionedBase<T, TVersion> : VersionedBase<T, TVersion> where TVersion : VersionBase<T>, new() {}

	public abstract class RequiredValueVersionedBase<TValue, TVersion> : VersionedBase<TValue, TVersion> where TVersion : VersionBase<TValue>, new() {
		[Required]
		public override TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}

	[ComplexType]
	public class VersionedString : RequiredValueVersionedBase<String, StringVersion> {
		protected override String DefaultValue {
			get { return String.Empty; }
		}
	}

	[ComplexType]
	public class VersionedDbGeography : RequiredValueVersionedBase<DbGeography, DbGeographyVersion> {
		protected override DbGeography DefaultValue {
			get { return DbGeography.FromText("POINT EMPTY"); }
		}
	}

	[ComplexType]
	public class VersionedDbGeometry : RequiredValueVersionedBase<DbGeometry, DbGeometryVersion> {
		protected override DbGeometry DefaultValue {
			get { return DbGeometry.FromText("POINT EMPTY"); }
		}
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