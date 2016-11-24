using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

#if EF_CORE
namespace EntityFrameworkCore.VersionedProperties.Tests {
#else
namespace EntityFramework.VersionedProperties.Tests {
#endif
	public class UnitTests {
		[Fact]
		public void Test() {

		}
	}
}
