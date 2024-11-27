using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Xunit;
[assembly: ImportConventions]

#pragma warning disable CA1707
namespace Sample.Core.Tests;

#region codeblock

public class SampleTests
{
    [Fact(Skip = "TODO")]
    public async Task Should_Register_Services()
    {
        var context = await ConventionContext.FromAsync(_builder);

        var services = ( await new ServiceCollection().ApplyConventionsAsync(context) ).BuildServiceProvider();
        Assert.Equal("TestService", services.GetRequiredService<IService>().GetString());
    }

    public SampleTests()
    {
        _builder = new ConventionContextBuilder(new Dictionary<object, object>(), []).Set(HostType.UnitTest);
    }

    private readonly ConventionContextBuilder _builder;
}

#endregion
