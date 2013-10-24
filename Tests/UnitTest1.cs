using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EfCodeFirstVersionedProperties.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            using (var context = new Context()) {
                var person = new Person("Nick", "Strupat");
                context.People.Add(person);
                person.FirstName.v
            }
        }
    }
}
