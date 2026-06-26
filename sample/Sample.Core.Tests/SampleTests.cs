[assembly: ImportConventions]

#pragma warning disable CA1707
namespace Sample.Core.Tests;

#region codeblock

public class SampleTests
{
    [Test]
    public async Task Should_Register_Services()
    {
        var context = await ConventionContext.FromAsync(_builder);

        // var services = ( await new ServiceCollection().ApplyConventionsAsync(context) ).BuildServiceProvider();
        // await Assert.That(services.GetRequiredService<IService>().GetString()).IsEqualTo("TestService");
    }

    public SampleTests() => _builder = ConventionContextBuilder.Create(_ => [], new Dictionary<object, object>(), []).Set(HostType.UnitTest);

    private readonly ConventionContextBuilder _builder;
}

#endregion
