using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
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

        class C : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                throw new NotImplementedException();
            }
        }

        class D : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                throw new NotImplementedException();
            }
        }

        class E : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                throw new NotImplementedException();
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

        [Fact]
        public void ShouldAppendConventions_AsASingle()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<IServiceConvention>();

            scanner.AppendConvention(convention);

            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();
            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().Contain(convention);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnArray()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            scanner.AppendConvention(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnEnumerable()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            scanner.AppendConvention(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldAppendConventions_AsAType()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            scanner.AppendConvention<C>();
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().AllBeOfType(typeof(C));
        }

        [Fact]
        public void ShouldAppendConventions_AsAnArrayOfTypes()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            scanner.AppendConvention(typeof(C), typeof(E), typeof(D));
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldAppendConventions_AsAnEnumerableOfTypes()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            scanner.AppendConvention(new[] { typeof(C), typeof(E), typeof(D) }.AsEnumerable());
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldAppendDelegates_AsAnArray()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            scanner.AppendDelegate(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Delegate).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldAppendDelegates_AsAnEnumerable()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            scanner.AppendDelegate(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Delegate).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldPrependConventions_AsASingle()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);

            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();
            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().Contain(convention);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnArray()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            scanner.PrependConvention(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnEnumerable()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            scanner.PrependConvention(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldPrependConventions_AsAType()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            scanner.PrependConvention<C>();
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().AllBeOfType(typeof(C));
        }

        [Fact]
        public void ShouldPrependConventions_AsAnArrayOfTypes()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            scanner.PrependConvention(typeof(C), typeof(E), typeof(D));
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldPrependConventions_AsAnEnumerableOfTypes()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);

            scanner.PrependConvention(new[] { typeof(C), typeof(E), typeof(D) }.AsEnumerable());
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldPrependDelegates_AsAnArray()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            scanner.PrependDelegate(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Delegate).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldPrependDelegates_AsAnEnumerable()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new Scanner(finder);
            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            scanner.PrependDelegate(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Delegate).Should().ContainInOrder(conventions);
        }
    }
}
