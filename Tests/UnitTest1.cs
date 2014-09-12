using System;
using System.Data.Entity.Spatial;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityFrameworkVersionedProperties.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            using (var context = new Context()) {
                var person = new Person("Nick", "Strupat");
	            person.FirstName.Value = "Nicholas";
				//person.Location = DbGeometry.FromText("POINT(53.095124 -0.864716)");
                context.People.Add(person);
                //person.FirstName.Value = String.Empty;
                context.SaveChanges();
            }
        }
    }
}
