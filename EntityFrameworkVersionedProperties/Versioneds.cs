using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace EntityFrameworkVersionedProperties {
	internal interface IVersioned { }

	public abstract class VersionedBase<T> : IVersioned {
		public Guid Id { get; internal set; }
		public DateTime Modified { get; internal set; }
		private T value;
		public virtual T Value {
			get { return value; }
			set {
				if (!(this is NullableVersionedBase<T>) && value == null)
					throw new ArgumentNullException("value");
				if (Id == Guid.Empty)
					Id = Guid.NewGuid();
				if (EqualityComparer<T>.Default.Equals(this.value, value))
					return;
				Modified = DateTime.Now;
				this.value = value;
			}
		}
		protected virtual T DefaultValue {
			get { return default(T); }
		}
		protected VersionedBase() {
			value = DefaultValue;
		}
		internal Action VerionAction;
	}
	public abstract class NullableVersionedBase<T> : VersionedBase<T> {
	}

	[ComplexType]
	public class VersionedString : VersionedBase<String> {
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
	public class VersionedDbGeography : VersionedBase<DbGeography> {
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
	public class VersionedDbGeometry : VersionedBase<DbGeometry> {
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
	public class VersionedBoolean : VersionedBase<Boolean> { }
	[ComplexType]
	public class VersionedDateTime : VersionedBase<DateTime> { }
	[ComplexType]
	public class VersionedDateTimeOffset : VersionedBase<DateTimeOffset> { }
	[ComplexType]
	public class VersionedDecimal : VersionedBase<Decimal> { }
	[ComplexType]
	public class VersionedDouble : VersionedBase<Double> { }
	[ComplexType]
	public class VersionedGuid : VersionedBase<Guid> { }
	[ComplexType]
	public class VersionedInt32 : VersionedBase<Int32> { }
	[ComplexType]
	public class VersionedInt64 : VersionedBase<Int64> { }

	[ComplexType]
	public class VersionedNullableBoolean : NullableVersionedBase<Boolean?> { }
	[ComplexType]
	public class VersionedNullableDateTime : NullableVersionedBase<DateTime?> { }
	[ComplexType]
	public class VersionedNullableDateTimeOffset : NullableVersionedBase<DateTimeOffset?> { }
	[ComplexType]
	public class VersionedNullableDbGeometry : NullableVersionedBase<DbGeometry> { }
	[ComplexType]
	public class VersionedNullableDecimal : NullableVersionedBase<Decimal?> { }
	[ComplexType]
	public class VersionedNullableDouble : NullableVersionedBase<Double?> { }
	[ComplexType]
	public class VersionedNullableGuid : NullableVersionedBase<Guid?> { }
	[ComplexType]
	public class VersionedNullableInt32 : NullableVersionedBase<Int32?> { }
	[ComplexType]
	public class VersionedNullableInt64 : NullableVersionedBase<Int64?> { }
	[ComplexType]
	public class VersionedNullableString : NullableVersionedBase<String> { }
}