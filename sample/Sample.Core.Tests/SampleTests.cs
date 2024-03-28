using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Xunit;

#pragma warning disable CA1707
namespace Sample.Core.Tests;

#region codeblock

public class SampleTests
{
    [Fact]
    public void Should_Register_Services()
    {
        var context = ConventionContext.From(_builder);

        var services = new ServiceCollection().ApplyConventions(context).BuildServiceProvider();
        Assert.Equal("TestService", services.GetRequiredService<IService>().GetString());
    }

    public SampleTests()
    {
        _builder = new ConventionContextBuilder(new Dictionary<object, object>())
                  .Set(HostType.UnitTest)
                  .UseDependencyContext(DependencyContext.Load(typeof(SampleTests).Assembly)!);
    }

    private readonly ConventionContextBuilder _builder;
}

#endregion