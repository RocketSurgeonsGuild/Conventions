using System.Reflection;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Sample.DependencyOne;
using Sample.DependencyThree;
using Sample.DependencyTwo;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionScannerTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper))
{
    [Fact]
    public void ShouldConstruct()
    {
        var scanner = ConventionContextBuilder.Create(_ => []);
        scanner.ShouldNotBeNull();
    }

    [Fact]
    public async Task ShouldBuildAProvider()
    {
        var builder = ConventionContextBuilder.Create(_ => [], new Dictionary<object, object?>()).AppendConvention(new Contrib());
        var context = await ConventionContext.FromAsync(builder);
        context.Conventions
           .Get<IServiceConvention, ServiceConvention>()
           .ShouldContain(x => x is Contrib);
    }

    [Fact]
    public async Task ShouldScanAddedContributions()
    {
        var scanner = ConventionContextBuilder.Create(builder => []);

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.AppendConvention(contribution2);

        var context = await ConventionContext.FromAsync(scanner);
        context.Conventions
           .Get<IServiceConvention, ServiceConvention>()
           .ShouldSatisfyAllConditions(z => z.ShouldContain(contribution2), z => z.ShouldContain(contribution));
    }

    [Fact]
    public async Task ShouldIncludeAddedDelegates()
    {
        var scanner = ConventionContextBuilder.Create(_ => []);
        var @delegate = new ServiceConvention((_, _, _) => { });
        var delegate2 = new ServiceConvention((_, _, _) => { });

        scanner.PrependDelegate(delegate2, null, null);
        scanner.AppendDelegate(@delegate, null, null);

        var context = await ConventionContext.FromAsync(scanner);

        context.Conventions
           .Get<IServiceConvention, ServiceConvention>()
           .ShouldSatisfyAllConditions(z => z.ShouldContain(delegate2), z => z.ShouldContain(@delegate));
    }

    [Fact]
    public async Task ShouldScanExcludeContributionTypes()
    {
        var scanner = ConventionContextBuilder.Create(_ => []);

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.AppendConvention(contribution);
        scanner.PrependConvention(contribution2);
        scanner.ExceptConvention(typeof(Contrib));

        var context = await ConventionContext.FromAsync(scanner);

        context.Conventions
               .Get<IServiceConvention, ServiceConvention>()
               .ShouldNotContain(x => x! is Contrib);
        context.Conventions
               .Get<IServiceConvention, ServiceConvention>()
               .ShouldSatisfyAllConditions(z => z.ShouldContain(contribution2), z => z.ShouldContain(contribution));
    }

    [Fact]
    public async Task ShouldScanExcludeContributionAssemblies()
    {
        var scanner = ConventionContextBuilder.Create(_ => []);

        var contribution = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var context = await ConventionContext.FromAsync(scanner);

        context.Conventions
               .Get<IServiceConvention, ServiceConvention>()
               .ShouldNotContain(x => x! is Contrib);
    }

    [field: AllowNull]
    [field: MaybeNull]
    private ILoggerFactory LoggerFactory => field ??= CreateLoggerFactory();

    private ILogger Logger => LoggerFactory.CreateLogger(GetType());
}
