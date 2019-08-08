using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class BasicConventionScannerTests: AutoTestBase
    {
        public BasicConventionScannerTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
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
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            scanner.Should().NotBeNull();
        }

        [Fact]
        public void ShouldBuildAProvider()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            var provider = scanner.BuildProvider();

            provider.Should().NotBeNull();
        }

        [Fact]
        public void ShouldCacheTheProvider()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            var provider = scanner.BuildProvider();
            var provider2 = scanner.BuildProvider();

            provider.Should().BeSameAs(provider2);
        }

        [Fact]
        public void ShouldHandleExcludedConventions()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            var convention = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);
            scanner.ExceptConvention(convention.GetType());

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(convention);
        }

        [Fact]
        public void ShouldAppendConventions_AsASingle()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
            var convention = A.Fake<IServiceConvention>();

            scanner.AppendConvention(convention);

            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();
            var delegateOrConventions = result as DelegateOrConvention[] ?? result.ToArray();
            delegateOrConventions.Count().Should().Be(1);
            delegateOrConventions.Select(x => x.Convention).Should().Contain(convention);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnArray()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
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
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
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
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            scanner.AppendConvention<C>();
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().AllBeOfType(typeof(C));
        }

        [Fact]
        public void ShouldAppendConventions_AsAnArrayOfTypes()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            scanner.AppendConvention(typeof(C), typeof(E), typeof(D));
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldAppendConventions_AsAnEnumerableOfTypes()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            scanner.AppendConvention(new[] { typeof(C), typeof(E), typeof(D) }.AsEnumerable());
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldAppendDelegates_AsAnArray()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
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
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
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
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
            var convention = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);

            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();
            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().Contain(convention);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnArray()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
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
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
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
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            scanner.PrependConvention<C>();
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().AllBeOfType(typeof(C));
        }

        [Fact]
        public void ShouldPrependConventions_AsAnArrayOfTypes()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            scanner.PrependConvention(typeof(C), typeof(E), typeof(D));
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldPrependConventions_AsAnEnumerableOfTypes()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();

            scanner.PrependConvention(new[] { typeof(C), typeof(E), typeof(D) }.AsEnumerable());
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldPrependDelegates_AsAnArray()
        {
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
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
            var scanner = AutoFake.Resolve<BasicConventionScanner>();
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
