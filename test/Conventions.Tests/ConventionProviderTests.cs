using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionProviderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper, LogLevel.Information)
{
    [Fact]
    public void Should_Throw_When_A_Cycle_Is_Detected()
    {
        var c1 = new Cyclic1();
        var c2 = new Cyclic2();

        var provider = new ConventionProvider(HostType.Undefined, [], [c1, c2,]);

        Action a = () => provider.GetAll();
        a.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [MemberData(nameof(GetCategories), HostType.Undefined)]
    public async Task Should_Sort_Conventions_Correctly(HostType hostType, ImmutableArray<ConventionCategory> categories)
    {
        var b = new B();
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            hostType,
            [..categories,],
            [d, f, b, c, e,]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Theory]
    [MemberData(nameof(GetCategories), HostType.Undefined)]
    public async Task Should_Not_Affect_Default_Sort_Order(HostType hostType, ImmutableArray<ConventionCategory> categories)
    {
        var b = new B();
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            hostType,
            [..categories,],
            [d, b, c, e, f,]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Theory]
    [MemberData(nameof(GetCategories), HostType.Undefined)]
    public async Task Should_Leave_Delegates_In_Place(HostType hostType, ImmutableArray<ConventionCategory> categories)
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
            hostType,
            [..categories,],
            [d1, d, d2, b, c, e, d3, f,]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Theory]
    [MemberData(nameof(GetCategories), HostType.Undefined)]
    public async Task Should_Leave_Delegates_In_Place_Order_Delegates(HostType hostType, ImmutableArray<ConventionCategory> categories)
    {
        var b = new B();
        var d1 = new ConventionOrDelegate(new ServiceConvention((_, _, _) => { }), 0, new("Custom"));
        var d2 = new ConventionOrDelegate(new ServiceConvention((_, _, _) => { }), int.MinValue, ConventionCategory.Infrastructure);
        var d3 = new ConventionOrDelegate(new ServiceConvention((_, _, _) => { }), int.MaxValue, ConventionCategory.Application);
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            hostType,
            [..categories,],
            [d1, d, d2, b, c, e, d3, f,]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Theory]
    [MemberData(nameof(GetCategories), HostType.Undefined)]
    public async Task Should_Sort_ConventionMetadata_Correctly(HostType hostType, ImmutableArray<ConventionCategory> categories)
    {
        var b = new B();
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ConventionProvider(
            hostType,
            [..categories,],
            [
                d,
                f,
                new ConventionMetadata(b, HostType.Undefined, ConventionCategory.Application).WithDependency(DependencyDirection.DependsOn, typeof(C)),
                new ConventionMetadata(c, HostType.Undefined, ConventionCategory.Infrastructure).WithDependency(DependencyDirection.DependentOf, typeof(D)),
                e,
            ]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Theory]
    [MemberData(nameof(GetCategories), HostType.Live)]
    public async Task Should_Exclude_Unit_Test_Conventions(HostType hostType, ImmutableArray<ConventionCategory> categories)
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
            hostType,
            [..categories,],
            [d1, d, d2, b, c, e, d3, f,]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Theory]
    [MemberData(nameof(GetCategories), HostType.UnitTest)]
    public async Task Should_Include_Unit_Test_Conventions(HostType hostType, ImmutableArray<ConventionCategory> categories)
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
            hostType,
            [..categories,],
            [d1, d, d2, b, c, e, d3, f,]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    private SettingsTask VerifyWithParameters(ConventionProvider provider, HostType hostType, ImmutableArray<ConventionCategory> categories)
    {
        return Verify(provider.GetAll().Select(z => z switch { Delegate d => d.Method.Name, IConvention c => c.GetType().Name, _ => z.ToString(), }))
           .UseParameters(hostType, string.Join(",", categories.Select(z => z.ToString())));
    }

    [ConventionCategory(ConventionCategory.Infrastructure)]
    [DependentOfConvention(typeof(C))]
    private sealed class B : IConvention;

    [DependsOnConvention(typeof(D))]
    [UnitTestConvention]
    private sealed class C : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }

    [ConventionCategory(ConventionCategory.Application)]
    [AfterConvention(typeof(E))]
    private sealed class D : ITestConvention
    {
        public void Register(ITestConventionContext context)
        {
            throw new NotImplementedException();
        }
    }

    [ConventionCategory("Custom")]
    private sealed class E : IConvention;

    [DependsOnConvention(typeof(E))]
    [LiveConvention]
    private sealed class F : IConvention;

    private sealed class Cyclic1 : IConvention;

    [BeforeConvention(typeof(Cyclic1))]
    [DependsOnConvention(typeof(Cyclic1))]
    private sealed class Cyclic2 : IConvention;

    public static IEnumerable<object[]> GetCategories(HostType hostType)
    {
        yield return [hostType, ImmutableArray.Create<ConventionCategory>(ConventionCategory.Application),];
        yield return [hostType, ImmutableArray.Create<ConventionCategory>(ConventionCategory.Application, new("Custom")),];
        yield return [hostType, ImmutableArray.Create<ConventionCategory>(ConventionCategory.Infrastructure),];
        yield return [hostType, ImmutableArray.Create<ConventionCategory>(ConventionCategory.Infrastructure, new("Custom")),];
        yield return [hostType, ImmutableArray.Create(new ConventionCategory("Custom")),];
    }
}
