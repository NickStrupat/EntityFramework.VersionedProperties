using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties.Tests {
#else
namespace EntityFramework.VersionedProperties.Tests {
#endif
	public class UnitTests : TestBase {
		protected override void TestImpl() {
			var person = new Person();
			Context.People.Add(person);
			person.FirstName.Value = "Nick";

			var snaps = Context.People.SelectSnapshots(DateTime.UtcNow);
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
}
