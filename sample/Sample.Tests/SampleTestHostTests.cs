using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Hosting;
using Sample.Core;
using Xunit;

#pragma warning disable CA1707
namespace Sample.Tests;

#region codeblock

public class SampleTestHostTests : IAsyncLifetime
{
    private readonly IHost _host;

    public SampleTestHostTests()
    {
        var builder = ConventionContextBuilder.Create().ForTesting(typeof(SampleTestHostTests));
        _host = new HostBuilder()
               .ConfigureRocketSurgery(builder)
               .Build();
    }

    [Fact]
    public void Should_Register_Services()
    {
        Assert.Equal("TestService", _host.Services.GetRequiredService<IService>().GetString());
    }

    public Task InitializeAsync()
    {
        return _host.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}

#endregion
