using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using EntityFramework.Triggers;
using EntityFramework.VersionedProperties;
using Mutuple;

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
				
				var aa = people.GroupJoin(VersionedString     .GetVersionDbSetStatic(context), p => p.FirstName.Id       , v => v.VersionedId, (p, vs) => new Mutuple<Person, IEnumerable<StringVersion>> { Item1 = p, Item2 = vs })
				               .GroupJoin(VersionedString     .GetVersionDbSetStatic(context), m => m.Item1.LastName.Id  , v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>, IEnumerable<StringVersion>> { Item1 = m.Item1, Item2 = m.Item2, Item3 = vs })
				               .GroupJoin(VersionedDbGeography.GetVersionDbSetStatic(context), m => m.Item1.Location.Id  , v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>, IEnumerable<StringVersion>, IEnumerable<DbGeographyVersion>> { Item1 = m.Item1, Item2 = m.Item2, Item3 = m.Item3, Item4 = vs})
				               .GroupJoin(VersionedStanding   .GetVersionDbSetStatic(context), m => m.Item1.Standing.Id  , v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>, IEnumerable<StringVersion>, IEnumerable<DbGeographyVersion>, IEnumerable<StandingVersion>> { Item1 = m.Item1, Item2 = m.Item2, Item3 = m.Item3, Item4 = m.Item4, Item5 = vs })
				               .GroupJoin(VersionedFriendship .GetVersionDbSetStatic(context), m => m.Item1.Friendship.Id, v => v.VersionedId, (m, vs) => new Mutuple<Person, IEnumerable<StringVersion>, IEnumerable<StringVersion>, IEnumerable<DbGeographyVersion>, IEnumerable<StandingVersion>, IEnumerable<FriendshipVersion>> { Item1 = m.Item1, Item2 = m.Item2, Item3 = m.Item3, Item4 = m.Item4, Item5 = m.Item5, Item6 = vs })
				               .Select(m => new {
					Person     = m.Item1,
					FirstNameVersion  = m.Item2.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
					LastNameVersion   = m.Item3.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
					LocationVersion   = m.Item4.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
					StandingVersion   = m.Item5.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt),
					FriendshipVersion = m.Item6.OrderBy(x => x.Added).FirstOrDefault(v => v.Added >= dt)
				});
				// here we need to go over the results: for each non-null version, set the current value of the corresponding vp value AND modified, AND set IsReadOnly to true
				var c = people.ToSnapshots(context, dt);
				var d = people.MyGroupJoin(context.StringVersions, person => person.FirstName.Id, sv => sv.VersionedId, (person, gj1) => new {person, gj1});
				var okay = aa.ToArray();
				var okaySnapshot = okay.Select(x => {
					var p = x.Person;
					p.FirstName.IsReadOnly = true;
					if (x.FirstNameVersion != null)
						p.FirstName.value = x.FirstNameVersion.Value;
					p.LastName.IsReadOnly = true;
					if (x.LastNameVersion != null)
						p.LastName.value = x.LastNameVersion.Value;
					p.Location.IsReadOnly = true;
					if (x.LocationVersion != null)
						p.Location.value = x.LocationVersion.Value;
					p.Standing.IsReadOnly = true;
					if (x.StandingVersion != null)
						p.Standing.value = x.StandingVersion.Value;
					p.Friendship.IsReadOnly = true;
					if (x.FriendshipVersion != null)
						p.Friendship.value = x.FriendshipVersion.Value;
					return p;
				});
			}
		}

		private static DateTime Avg(DateTime x, DateTime y) => new DateTime(Avg(x.Ticks, y.Ticks));
		private static Int64 Avg(Int64 x, Int64 y) => (Int64) new [] {x ,y}.Average();
		private static Int32 Avg(Int32 x, Int32 y) => (Int32) new [] {x ,y}.Average();
	}

	internal static class Ex {
		public static IQueryable<TResult> MyGroupJoin<TOuter, TInner, TKey, TResult>(
			this IQueryable<TOuter> outer,
			IEnumerable<TInner> inner,
			Expression<Func<TOuter, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector) {
			return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector);
		}

		//public static IQueryable<Person>

		//public static Object MyGroupJoin<TEntity, TVersion>(this IQueryable<TEntity> source, IEnumerable<TVersions> versions, Func<TEntity, Object> func, Func<TVersion, Object> func1, Func<Object, Object, Object> func2) {
		//	return null;
		//}
	}
}
