using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using EntityFrameworkTriggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityFrameworkVersionedProperties.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            using (var context = new Context()) {
	            context.People.RemoveRange(context.People);
	            context.SaveChangesWithTriggers();

	            var person = new Person("Nick", "Strupat");
	            person.FirstName.Value = "Nicholas";
				//person.Location = DbGeometry.FromText("POINT(53.095124 -0.864716)");
                context.People.Add(person);
                //person.FirstName.Value = String.Empty;
                context.SaveChangesWithTriggers();
	            Assert.IsTrue(context.StringVersions.Single().Value == "Nick");
	            person.LastName.Value = "Sputnik";
				context.SaveChangesWithTriggers();
				Assert.IsTrue(context.StringVersions.Count() == 2);
				Assert.IsTrue(context.StringVersions.Single(x => x.VersionedId == person.LastName.Id).Value == "Strupat");

	            try {
		            person.Inserted = new DateTime();
		            context.SaveChangesWithTriggers(); // Should throw here about datetime2
	            }
				catch (DbUpdateException ex) {
		            //ex.Entries.Single().Entity
	            }

	            context.People.Remove(person);
				context.SaveChangesWithTriggers();
				Assert.IsTrue(!context.StringVersions.Any());
            }
        }
    }
}
