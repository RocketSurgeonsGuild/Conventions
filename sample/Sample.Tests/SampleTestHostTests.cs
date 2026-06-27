using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Core;
using TUnit.Core.Interfaces;

#pragma warning disable CA1707
namespace Sample.Tests;

#region codeblock

public class SampleTestHostTests : IAsyncInitializer, IAsyncDisposable
{
    [Test]
    public async Task Should_Register_Services() => await Assert.That(_host.Services.GetRequiredService<IService>().GetString()).IsEqualTo("TestService");

    private IHost _host = null!;

    public async Task InitializeAsync()
    {
        _host = await Host.CreateApplicationBuilder().ConfigureRocketSurgery();
        await _host.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}

#endregion
