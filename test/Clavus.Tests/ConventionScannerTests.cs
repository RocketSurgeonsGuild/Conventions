using FakeItEasy;

using Rocket.Surgery.Extensions.Testing;



namespace Clavus.Tests;

public class ConventionScannerTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task ShouldConstruct()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);
        scanner.ShouldNotBeNull();

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }

    [Test]
    public async Task ShouldBuildAProvider()
    {
        var builder = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []).AppendPart(new Contrib());

        var context = await ClavusContext.FromAsync(builder);
        await Verify(context.Parts.Select(z => z.ToString()));
    }

    [Test]
    public async Task ShouldScanAddedContributions()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);

        var contribution = A.Fake<IServicePart>(z => z.Named("contribution"));
        var contribution2 = A.Fake<IServicePart>(z => z.Named("contribution2"));

        scanner.PrependPart(contribution);
        scanner.AppendPart(contribution2);

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }

    [Test]
    public async Task ShouldIncludeAddedDelegates()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);

        var d1 = A.Fake<ServicePart>(z => z.Named("d1"));
        var d2 = A.Fake<ServicePart>(z => z.Named("d2"));

        var @delegate = scanner.ConfigureServices(d1, default, null);
        var delegate2 = scanner.ConfigureServices(d2, default, null);

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }

    [Test]
    public async Task ShouldScanExcludeContributionTypes()
    {
        var scanner = ClavusContextBuilder.Create([], new Dictionary<object, object?>(), []);

        var contribution = A.Fake<IServicePart>(z => z.Named("contribution"));
        var contribution2 = A.Fake<IServicePart>(z => z.Named("contribution2"));

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

        var contribution = A.Fake<IServicePart>(z => z.Named("contribution"));

        scanner.PrependPart(contribution);
        scanner.ExceptConvention(contribution.GetType().Assembly);

        var context = await ClavusContext.FromAsync(scanner);
        await Verify(context.Parts.Select(z => z.ToString()));
    }
}
