using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class GenericTypedConventionProviderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper, LogLevel.Information)
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
            new IConvention[] { b, c, },
            new object[] { d, f, },
            new object[] { e, }
        );

        provider
           .GetAll()
           .Should()
           .ContainInOrder(e, d, b);
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
            new IConvention[] { b, c, },
            new object[] { d, },
            new object[] { e, f, }
        );

        provider
           .GetAll()
           .Should()
           .ContainInOrder(e, d, b);
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
            new IConvention[] { b, c, },
            new object[] { d1, d, d2, },
            new object[] { e, d3, f, }
        );

        provider
           .GetAll()
           .Should()
           .ContainInOrder(
                d1,
                e,
                d,
                d2,
                b,
                d3
            );
    }

    [Fact]
    public void Should_Throw_When_A_Cycle_Is_Detected()
    {
        var c1 = new Cyclic1();
        var c2 = new Cyclic2();

        var provider = new ConventionProvider(
            HostType.Undefined,
            new IConvention[] { c1, c2, },
            Array.Empty<object>(),
            Array.Empty<object>()
        );

        Action a = () => provider.GetAll();
        a.Should().Throw<NotSupportedException>();
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
            new object[] { d, f, },
            new object[] { e, }
        );

        provider
           .GetAll()
           .Should()
           .ContainInOrder(e, d, b);
    }

    [Theory]
    [InlineData(HostType.Live)]
    public void Should_Exclude_Unit_Test_Conventions(HostType ctor)
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
            new IConvention[] { b, c, },
            new object[] { d1, d, d2, },
            new object[] { e, d3, f, }
        );

        provider
           .GetAll()
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
    [InlineData(HostType.UnitTest)]
    public void Should_Include_Unit_Test_Conventions(HostType ctor)
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
            new IConvention[] { b, c, },
            new object[] { d1, d, d2, },
            new object[] { e, d3, f, }
        );

        provider
           .GetAll()
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

    [DependentOfConvention<C>]
    private sealed class B : IConvention;

    [DependsOnConvention<D>]
    [UnitTestConvention]
    private sealed class C : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }

    [AfterConvention<E>]
    private sealed class D : ITestConvention
    {
        public void Register(ITestConventionContext context)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class E : IConvention;

    [DependsOnConvention<E>]
    [LiveConvention]
    private sealed class F : IConvention;

    private sealed class Cyclic1 : IConvention;

    [BeforeConvention<Cyclic1>]
    [DependsOnConvention<Cyclic1>]
    private sealed class Cyclic2 : IConvention;
}
