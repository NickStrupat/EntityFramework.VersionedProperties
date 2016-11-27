using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using EntityFramework.Triggers;
using EntityFramework.VersionedProperties;
using Mutuples;

namespace Example {
	class Program {
		static Program() {
			SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
		}

		public enum Standing { Good = 0, Fair = 1, Poor = 2 }
		public class StandingVersion : VersionBase<Standing> { }
		public interface IStandingVersions { DbSet<StandingVersion> StandingVersions { get; set; } }
		[ComplexType]
		public class VersionedStanding : VersionedBase<VersionedStanding, Standing, StandingVersion, IStandingVersions> {}

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
		public interface IFriendshipVersions { DbSet<FriendshipVersion> FriendshipVersions { get; set; } }
		[ComplexType]
		public class VersionedFriendship : VersionedRequiredValueBase<VersionedFriendship, Friendship, FriendshipVersion, IFriendshipVersions> {
			protected override Friendship DefaultValue => new Friendship();
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
				Func<Context, IEnumerable<StringVersion>> fasd = dbc => dbc.StringVersions;
				var people = context.People;
				
				var a = from person in people
						join sv in context.StringVersions on person.FirstName.Id equals sv.VersionedId into gj1
						select new {
							Person = person,
							FirstName = gj1.OrderBy(x => x.Added).FirstOrDefault(x => x.Added >= dt)
						};
				//people.GroupJoin(context.StringVersions, person => person.FirstName.Id, sv => sv.VersionedId, (person, gj1) => new {person, gj1})
				//	.GroupJoin(context.StringVersions, @t => @t.person.LastName.Id, sv2 => sv2.VersionedId, (@t, gj2) => new {@t, gj2})
				//	.GroupJoin(context.DbGeographyVersions, @t => @t.@t.person.Location.Id, sv3 => sv3.VersionedId, (@t, gj3) => new {@t, gj3})
				//	.GroupJoin(context.StandingVersions, @t => @t.@t.@t.person.Standing.Id, sv4 => sv4.VersionedId, (@t, gj4) => new {@t, gj4})
				//	.GroupJoin(context.FriendshipVersions, @t => @t.@t.@t.@t.person.Friendship.Id, sv5 => sv5.VersionedId, (@t, gj5) => new {
				//		Person = @t.@t.@t.@t.person,
				//		FirstName = @t.@t.@t.@t.gj1.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
				//		LastName = @t.@t.@t.gj2.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
				//		Location = @t.@t.gj3.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
				//		Standing = @t.gj4.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
				//		Friendship = gj5.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt)
				//	});
				new Tuple<int, int, int, int, int>(1, 1, 1, 1, 1);
				
				var aa = people.Select(x => new Mutuple<Person> { Item1 = x })
				               .GroupJoin(VersionedString     .GetVersionDbSet(context), m => m.Item1.FirstName.Id , v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>> { Item1 = m.Item1, Item2 = vs })
				               .GroupJoin(VersionedString     .GetVersionDbSet(context), m => m.Item1.LastName.Id  , v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>, IEnumerable<StringVersion>> { Item1 = m.Item1, Item2 = m.Item2, Item3 = vs })
				               .GroupJoin(VersionedDbGeography.GetVersionDbSet(context), m => m.Item1.Location.Id  , v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>, IEnumerable<StringVersion>, IEnumerable<DbGeographyVersion>> { Item1 = m.Item1, Item2 = m.Item2, Item3 = m.Item3, Item4 = vs})
				               .GroupJoin(VersionedStanding   .GetVersionDbSet(context), m => m.Item1.Standing.Id  , v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>, IEnumerable<StringVersion>, IEnumerable<DbGeographyVersion>, IEnumerable<StandingVersion>> { Item1 = m.Item1, Item2 = m.Item2, Item3 = m.Item3, Item4 = m.Item4, Item5 = vs })
				               .GroupJoin(VersionedFriendship .GetVersionDbSet(context), m => m.Item1.Friendship.Id, v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>, IEnumerable<StringVersion>, IEnumerable<DbGeographyVersion>, IEnumerable<StandingVersion>, IEnumerable<FriendshipVersion>> { Item1 = m.Item1, Item2 = m.Item2, Item3 = m.Item3, Item4 = m.Item4, Item5 = m.Item5, Item6 = vs })
				               .Select(m => new Mutuple<Person, StringVersion, StringVersion, DbGeographyVersion, StandingVersion, FriendshipVersion> {
					Item1 = m.Item1,
					Item2 = m.Item2.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
					Item3 = m.Item3.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
					Item4 = m.Item4.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
					Item5 = m.Item5.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
					Item6 = m.Item6.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt)
				});
				// here we need to go over the results: for each non-null version, set the current value of the corresponding vp value AND modified, AND set IsReadOnly to true
				var c = people.ToSnapshots(context, dt);
				var okay = aa.ToArray();
				var okaySnapshot = okay.Select(x => {
					var p = x.Item1;
					p.FirstName .SetSnapshotVersion(x.Item2);
					p.LastName  .SetSnapshotVersion(x.Item3);
					p.Location  .SetSnapshotVersion(x.Item4);
					p.Standing  .SetSnapshotVersion(x.Item5);
					p.Friendship.SetSnapshotVersion(x.Item6);
					return p;
				});
			}
		}

		private static DateTime Avg(DateTime x, DateTime y) => new DateTime(Avg(x.Ticks, y.Ticks));
		private static Int64 Avg(Int64 x, Int64 y) => (Int64) new [] {x ,y}.Average();
		private static Int32 Avg(Int32 x, Int32 y) => (Int32) new [] {x ,y}.Average();
	}
}
