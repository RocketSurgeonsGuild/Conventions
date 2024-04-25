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
    [Fact]
    public void Should_Register_Services()
    {
        Assert.Equal("TestService", _host.Services.GetRequiredService<IService>().GetString());
    }

    private IHost _host = null!;

    public async Task InitializeAsync()
    {
        var builder = ConventionContextBuilder.Create().ForTesting(typeof(SampleTestHostTests));
        _host = await Host.CreateApplicationBuilder().ConfigureRocketSurgery(builder);
        await _host.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}

#endregion