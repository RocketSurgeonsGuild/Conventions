[assembly: ImportClavusParts]

#pragma warning disable CA1707
namespace Sample.Core.Tests;

#region codeblock

public class SampleTests
{
    [Test]
    public async Task Should_Register_Services()
    {
        var context = await ClavusContext.FromAsync(_builder);

        // var services = ( await new ServiceCollection().ApplyPartsAsync(context) ).BuildServiceProvider();
        // await Assert.That(services.GetRequiredService<IService>().GetString()).IsEqualTo("TestService");
    }

    public SampleTests() => _builder = ClavusContextBuilder.Create(_ => [], new Dictionary<object, object>(), []).Set(HostType.UnitTest);

    private readonly ClavusContextBuilder _builder;
}

#endregion
