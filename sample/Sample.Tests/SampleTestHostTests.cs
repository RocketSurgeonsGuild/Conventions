using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        _host = TestHost.For(typeof(SampleTestHostTests))
                        .Create()
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
