using FluentAssertions;
using Rocket.Surgery.Conventions.Scanners;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class BasicConventionScannerTests
    {
        [Fact]
        public void ShouldConstruct()
        {
            var scanner = new BasicConventionScanner();

            scanner.Should().NotBeNull();
        }

        [Fact]
        public void ShouldBuildAProvider()
        {
            var scanner = new BasicConventionScanner();

            var provider = scanner.BuildProvider();

            provider.Should().NotBeNull();
        }

        [Fact]
        public void ShouldCacheTheProvider()
        {
            var scanner = new BasicConventionScanner();

            var provider = scanner.BuildProvider();
            var provider2 = scanner.BuildProvider();

            provider.Should().BeSameAs(provider2);
        }
    }
}
