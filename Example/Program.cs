using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using EntityFramework.Triggers;
using EntityFramework.VersionedProperties;

namespace Example {
	class Program {
		public enum Standing { Good, Fair, Poor }
		public class StandingVersion : VersionBase<Standing> { }
		[ComplexType]
		public class VersionedStanding : VersionedBase<Standing, StandingVersion> {
			protected override Func<IDbContextWithVersionedProperties, DbSet<StandingVersion>> VersionsDbSet {
				get { return x => ((Context) x).StandingVersions; }
			}
		}

		[ComplexType]
		public class Friendship {
			public Int64 PersonId { get; protected set; } // We're unable to apply a foreign constraint here due to current limitations of complex types in Entity Framework 6
			public Boolean IsBestFriend { get; protected set; }
			public Friendship() {}
			public Friendship(Int64 personId, Boolean isBestFriend) {
				PersonId = personId;
				IsBestFriend = isBestFriend;
			}
		}
		public class FriendshipVersion : VersionBase<Friendship> {}
		[ComplexType]
		public class VersionedFriendship : RequiredValueVersionedBase<Friendship, FriendshipVersion> {
			protected override Friendship DefaultValue { get { return new Friendship(); } }
			protected override Func<IDbContextWithVersionedProperties, DbSet<FriendshipVersion>> VersionsDbSet {
				get { return x => ((Context)x).FriendshipVersions; }
			}
		}

		public class Person : IVersionedProperties {
			public Int64 Id { get; protected set; }
			public DateTime Inserted { get; protected set; }
			public DateTime Updated { get; protected set; }
			public VersionedString FirstName { get; protected set; }
			public VersionedString LastName { get; protected set; }
			public VersionedDbGeography Location { get; protected set; }
			public VersionedStanding Standing { get; protected set; }
			public VersionedFriendship Friendship { get; protected set; }

			public Person() {
				this.InitializeVersionedProperties();
				this.Triggers().Inserting += e => e.Entity.Inserted = e.Entity.Updated = DateTime.Now;
				this.Triggers().Updating += e => e.Entity.Updated = DateTime.Now;
			}
		}

		public class Context : DbContextWithVersionedProperties {
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
												 Location = { Value = DbGeography.FromText("POINT(42.948881 -81.24862)") },
				                             };
				var johnSmith = new Person {
					                           FirstName = { Value = "John" },
					                           LastName = { Value = "Smith" }
				                           };
				context.People.Add(johnSmith);
				context.SaveChanges();
				nickStrupat.Friendship.Value = new Friendship(johnSmith.Id, true);
				nickStrupat.FirstName.Value = "Nicholas";
				nickStrupat.Location.Value = DbGeography.FromText("POINT(43.7182713 -79.3777061)");
				context.People.Add(nickStrupat);
				context.SaveChanges();

			}
		}
	}
}
