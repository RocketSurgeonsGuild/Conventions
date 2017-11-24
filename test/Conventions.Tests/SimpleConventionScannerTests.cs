using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class SimpleConventionScannerTests
    {

        [Fact]
        public void ShouldConstruct()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new SimpleConventionScanner(finder);

            scanner.Should().NotBeNull();
        }

        [Fact]
        public void ShouldBuildAProvider()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new SimpleConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .Contain(x => x.GetType() == typeof(AssemblyCandidateResolverTests.Contrib));
        }

        [Fact]
        public void ShouldCacheTheProvider()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new SimpleConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();
            var provider2 = scanner.BuildProvider();

            provider.Should().BeSameAs(provider2);
        }

        [Fact]
        public void ShouldScanAddedConventionss()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new SimpleConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var conventions = A.Fake<IServiceConvention>();

            scanner.AddConvention(conventions);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .Contain(conventions);
        }

        [Fact]
        public void ShouldIncludeAddedDelegates()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new SimpleConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var @delegate = new ServiceConventionDelegate(context => { });

            scanner.AddDelegate(@delegate);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Delegate)
                .Should()
                .Contain(@delegate);
        }

        [Fact]
        public void ShouldScanExcludeConventionsTypes()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new SimpleConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var convention = A.Fake<IServiceConvention>();

            scanner.AddConvention(convention);
            scanner.ExceptConvention(typeof(ConventionScannerTests));

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x.GetType() == typeof(AssemblyCandidateResolverTests.Contrib));
        }

        [Fact]
        public void ShouldScanExcludeConventionsAssemblies()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new SimpleConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var convention = A.Fake<IServiceConvention>();

            scanner.AddConvention(convention);
            scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x.GetType() == typeof(AssemblyCandidateResolverTests.Contrib));
        }
    }
}
