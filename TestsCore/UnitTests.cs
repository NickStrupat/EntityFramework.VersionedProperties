using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
#if NET40
using NUnit.Framework;
using Fact = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties.Tests {
#else
namespace EntityFramework.VersionedProperties.Tests {
#endif
	// TODO:
	// Change tracking VersionedX in EFCore (INotifyPropertyChanged)
	// Set() and change tracking in EF6 and EFCore

	public class LocalInstantiationAndModification : TestBase {
		protected override void TestImpl() {
			const String first = "Nick";
			const String second = "Nicholas";

			var person = new Person();
			Assert.False(person.FirstName.IsReadOnly);
			Assert.True(person.FirstName.Value == null);
			Assert.True(person.FirstName.Id == Guid.Empty);
			Assert.True(person.FirstName.Modified == default(DateTime?));
			Assert.True(person.FirstName.LocalVersions.Count == 0);

			person.FirstName.Value = first;
			Assert.False(person.FirstName.IsReadOnly);
			Assert.True(person.FirstName.Value == first);
			Assert.True(person.FirstName.Id == Guid.Empty);
			var modified = person.FirstName.Modified;
			Assert.True(modified != default(DateTime?));
			Assert.True(person.FirstName.LocalVersions.Count == 0);

			person.FirstName.Value = second;
			Assert.False(person.FirstName.IsReadOnly);
			Assert.True(person.FirstName.Value == second);
			var id = person.FirstName.Id;
			Assert.True(id != Guid.Empty);
			Assert.True(person.FirstName.Modified != default(DateTime?));
			Assert.True(person.FirstName.Modified != modified);
			Assert.True(person.FirstName.GetVersions(Context).Count() == 0);

			var prev = person.FirstName.LocalVersions.Single();
			Assert.True(prev.VersionedId == id);
			Assert.True(prev.Id == 0);
			Assert.True(prev.Added == modified);
			Assert.True(prev.Value == first);

			Context.People.Add(person);
			Context.SaveChanges();

			Assert.False(person.FirstName.IsReadOnly);
			Assert.True(person.FirstName.Value == second);
			Assert.True(person.FirstName.Id == id);
			Assert.True(person.FirstName.Modified != default(DateTime?));
			Assert.True(person.FirstName.Modified != modified);
			Assert.True(person.FirstName.LocalVersions.Count == 0);

			var remoteVersion = person.FirstName.GetVersions(Context).Single();
			Assert.True(remoteVersion.VersionedId == id);
			Assert.True(remoteVersion.Id != 0);
			Assert.True(remoteVersion.Added == modified);
			Assert.True(remoteVersion.Value == first);

			Context.People.Remove(person);
			Context.SaveChanges();

			Assert.False(person.FirstName.IsReadOnly);
			Assert.True(person.FirstName.Value == second);
			Assert.True(person.FirstName.Id == id);
			Assert.True(person.FirstName.Modified != default(DateTime?));
			Assert.True(person.FirstName.Modified != modified);
			Assert.True(person.FirstName.LocalVersions.Count == 0);

			Assert.True(person.FirstName.GetVersions(Context).Count() == 0);

			// get the state of all people as of 24 hours ago
			//var snaps = Context.People.ToSnapshots(Context, DateTime.UtcNow - TimeSpan.FromDays(1));

			//var dt = DateTime.UtcNow;
			//var f = Context.People.Select(p => {
			//	p.
			//	Context.StringVersions.Where(x => x.VersionedId == p.FirstName.Id).OrderByDescending(x => x.Added).First(v => v.Added > dt)
			//});
			//var what = Context.People.Join(Context.StringVersions.Last(x => x.Added < dt), p => p.FirstName.Id, sv => sv.VersionedId, (p, version) => { p. });
		}

#if !NET40
		protected override Task TestAsyncImpl() {
			return null;
		}
#endif
	}

	public class SaveAndLoad : TestBase {
		protected override void TestImpl() {
			var person = new Person { FirstName = { Value = "John" } };
			Context.People.Add(person);
		}

#if !NET40
		protected override Task TestAsyncImpl() {
			throw new NotImplementedException();
		}
#endif
	}
}
