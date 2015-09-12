using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Triggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityFramework.VersionedProperties.Tests {
	[TestClass]
	public class UnitTest1 {
		private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
		private void TestMethod1Wait(Object o) {
			EventWaitHandle queued = (EventWaitHandle) o;
			queued.Set();
			manualResetEvent.WaitOne();
			TestMethod1(false);
		}
		[TestMethod]
		public void TestAsync() {
			var list = new ManualResetEvent[32];
			for (var i = 0; i != list.Length; ++i) {
				list[i] = new ManualResetEvent(initialState: false);
				var i1 = i;
				Task.Factory.StartNew(TestMethod1Wait, list[i1]);
			}
			foreach (var a in list)
				a.WaitOne();
			manualResetEvent.Set();
			foreach (var a in list)
				a.WaitOne();
		}

		[TestMethod]
		public void TestMethod1() => TestMethod1(true);

		private void TestMethod1(Boolean deleteDbFirst) {
			using (var context = new Context()) {
				if (deleteDbFirst && context.Database.Exists()) {
					context.Database.Delete();
					context.Database.Create();
				}

				var person = new Person { FirstName = { Value = "Nick" }, LastName = { Value = "Strupat" } };
				person.FirstName.Value = "Nicholas";
				//person.Location = DbGeometry.FromText("POINT(53.095124 -0.864716)");
				context.People.Add(person);
				//person.FirstName.Value = String.Empty;
				context.SaveChanges();
				Assert.IsTrue(context.StringVersions.Single().Value == "Nick");
				person.LastName.Value = "Sputnik";
				context.SaveChanges();
				Assert.IsTrue(context.StringVersions.Count() == 2);
				Assert.IsTrue(person.LastName.Versions(context).Single().Value == "Strupat");

				Boolean updateFailed = false;
				person.Triggers().UpdateFailed += e => updateFailed = true;
				try {
					person.Inserted = new DateTime();
					context.SaveChanges(); // Should throw here about datetime2
				}
				catch (DbUpdateException ex) {
				}
				Assert.IsTrue(updateFailed);

				var another = new Person { FirstName = { Value = "John" }, LastName = { Value = "Smith" } };
				another.FirstName.Value = "Johnathan";
				another.LastName.Value = "Smitherman";
				context.People.Add(another);
				person.FirstName.Value = "TEST";
				context.People.Remove(person);
				context.SaveChanges();
				Assert.IsFalse(person.FirstName.Versions(context).Any());
				Assert.IsFalse(person.LastName.Versions(context).Any());

				context.Database.Delete();
			}
		}
	}
}
