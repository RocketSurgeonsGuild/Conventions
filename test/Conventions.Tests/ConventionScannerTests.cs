using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionScannerTests : AutoTestBase
    {
        public ConventionScannerTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

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

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();

            var items = provider.Get<IServiceConvention, ServiceConventionDelegate>().ToArray();

            items
                .Select(x => x.Convention)
                .Should()
                .Contain(x => x.GetType() == typeof(Contrib));
        }

        [Fact]
        public void ShouldCacheTheProvider()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
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

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var convention = A.Fake<IServiceConvention>();
            var convention2 = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);
            scanner.AppendConvention(convention2);

            var provider = scanner.BuildProvider();

            var items = provider.Get<IServiceConvention, ServiceConventionDelegate>().ToArray();
            foreach (var item in items) Logger.LogInformation("Convention: {@item}", new
            {
                convention = item.Convention?.GetType().FullName,
                @delegate = item.Delegate?.ToString()
            });

            items
                .Select(x => x.Convention)
                .Should()
                .ContainInOrder(convention, convention2);
        }

        [Fact]
        public void ShouldReturnAllConventions()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            IConvention convention = A.Fake<IServiceConvention>();
            IConvention convention2 = A.Fake<ITestConvention>();
            IConvention convention3 = A.Fake<IServiceConvention>();
            IConvention convention4 = A.Fake<ITestConvention>();

            scanner.PrependConvention(convention, convention2);
            scanner.AppendConvention(convention3, convention4);

            var provider = scanner.BuildProvider();

            var result = provider.GetAll()
                .Select(x => x.Convention);

            result.Should().ContainInOrder(convention, convention2, convention3, convention4);
        }

        [Fact]
        public void ShouldReturnAllDelegates()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            Delegate delegate2 = A.Fake<ServiceConventionDelegate>();
            Delegate Delegate = A.Fake<Action>();

            scanner.PrependDelegate(Delegate, delegate2);

            var provider = scanner.BuildProvider();

            var result = provider.GetAll()
                .Select(x => x.Delegate);

            result.Should().Contain(Delegate).And.Contain(delegate2);
        }

        [Fact]
        public void ShouldScanExcludeConventionTypes()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var convention = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);
            scanner.ExceptConvention(typeof(ConventionScannerTests));

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x.GetType() == typeof(Contrib));
        }

        [Fact]
        public void ShouldScanExcludeConventionAssemblies()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var convention = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);
            scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x.GetType() == typeof(Contrib));
        }
    }
}
