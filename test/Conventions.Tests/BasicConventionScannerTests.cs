using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
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

        [Fact]
        public void ShouldIncludeAddedConventions()
        {
            var scanner = new BasicConventionScanner();

            var convention = A.Fake<IServiceConvention>();

            scanner.AddConvention(convention);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .Contain(convention);
        }

        [Fact]
        public void ShouldIncludeAddedDelegates()
        {
            var scanner = new BasicConventionScanner();

            var @delegate = new ServiceConventionDelegate(context => { });

            scanner.AddDelegate(@delegate);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Delegate)
                .Should()
                .Contain(@delegate);
        }

        [Fact]
        public void ShouldHandleExcludedConventions()
        {
            var scanner = new BasicConventionScanner();

            var convention = A.Fake<IServiceConvention>();

            scanner.AddConvention(convention);
            scanner.ExceptConvention(convention.GetType());

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(convention);
        }
    }
}
