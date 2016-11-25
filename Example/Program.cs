using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using EntityFramework.Triggers;
using EntityFramework.VersionedProperties;

namespace Example {
	class Program {
		static Program() {
			SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
		}

		public enum Standing { Good, Fair, Poor }
		public class StandingVersion : VersionBase<Standing> { }
		[ComplexType]
		public class VersionedStanding : VersionedBase<VersionedStanding, Standing, StandingVersion, IStandingVersions> {
			protected override DbSet<StandingVersion> GetVersionDbSet(IStandingVersions x) => x.StandingVersions;
		}
		public interface IStandingVersions {
			DbSet<StandingVersion> StandingVersions { get; set; }
		}

		[ComplexType]
		public class Friendship {
			public Int64 PersonId { get; private set; } // We're unable to apply a foreign constraint here due to current limitations of complex types in Entity Framework 6
			public Boolean IsBestFriend { get; private set; }
			public Friendship() {}
			public Friendship(Int64 personId, Boolean isBestFriend) {
				PersonId = personId;
				IsBestFriend = isBestFriend;
			}
		}
		public class FriendshipVersion : RequiredValueVersionBase<Friendship> {}

		[ComplexType]
		public class VersionedFriendship : VersionedRequiredValueBase<VersionedFriendship, Friendship, FriendshipVersion, IFriendshipVersions> {
			protected override Friendship DefaultValue => new Friendship();
			protected override DbSet<FriendshipVersion> GetVersionDbSet(IFriendshipVersions x) => x.FriendshipVersions;
		}
		public interface IFriendshipVersions {
			DbSet<FriendshipVersion> FriendshipVersions { get; set; }
		}

		public class Person {
			public Int64 Id { get; private set; }
			public DateTime Inserted { get; private set; }
			public DateTime Updated { get; private set; }
			public VersionedString FirstName { get; private set; } = new VersionedString();
			public VersionedString LastName { get; private set; } = new VersionedString();
			public VersionedDbGeography Location { get; private set; } = new VersionedDbGeography();
			public VersionedStanding Standing { get; private set; } = new VersionedStanding();
			public VersionedFriendship Friendship { get; private set; } = new VersionedFriendship();

			static Person() {
				VersionedProperties<Person>.Initialize();
				Triggers<Person>.Inserting += e => e.Entity.Inserted = e.Entity.Updated = DateTime.Now;
				Triggers<Person>.Updating += e => e.Entity.Updated = DateTime.Now;
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
				var locations = new[] { nickStrupat.Location.Value }.Concat(nickStrupat.Location.GetVersions(context).Select(x => x.Value)).ToArray();
				var distanceTravelledAsTheCrowFlies = locations.Skip(1).Select((x, i) => x.Distance(locations[i])).Sum();
			}
		}
	}
}
