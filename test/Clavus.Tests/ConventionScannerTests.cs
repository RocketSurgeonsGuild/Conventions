using System.Reflection;

using FakeItEasy;

using Rocket.Surgery.Clavus.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;



namespace Rocket.Surgery.Clavus.Tests;

public class ConventionScannerTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public void ShouldConstruct()
    {
        var scanner = ClavusContextBuilder.Create(_ => []);
        scanner.ShouldNotBeNull();
    }

    [Test]
    public async Task ShouldBuildAProvider()
    {
        var builder = ClavusContextBuilder.Create(_ => [], new Dictionary<object, object?>()).AppendConvention(new Contrib());
        var context = await ClavusContext.FromAsync(builder);
        context
           .Conventions
           .GetAll()
           .ShouldContain(x => x is Contrib);
    }

    [Test]
    public async Task ShouldScanAddedContributions()
    {
        var scanner = ClavusContextBuilder.Create(builder => []);

        var contribution = A.Fake<IServicePart>();
        var contribution2 = A.Fake<IServicePart>();

        scanner.PrependConvention(contribution);
        scanner.AppendConvention(contribution2);

        var context = await ClavusContext.FromAsync(scanner);
        context
           .Conventions
           .GetAll()
           .ShouldSatisfyAllConditions(z => z.ShouldContain(contribution2), z => z.ShouldContain(contribution));
    }

    [Test]
    public async Task ShouldIncludeAddedDelegates()
    {
        var scanner = ClavusContextBuilder.Create(_ => []);
        var @delegate = new ServicePart((_, _, _) => { });
        var delegate2 = new ServicePart((_, _, _) => { });

        scanner.PrependDelegate(delegate2, null, null);
        scanner.AppendDelegate(@delegate, null, null);

        var context = await ClavusContext.FromAsync(scanner);

        context
           .Conventions
           .GetAll()
           .ShouldSatisfyAllConditions(z => z.ShouldContain(delegate2), z => z.ShouldContain(@delegate));
    }

    [Test]
    public async Task ShouldScanExcludeContributionTypes()
    {
        var scanner = ClavusContextBuilder.Create(_ => []);

        var contribution = A.Fake<IServicePart>();
        var contribution2 = A.Fake<IServicePart>();

        scanner.AppendConvention(contribution);
        scanner.PrependConvention(contribution2);
        scanner.ExceptConvention(typeof(Contrib));

        var context = await ClavusContext.FromAsync(scanner);

        context
           .Conventions
           .GetAll()
           .ShouldNotContain(x => x is Contrib);
        context
           .Conventions
           .GetAll()
           .ShouldSatisfyAllConditions(z => z.ShouldContain(contribution2), z => z.ShouldContain(contribution));
    }

    [Test]
    public async Task ShouldScanExcludeContributionAssemblies()
    {
        var scanner = ClavusContextBuilder.Create(_ => []);

        var contribution = A.Fake<IServicePart>();

        scanner.PrependConvention(contribution);
        scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var context = await ClavusContext.FromAsync(scanner);

        context
           .Conventions
           .GetAll()
           .ShouldNotContain(x => x is Contrib);
    }
}
