using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net.Sockets;
using EntityFramework.Triggers;
using EntityFramework.VersionedProperties;

namespace Example {
	class Program {
		static Program() {
			SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
		}

		public enum Standing { Good = 0, Fair = 1, Poor = 2 }
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
				nickStrupat.FirstName.Value = "N.";
				//context.People.Add(nickStrupat);
				context.SaveChanges();
				var locations = new[] { nickStrupat.Location.Value }.Concat(nickStrupat.Location.GetVersions(context).Select(x => x.Value)).ToArray();
				var distanceTravelledAsTheCrowFlies = locations.Skip(1).Select((x, i) => x.Distance(locations[i])).Sum();

				// get an entity snapshot at some moment
				var first = nickStrupat.FirstName.Versions.First().Added;
				var last = nickStrupat.FirstName.Versions.Last().Added;
				var dt = Avg(first, last);
				var people = context.People;
				var a =
					people.GroupJoin(context.StringVersions, person => person.FirstName.Id, sv => sv.VersionedId, (person, gj1) => new {person, gj1})
						.GroupJoin(context.StringVersions, @t => @t.person.LastName.Id, sv2 => sv2.VersionedId, (@t, gj2) => new {@t, gj2})
						.GroupJoin(context.DbGeographyVersions, @t => @t.@t.person.Location.Id, sv3 => sv3.VersionedId, (@t, gj3) => new {@t, gj3})
						.GroupJoin(context.StandingVersions, @t => @t.@t.@t.person.Standing.Id, sv4 => sv4.VersionedId, (@t, gj4) => new {@t, gj4})
						.GroupJoin(context.FriendshipVersions, @t => @t.@t.@t.@t.person.Friendship.Id, sv5 => sv5.VersionedId, (@t, gj5) => new {
							Person = @t.@t.@t.@t.person,
							FirstName = @t.@t.@t.@t.gj1.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
							LastName = @t.@t.@t.gj2.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
							Location = @t.@t.gj3.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
							Standing = @t.gj4.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
							Friendship = gj5.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt)
						});
				// here we need to go over the results: for each non-null version, set the current value of the corresponding vp value AND modified, and set IsReadOnly to true
				var b = people.SelectSnapshots(context, dt);
				var okay = a.ToArray();
			}
		}

		private static DateTime Avg(DateTime x, DateTime y) => new DateTime(Avg(x.Ticks, y.Ticks));
		private static Int64 Avg(Int64 x, Int64 y) => (Int64) new [] {x ,y}.Average();
		private static Int32 Avg(Int32 x, Int32 y) => (Int32) new [] {x ,y}.Average();
	}
}
