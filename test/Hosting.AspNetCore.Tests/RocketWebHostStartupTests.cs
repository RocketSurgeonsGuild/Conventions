using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.AspNetCore.Tests;

public class RocketWebHostStartupTests : AutoFakeTest
{
    [Fact]
    public async Task Should_Start_Application()
    {
        var builder = _baseBuilder;
        builder.ConfigureWebHost(x => x.UseStartup<SimpleStartup>().UseTestServer());

        using var host = builder.Build();
        await host.StartAsync();
        var server = host.GetTestServer();
        var response = await server.CreateRequest("/")
                                   .GetAsync();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("SimpleStartup -> Configure");
        await host.StopAsync();
    }

    public RocketWebHostStartupTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        _baseBuilder = Host
                      .CreateDefaultBuilder()
                      .ConfigureWebHostDefaults(x => { })
                      .ConfigureRocketSurgery(x => x.UseAssemblies(new[] { typeof(RocketWebHostBuilderTests).Assembly }));
    }

    private readonly IHostBuilder _baseBuilder;
}
