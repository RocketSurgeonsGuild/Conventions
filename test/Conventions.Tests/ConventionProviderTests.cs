using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionProviderTests : AutoTestBase
    {
        public ConventionProviderTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information)
        {
        }

        [DependentOfConvention(typeof(C))]
        class B : IConvention
        {

        }

        [DependsOnConvention(typeof(D))]
        class C : IServiceConvention
        {
            public void Register(IServiceConventionContext context) => throw new NotImplementedException();
        }

        [AfterConvention(typeof(E))]
        class D : ITestConvention
        {
            public void Register(ITestConventionContext context) => throw new NotImplementedException();
        }

        class E : IConvention
        {

        }

        [DependsOnConvention(typeof(E))]
        class F : IConvention
        {

        }

        [Fact]
        public void Should_Sort_Conventions_Correctly()
        {
            var b = new B();
            var c = new C();
            var d = new D();
            var e = new E();
            var f = new F();

            var provider = new ConventionProvider(
                new IConvention[] { b, c, },
                new object[] { d, f },
                new object[] { e }
            );

            var r = provider.GetAll()
                .Select(x => x.Convention as object ?? x.Delegate)
                .Should()
                .ContainInOrder(e, d, f, b, c);
        }

        [Fact]
        public void Should_Not_Affect_Default_Sort_Order()
        {
            var b = new B();
            var c = new C();
            var d = new D();
            var e = new E();
            var f = new F();

            var provider = new ConventionProvider(
                new IConvention[] { b, c, },
                new object[] { d },
                new object[] { e, f }
            );

            var r = provider.GetAll()
                .Select(x => x.Convention as object ?? x.Delegate)
                .Should()
                .ContainInOrder(e, d, b, c, f);
        }

        [Fact]
        public void Should_Leave_Delegates_In_Place()
        {
            var b = new B();
            var d1 = new ServiceConventionDelegate((x) => { });
            var d2 = new ServiceConventionDelegate((x) => { });
            var d3 = new ServiceConventionDelegate((x) => { });
            var c = new C();
            var d = new D();
            var e = new E();
            var f = new F();

            var provider = new ConventionProvider(
                new IConvention[] { b, c, },
                new object[] { d1, d, d2 },
                new object[] { e, d3, f }
            );

            var r = provider.GetAll()
                .Select(x => x.Convention as object ?? x.Delegate)
                .Should()
                .ContainInOrder(d1, e, d, d2, b, c, d3, f);
        }

        class Cyclic1 : IConvention
        {

        }

        [BeforeConvention(typeof(Cyclic1))]
        [DependsOnConvention(typeof(Cyclic1))]
        class Cyclic2 : IConvention
        {

        }

        [Fact]
        public void Should_Throw_When_A_Cycle_Is_Detected()
        {
            var c1 = new Cyclic1();
            var c2 = new Cyclic2();

            var provider = new ConventionProvider(
                new IConvention[] { c1, c2, },
                new object[] { },
                new object[] {  }
            );

            Action a = () => provider.GetAll();
            a.Should().Throw<NotSupportedException>();
        }
    }
}
