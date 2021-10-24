using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionProviderTests : AutoFakeTest
{
    [Fact]
    public void Should_Sort_Conventions_Correctly()
    {
        var b = new B();
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            HostType.Undefined,
            new IConvention[] { b, c },
            new object[] { d, f },
            new object[] { e }
        );

        provider.GetAll()
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
            HostType.Undefined,
            new IConvention[] { b, c },
            new object[] { d },
            new object[] { e, f }
        );

        provider.GetAll()
                .Should()
                .ContainInOrder(e, d, b, c, f);
    }

    [Fact]
    public void Should_Leave_Delegates_In_Place()
    {
        var b = new B();
        var d1 = new ServiceConvention((_, _, _) => { });
        var d2 = new ServiceConvention((_, _, _) => { });
        var d3 = new ServiceConvention((_, _, _) => { });
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            HostType.Undefined,
            new IConvention[] { b, c },
            new object[] { d1, d, d2 },
            new object[] { e, d3, f }
        );

        provider.GetAll()
                .Should()
                .ContainInOrder(
                     d1,
                     e,
                     d,
                     d2,
                     b,
                     c,
                     d3,
                     f
                 );
    }

    [Fact]
    public void Should_Throw_When_A_Cycle_Is_Detected()
    {
        var c1 = new Cyclic1();
        var c2 = new Cyclic2();

        var provider = new ConventionProvider(
            HostType.Undefined,
            new IConvention[] { c1, c2 },
            Array.Empty<object>(),
            Array.Empty<object>()
        );

        Action a = () => provider.GetAll();
        a.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [InlineData(HostType.Undefined, HostType.Live)]
    [InlineData(HostType.Live, HostType.Live)]
    [InlineData(HostType.Live, HostType.Undefined)]
    [InlineData(HostType.UnitTest, HostType.Live)] // call has precedence
    public void Should_Exclude_Unit_Test_Conventions(HostType ctor, HostType call)
    {
        var b = new B();
        var d1 = new ServiceConvention((_, _, _) => { });
        var d2 = new ServiceConvention((_, _, _) => { });
        var d3 = new ServiceConvention((_, _, _) => { });
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            ctor,
            new IConvention[] { b, c },
            new object[] { d1, d, d2 },
            new object[] { e, d3, f }
        );

        provider.GetAll(call)
                .Should()
                .ContainInOrder(
                     d1,
                     e,
                     d,
                     d2,
                     b,
                     d3,
                     f
                 );
    }

    [Theory]
    [InlineData(HostType.Undefined, HostType.UnitTest)]
    [InlineData(HostType.UnitTest, HostType.UnitTest)]
    [InlineData(HostType.UnitTest, HostType.Undefined)]
    [InlineData(HostType.Live, HostType.UnitTest)] // call has precedence
    public void Should_Include_Unit_Test_Conventions(HostType ctor, HostType call)
    {
        var b = new B();
        var d1 = new ServiceConvention((_, _, _) => { });
        var d2 = new ServiceConvention((_, _, _) => { });
        var d3 = new ServiceConvention((_, _, _) => { });
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            ctor,
            new IConvention[] { b, c },
            new object[] { d1, d, d2 },
            new object[] { e, d3, f }
        );

        provider.GetAll(call)
                .Should()
                .ContainInOrder(
                     d1,
                     e,
                     d,
                     d2,
                     b,
                     c,
                     d3
                 );
    }

    [Fact]
    public void Should_Sort_ConventionWithDependencies_Correctly()
    {
        var b = new B();
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            HostType.Undefined,
            new IConventionWithDependencies[]
            {
                new ConventionWithDependencies(b, HostType.Undefined).WithDependency(DependencyDirection.DependsOn, typeof(C)),
                new ConventionWithDependencies(c, HostType.Undefined).WithDependency(DependencyDirection.DependentOf, typeof(D)),
            },
            new object[] { d, f },
            new object[] { e }
        );

        provider.GetAll()
                .Should()
                .ContainInOrder(e, c, d, f, b);
    }

    public ConventionProviderTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information)
    {
    }

    [DependentOfConvention(typeof(C))]
    private class B : IConvention
    {
    }

    [DependsOnConvention(typeof(D))]
    [UnitTestConvention]
    private class C : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }

    [AfterConvention(typeof(E))]
    private class D : ITestConvention
    {
        public void Register(ITestConventionContext context)
        {
            throw new NotImplementedException();
        }
    }

    private class E : IConvention
    {
    }

    [DependsOnConvention(typeof(E))]
    [LiveConvention]
    private class F : IConvention
    {
    }

    private class Cyclic1 : IConvention
    {
    }

    [BeforeConvention(typeof(Cyclic1))]
    [DependsOnConvention(typeof(Cyclic1))]
    private class Cyclic2 : IConvention
    {
    }
}
