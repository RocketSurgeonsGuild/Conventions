using FakeItEasy;

using Rocket.Surgery.Extensions.Testing;



namespace Clavus.Tests;

public class GenericTypedConventionScannerTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public void ShouldConstruct()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);
        scanner.ShouldNotBeNull();
    }

    [Test]
    public async Task ShouldBuildAProvider()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []).AppendPart(new Contrib());

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }

    [Test]
    public async Task ShouldScanAddedContributions()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);

        var contribution = A.Fake<IServicePart>(c => c.Named("contribution"));
        var contribution2 = A.Fake<IServicePart>(c => c.Named("contribution2"));

        scanner.PrependPart(contribution);
        scanner.AppendPart(contribution2);

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }

    [Test]
    public async Task ShouldIncludeAddedDelegates()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);

        var @delegate = A.Fake<ServicePart>(c => c.Named("delegate"));
        var delegate2 = A.Fake<ServicePart>(c => c.Named("delegate2"));

        scanner.ConfigureServices(delegate2, default, null);
        scanner.ConfigureServices(@delegate, default, null);

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }

    [Test]
    public async Task ShouldScanExcludeContributionTypes()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);
        var contribution = A.Fake<IServicePart>(c => c.Named("contribution"));
        var contribution2 = A.Fake<IServicePart>(c => c.Named("contribution2"));

        scanner.AppendPart(contribution);
        scanner.PrependPart(contribution2);
        scanner.ExceptConvention(typeof(Contrib));

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }

    [Test]
    public async Task ShouldScanExcludeContributionAssemblies()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);

        var contribution = A.Fake<IServicePart>(c => c.Named("contribution"));

        scanner.PrependPart(contribution);
        scanner.ExceptConvention(contribution.GetType().Assembly);

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }
}
