EntityFramework.VersionedProperties
==================================

NuGet package listed on nuget.org at https://www.nuget.org/packages/EntityFramework.VersionedProperties/ [![NuGet Status](http://img.shields.io/nuget/v/EntityFramework.VersionedProperties.svg?style=flat)](https://www.nuget.org/packages/EntityFramework.VersionedProperties/)

VersionedProperties is a library for .NET developers using Entity Framework which makes it very easy to support fine-grained (down to the property) data versioning. Triggers are inherently supported as this project depends on some of the underlying features of https://github.com/NickStrupat/EntityFramework.Triggers.

Versioned property types are included for most primitive types which are mappable by Entity Framework. Extending the set to support your own custom types is demonstrated in the usage example below.

## Basic requirements

Your entity classes which contain any versioned properties must inherit from `IVersionedProperties` and call `this.InitializeVersionedProperties()` before any versioned properties are used (i.e. in the constructor).

Your `DbContext` must inherit from `IDbContextWithVersionedProperties` and implement the `DbSet`s and `SaveChanges(Async)` overrides. If you have control of your `DbContext` inheritance chain, the helper class `DbContextWithVersionedProperties` implements the required `DbSet`s and `SaveChanges(Async)` overrides for you.

When accessing your versioned property, the `Value` property represents the current value of your versioned type. Please see the usage example below.

## Built-in versioned property types

- VersionedBoolean
- VersionedDateTime
- VersionedDateTimeOffset
- VersionedDbGeography
- VersionedDbGeometry
- VersionedDecimal
- VersionedDouble
- VersionedGuid
- VersionedInt32
- VersionedInt64
- VersionedString
- VersionedNullableBoolean
- VersionedNullableDateTime
- VersionedNullableDateTimeOffset
- VersionedNullableDbGeography
- VersionedNullableDbGeometry
- VersionedNullableDecimal
- VersionedNullableDouble
- VersionedNullableGuid
- VersionedNullableInt32
- VersionedNullableInt64
- VersionedNullableString

## Usage

#### Basic

```csharp
	public class Person : IVersionedProperties {
		public Int64 Id { get; private set; }
		public String Name { get; set; }
		public VersionedDateTime CheckIn { get; private set; }
		
		public Person() { this.InitializeVersionedProperties(); }
	}
	
	public class Context : DbContextWithVersionedProperties {
		public DbSet<Person> People { get; set; }
	}
	
	var nick = new Person { Name = "Nick", CheckIn = { Value = DateTime.Now } };
	context.People.Add(nick);
	context.SaveChanges();
	
	// Then at some point later...
	nick.CheckIn.Value = DateTime.Now;
	context.SaveChanges();
	
	// At this point, `People` contains one row with the most recent check-in value, and `VersionedStrings` contains one row with the previous check-in time.
```

#### Extended
	
```csharp
	using System;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity;
	using System.Data.Entity.Spatial;
	using System.Linq;
	using EntityFramework.Triggers;
	using EntityFramework.VersionedProperties;
	
	namespace Example {
		class Program {
			public enum Standing { Good, Fair, Poor }
			public class StandingVersion : VersionBase<Standing> { }
			[ComplexType]
			public class VersionedStanding : VersionedTypeBase<Standing, StandingVersion, IStandingVersions> {
				protected override Func<IStandingVersions, DbSet<StandingVersion>> VersionDbSet {
					get { return x => x.StandingVersions; }
				}
			}
			public interface IStandingVersions {
				DbSet<StandingVersion> StandingVersions { get; set; }
			}
	
			[ComplexType]
			public class Friendship {
				public Int64 PersonId { get; private set; } // We're unable to apply a foreign constraint here due to current limitations of complex types in Entity Framework 6
				public Boolean IsBestFriend { get; protected set; }
				public Friendship() {}
				public Friendship(Int64 personId, Boolean isBestFriend) {
					PersonId = personId;
					IsBestFriend = isBestFriend;
				}
			}
			public class FriendshipVersion : VersionBase<Friendship> {}
			[ComplexType]
			public class VersionedFriendship : RequiredValueVersionedTypeBase<Friendship, FriendshipVersion, IFriendshipVersions> {
				protected override Friendship DefaultValue { get { return new Friendship(); } }
				protected override Func<IFriendshipVersions, DbSet<FriendshipVersion>> VersionDbSet {
					get { return x => x.FriendshipVersions; }
				}
			}
			public interface IFriendshipVersions {
				DbSet<FriendshipVersion> FriendshipVersions { get; set; }
			}
	
			public class VersionedProperties : IVersionedProperties {
				public VersionedProperties() {
					this.InitializeVersionedProperties();
				}
			}
	
			public class Person : VersionedProperties {
				public Int64 Id { get; private set; }
				public DateTime Inserted { get; private set; }
				public DateTime Updated { get; private set; }
				public VersionedString FirstName { get; private set; }
				public VersionedString LastName { get; private set; }
				public VersionedDbGeography Location { get; private set; }
				public VersionedStanding Standing { get; private set; }
				public VersionedFriendship Friendship { get; private set; }
	
				public Person() {
					this.InitializeVersionedProperties();
					this.Triggers().Inserting += e => e.Entity.Inserted = e.Entity.Updated = DateTime.Now;
					this.Triggers().Updating += e => e.Entity.Updated = DateTime.Now;
				}
			}
	
			public class Context : DbContextWithVersionedProperties, IStandingVersions, IFriendshipVersions {
				public DbSet<Person> People { get; set; }
				public DbSet<StandingVersion> StandingVersions { get; set; }
				public DbSet<FriendshipVersion> FriendshipVersions { get; set; }
			}
	
			static void Main(String[] args) {
				using (var context = new Context()) {
					context.Database.Delete();
					context.Database.Create();
					var nickStrupat = new Person {
						                             FirstName = { Value = "Nick" },
													 LastName = { Value = "Strupat" },
													 Location = { Value = DbGeography.FromText("POINT(-81.24862 42.948881)") },
													 Friendship = { Value = new Friendship(42, false) },
					                             };
					var johnSmith = new Person {
						                           FirstName = { Value = "John" },
						                           LastName = { Value = "Smith" }
					                           };
					context.People.Add(johnSmith);
					context.People.Add(nickStrupat);
					context.SaveChanges();
					nickStrupat = context.People.Single(x => x.Id == nickStrupat.Id);
					nickStrupat.Friendship.Value = new Friendship(johnSmith.Id, true);
					nickStrupat.FirstName.Value = "Nicholas";
					nickStrupat.Location.Value = DbGeography.FromText("POINT(-79.3777061 43.7182713)");
					//context.People.Add(nickStrupat);
					context.SaveChanges();
					var locations = new[] { nickStrupat.Location.Value }.Concat(nickStrupat.Location.Versions(context).Select(x => x.Value)).ToArray();
					var distanceTravelledAsTheCrowFlies = locations.Skip(1).Select((x, i) => x.Distance(locations[i])).Sum();
				}
			}
		}
	}
```