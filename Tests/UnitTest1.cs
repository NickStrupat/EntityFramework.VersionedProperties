using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityFrameworkVersionedProperties.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            using (var context = new Context()) {
                var person = new Person("Nick", "Strupat");
                context.People.Add(person);
                person.FirstName.Value = String.Empty;
                context.SaveChanges();
            }
        }
    }
}
