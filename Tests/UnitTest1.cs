using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using EntityFramework.Triggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityFramework.VersionedProperties.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            using (var context = new Context()) {
	            context.Database.Delete();
	            context.Database.Create();

	            var person = new Person { FirstName = { Value = "Nick" }, LastName = { Value = "Strupat"} };
	            person.FirstName.Value = "Nicholas";
				//person.Location = DbGeometry.FromText("POINT(53.095124 -0.864716)");
                context.People.Add(person);
                //person.FirstName.Value = String.Empty;
                context.SaveChanges();
	            Assert.IsTrue(context.StringVersions.Single().Value == "Nick");
	            person.LastName.Value = "Sputnik";
				context.SaveChanges();
				Assert.IsTrue(context.StringVersions.Count() == 2);
				Assert.IsTrue(context.StringVersions.Single(x => x.VersionedId == person.LastName.Id).Value == "Strupat");

	            Boolean updateFailed = false;
	            person.Triggers.UpdateFailed += e => updateFailed = true;
	            try {
		            person.Inserted = new DateTime();
		            context.SaveChanges(); // Should throw here about datetime2
	            }
				catch (DbUpdateException ex) {
		            //ex.Entries.Single().Entity
	            }
				Assert.IsTrue(updateFailed);

				var another = new Person { FirstName = { Value = "John" }, LastName = { Value = "Smith" } };
	            another.FirstName.Value = "Johnathan";
	            another.LastName.Value = "Smitherman";
	            context.People.Add(another);
	            person.FirstName.Value = "TEST";
	            context.People.Remove(person);
				context.SaveChanges();
				Assert.IsTrue(!context.StringVersions.Any(x => x.VersionedId == person.FirstName.Id || x.VersionedId == person.LastName.Id));
            }
        }
    }
}
