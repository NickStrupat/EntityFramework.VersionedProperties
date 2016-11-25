using System;
using System.Threading;
using System.Threading.Tasks;

#if NET40
using NUnit.Framework;
using Fact = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

#if EF_CORE
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.VersionedProperties.Tests {
#else
using System.Data.Entity;
using System.Data.Entity.Validation;
namespace EntityFramework.VersionedProperties.Tests {
#endif
	public abstract class TestBase : IDisposable {
		protected virtual void Setup() {}
		protected virtual void Teardown() {}

		protected abstract void TestImpl();

		[Fact]
		public void Test() {
			if (!semaphoreSlim.Wait(10000))
				Assert.True(false, "Wait failed due to timeout");
			Setup();
			try {
				TestImpl();
			}
			finally {
				Teardown();
				semaphoreSlim.Release();
			}
		}

#if !NET40
		protected abstract Task TestAsyncImpl();

		[Fact]
		protected async Task TestAsync() {
			if (!await semaphoreSlim.WaitAsync(10000).ConfigureAwait(false))
				Assert.True(false, "Wait failed due to timeout");
			Setup();
			try {
				await TestAsyncImpl().ConfigureAwait(false);
			}
			finally {
				Teardown();
				semaphoreSlim.Release();
			}
		}
#endif

		private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
		protected readonly Context Context = new Context();

		public virtual void Dispose() => Context.Dispose();
	}
}
