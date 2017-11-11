using System;
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
    public class ConventionScannerTests
    {
        private class Scanner : ConventionScannerBase
        {
            public Scanner(IAssemblyCandidateFinder assemblyCandidateFinder) : base(assemblyCandidateFinder)
            {
            }
        }

        [Fact]
        public void ShouldConstruct()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            scanner.Should().NotBeNull();
        }

        [Fact]
        public void ShouldBuildAProvider()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<string[]>._))
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
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<string[]>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();
            var provider2 = scanner.BuildProvider();

            provider.Should().BeSameAs(provider2);
        }

        [Fact]
        public void ShouldScanAddedConventions()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<string[]>._))
                .Returns(new Assembly[0]);

            var Convention = A.Fake<IServiceConvention>();

            scanner.AddConvention(Convention);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .Contain(Convention);
        }

        [Fact]
        public void ShouldReturnAllConventions()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<string[]>._))
                .Returns(new Assembly[0]);

            IConvention Convention = A.Fake<IServiceConvention>();
            IConvention Convention2 = A.Fake<ITestConvention>();

            scanner.AddConvention(Convention, Convention2);

            var provider = scanner.BuildProvider();

            var result = provider.GetAll()
                .Select(x => x.Convention);

            result.Should().Contain(Convention).And.Contain(Convention2);
        }

        [Fact]
        public void ShouldReturnAllDelegates()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<string[]>._))
                .Returns(new Assembly[0]);

            Delegate Delegate2 = A.Fake<ServiceConventionDelegate>();
            Delegate Delegate = A.Fake<Action>();

            scanner.AddDelegate(Delegate, Delegate2);

            var provider = scanner.BuildProvider();

            var result = provider.GetAll()
                .Select(x => x.Delegate);

            result.Should().Contain(Delegate).And.Contain(Delegate2);
        }

        [Fact]
        public void ShouldScanExcludeConventionTypes()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<string[]>._))
                .Returns(new Assembly[0]);

            var Convention = A.Fake<IServiceConvention>();

            scanner.AddConvention(Convention);
            scanner.ExceptConvention(typeof(ConventionScannerTests));

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x.GetType() == typeof(AssemblyCandidateResolverTests.Contrib));
        }

        [Fact]
        public void ShouldScanExcludeConventionAssemblies()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<string[]>._))
                .Returns(new Assembly[0]);

            var Convention = A.Fake<IServiceConvention>();

            scanner.AddConvention(Convention);
            scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x.GetType() == typeof(AssemblyCandidateResolverTests.Contrib));
        }
    }
}
