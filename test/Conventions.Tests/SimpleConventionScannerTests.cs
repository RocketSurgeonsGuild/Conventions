using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class SimpleConventionScannerTests : AutoFakeTest
    {
        [Fact]
        public void ShouldConstruct()
        {
            var scanner = AutoFake.Resolve<SimpleConventionScanner>();

            scanner.Should().NotBeNull();
        }

        [Fact]
        public void ShouldBuildAProvider()
        {
            var scanner = AutoFake.Resolve<SimpleConventionScanner>();
            var finder = AutoFake.Resolve<IAssemblyCandidateFinder>();

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
               .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
               .Select(x => x.Convention)
               .Should()
               .Contain(x => x!.GetType() == typeof(Contrib));
        }

        [Fact]
        public void ShouldCacheTheProvider()
        {
            var scanner = AutoFake.Resolve<SimpleConventionScanner>();
            var finder = AutoFake.Resolve<IAssemblyCandidateFinder>();

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
               .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();
            var provider2 = scanner.BuildProvider();

            provider.Should().BeSameAs(provider2);
        }

        [Fact]
        public void ShouldScanAddedConventionss()
        {
            var scanner = AutoFake.Resolve<SimpleConventionScanner>();
            var finder = AutoFake.Resolve<IAssemblyCandidateFinder>();

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
               .Returns(Array.Empty<Assembly>());

            var conventions = A.Fake<IServiceConvention>();

            scanner.PrependConvention(conventions);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
               .Select(x => x.Convention)
               .Should()
               .Contain(conventions);
        }

        [Fact]
        public void ShouldIncludeAddedDelegates()
        {
            var scanner = AutoFake.Resolve<SimpleConventionScanner>();
            var finder = AutoFake.Resolve<IAssemblyCandidateFinder>();

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
               .Returns(Array.Empty<Assembly>());

            var @delegate = new ServiceConventionDelegate(context => { });
            var delegate2 = new ServiceConventionDelegate(context => { });

            scanner.PrependDelegate(delegate2);
            scanner.AppendDelegate(@delegate);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
               .Select(x => x.Delegate)
               .Should()
               .ContainInOrder(delegate2, @delegate);
        }

        [Fact]
        public void ShouldScanExcludeConventionsTypes()
        {
            var scanner = AutoFake.Resolve<SimpleConventionScanner>();
            var finder = AutoFake.Resolve<IAssemblyCandidateFinder>();

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
               .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var contribution = A.Fake<IServiceConvention>();
            var contribution2 = A.Fake<IServiceConvention>();

            scanner.AppendConvention(contribution);
            scanner.PrependConvention(contribution2);
            scanner.ExceptConvention(typeof(Contrib));

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
               .Select(x => x.Convention)
               .Should()
               .NotContain(x => x!.GetType() == typeof(Contrib));
            provider.Get<IServiceConvention, ServiceConventionDelegate>()
               .Select(x => x.Convention)
               .Should()
               .ContainInOrder(contribution2, contribution);
        }

        [Fact]
        public void ShouldScanExcludeConventionsAssemblies()
        {
            var scanner = AutoFake.Resolve<SimpleConventionScanner>();
            var finder = AutoFake.Resolve<IAssemblyCandidateFinder>();

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
               .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var contribution = A.Fake<IServiceConvention>();

            scanner.PrependConvention(contribution);
            scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
               .Select(x => x.Convention)
               .Should()
               .NotContain(x => x!.GetType() == typeof(Contrib));
        }

        public SimpleConventionScannerTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}