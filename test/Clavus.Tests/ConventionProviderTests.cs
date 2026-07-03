using System.Collections.Immutable;
using Clavus.Infrastructure;
using Clavus.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Serilog.Events;

namespace Clavus.Tests;

public class ClavusProviderTests()
    : AutoFakeTest<TestRecord>(TestRecord.Create(LogEventLevel.Information))
{
    [Test]
    public void Should_Throw_When_A_Cycle_Is_Detected()
    {
        var c1 = new Cyclic1();
        var c2 = new Cyclic2();

        var provider = new ClavusProvider(HostType.Undefined, [], [c1, c2]);

        Action a = () => provider.GetAll();
        a.ShouldThrow<NotSupportedException>();
    }

    [Test]
    [MethodDataSource(nameof(GetCategoriesUndefined))]
    public async Task Should_Sort_Conventions_Correctly(HostType hostType, ImmutableArray<ClavusCategory> categories)
    {
        var b = new B();
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ClavusProvider(
            hostType,
            [.. categories],
            [d, f, b, c, e]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Test]
    [MethodDataSource(nameof(GetCategoriesUndefined))]
    public async Task Should_Not_Affect_Default_Sort_Order(HostType hostType, ImmutableArray<ClavusCategory> categories)
    {
        var b = new B();
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ClavusProvider(
            hostType,
            [.. categories],
            [d, b, c, e, f]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Test]
    [MethodDataSource(nameof(GetCategoriesUndefined))]
    public async Task Should_Leave_Delegates_In_Place(HostType hostType, ImmutableArray<ClavusCategory> categories)
    {
        var b = new B();
        var d1 = new ServicePart((_, _) => { });
        var d2 = new ServicePart((_, _) => { });
        var d3 = new ServicePart((_, _) => { });
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ClavusProvider(
            hostType,
            [.. categories],
            [d1, d, d2, b, c, e, d3, f]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Test]
    [MethodDataSource(nameof(GetCategoriesUndefined))]
    public async Task Should_Leave_Delegates_In_Place_Order_Delegates(HostType hostType, ImmutableArray<ClavusCategory> categories)
    {
        var b = new B();
        var d1 = new ClavusOrDelegate(new ServicePart((_, _) => { }), 0, new("Custom"));
        var d2 = new ClavusOrDelegate(new ServicePart((_, _) => { }), int.MinValue, ClavusCategory.Core);
        var d3 = new ClavusOrDelegate(new ServicePart((_, _) => { }), int.MaxValue, ClavusCategory.Application);
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ClavusProvider(
            hostType,
            [.. categories],
            [d1, d, d2, b, c, e, d3, f]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Test]
    [MethodDataSource(nameof(GetCategoriesUndefined))]
    public async Task Should_Sort_ClavusPartMetadata_Correctly(HostType hostType, ImmutableArray<ClavusCategory> categories)
    {
        var b = new B();
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ClavusProvider(
            hostType,
            [.. categories],
            [
                d,
                f,
                new ClavusPartMetadata(b, HostType.Undefined, ClavusCategory.Application).WithDependency(DependencyDirection.DependsOn, typeof(C)),
                new ClavusPartMetadata(c, HostType.Undefined, ClavusCategory.Core).WithDependency(DependencyDirection.DependentOf, typeof(D)),
                e,
            ]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Test]
    [MethodDataSource(nameof(GetCategoriesLive))]
    public async Task Should_Exclude_Unit_Test_Conventions(HostType hostType, ImmutableArray<ClavusCategory> categories)
    {
        var b = new B();

        var d1 = new ServicePart((_, _) => { });
        var d2 = new ServicePart((_, _) => { });
        var d3 = new ServicePart((_, _) => { });
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ClavusProvider(
            hostType,
            [.. categories],
            [d1, d, d2, b, c, e, d3, f]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    [Test]
    [MethodDataSource(nameof(GetCategoriesUnitTest))]
    public async Task Should_Include_Unit_Test_Conventions(HostType hostType, ImmutableArray<ClavusCategory> categories)
    {
        var b = new B();
        var d1 = new ServicePart((_, _) => { });
        var d2 = new ServicePart((_, _) => { });
        var d3 = new ServicePart((_, _) => { });
        var c = new C();
        var d = new D();
        var e = new E();
        var f = new F();

        var provider = new ClavusProvider(
            hostType,
            [.. categories],
            [d1, d, d2, b, c, e, d3, f]
        );

        await VerifyWithParameters(provider, hostType, categories);
    }

    private SettingsTask VerifyWithParameters(ClavusProvider provider, HostType hostType, ImmutableArray<ClavusCategory> categories) => Verify(
            provider.GetAll().Select(z => z switch { Delegate d => d.Method.Name, IClavusPart c => c.GetType().Name, _ => z.ToString() })
        )
       .UseParameters(hostType, string.Join(",", categories.Select(z => z.ToString())));

    [ClavusCategory(ClavusCategory.Core)]
    [DependentOfPart(typeof(C))]
    private sealed class B : IClavusPart;

    [DependsOnPart(typeof(D))]
    [UnitTestPart]
    private sealed class C : IServicePart
    {
        public void Register(IClavusContext context, IConfiguration configuration, IServiceCollection services) => throw new NotImplementedException();
    }

    [ClavusCategory(ClavusCategory.Application)]
    [AfterPart(typeof(E))]
    private sealed class D : ITestConvention
    {
        public void Register(ITestClavusContext context) => throw new NotImplementedException();
    }

    [ClavusCategory("Custom")]
    private sealed class E : IClavusPart;

    [DependsOnPart(typeof(E))]
    [LivePart]
    private sealed class F : IClavusPart;

    private sealed class Cyclic1 : IClavusPart;

    [BeforePart(typeof(Cyclic1))]
    [DependsOnPart(typeof(Cyclic1))]
    private sealed class Cyclic2 : IClavusPart;

    private static IEnumerable<(HostType, ImmutableArray<ClavusCategory>)> GetCategories(HostType hostType)
    {
        yield return (hostType, ImmutableArray.Create<ClavusCategory>(ClavusCategory.Application));
        yield return (hostType, ImmutableArray.Create<ClavusCategory>(ClavusCategory.Application, new("Custom")));
        yield return (hostType, ImmutableArray.Create<ClavusCategory>(ClavusCategory.Core));
        yield return (hostType, ImmutableArray.Create<ClavusCategory>(ClavusCategory.Core, new("Custom")));
        yield return (hostType, ImmutableArray.Create(new ClavusCategory("Custom")));
    }

    public static IEnumerable<(HostType, ImmutableArray<ClavusCategory>)> GetCategoriesUndefined() => GetCategories(HostType.Undefined);
    public static IEnumerable<(HostType, ImmutableArray<ClavusCategory>)> GetCategoriesLive() => GetCategories(HostType.Live);
    public static IEnumerable<(HostType, ImmutableArray<ClavusCategory>)> GetCategoriesUnitTest() => GetCategories(HostType.UnitTest);
}
