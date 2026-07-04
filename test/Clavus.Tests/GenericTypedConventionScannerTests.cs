using System.Reflection;

using FakeItEasy;

using Rocket.Surgery.Extensions.Testing;



namespace Clavus.Tests;

public class GenericTypedConventionScannerTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
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
        var scanner = ClavusContextBuilder.Create(_ => [], new Dictionary<object, object?>()).AppendPart(new Contrib());

        var context = await ClavusContext.FromAsync(scanner);
        context
           .Parts
           .GetAll()
           .ShouldContain(x => x is Contrib);
    }

    [Test]
    public async Task ShouldScanAddedContributions()
    {
        var scanner = ClavusContextBuilder.Create(_ => []);

        var contribution = A.Fake<IServicePart>();
        var contribution2 = A.Fake<IServicePart>();

        scanner.PrependPart(contribution);
        scanner.AppendPart(contribution2);

        var context = await ClavusContext.FromAsync(scanner);
        context
           .Parts
           .GetAll()
           .ShouldSatisfyAllConditions(z => z.ShouldContain(contribution2), z => z.ShouldContain(contribution));
    }

    [Test]
    public async Task ShouldIncludeAddedDelegates()
    {
        var scanner = ClavusContextBuilder.Create(_ => []);

        var @delegate = new ServicePart((_, _) => { });
        var delegate2 = new ServicePart((_, _) => { });

        scanner.PrependDelegate(delegate2, null, null);
        scanner.AppendDelegate(@delegate, null, null);

        var context = await ClavusContext.FromAsync(scanner);
        context
           .Parts
           .GetAll()
           .ShouldSatisfyAllConditions(z => z.ShouldContain(delegate2), z => z.ShouldContain(@delegate));
    }

    [Test]
    public async Task ShouldScanExcludeContributionTypes()
    {
        var scanner = ClavusContextBuilder.Create(_ => []);
        var contribution = A.Fake<IServicePart>();
        var contribution2 = A.Fake<IServicePart>();

        scanner.AppendPart(contribution);
        scanner.PrependPart(contribution2);
        scanner.ExceptConvention(typeof(Contrib));
        var context = await ClavusContext.FromAsync(scanner);
        context
           .Parts
           .GetAll()
           .ShouldNotContain(x => x is Contrib);
        context
           .Parts
           .GetAll()
           .ShouldSatisfyAllConditions(z => z.ShouldContain(contribution2), z => z.ShouldContain(contribution));
    }

    [Test]
    public async Task ShouldScanExcludeContributionAssemblies()
    {
        var scanner = ClavusContextBuilder.Create(_ => []);

        var contribution = A.Fake<IServicePart>();

        scanner.PrependPart(contribution);
        scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var context = await ClavusContext.FromAsync(scanner);
        context
           .Parts
           .GetAll()
           .ShouldNotContain(x => x is Contrib);
    }
}
