#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;
using Xunit.Abstractions;

// ReSharper disable once CheckNamespace
namespace AspNetCore.Tests;

public class RocketWebApplicationBuilderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
{
    [Fact]
    public async Task Should_Build_The_Host_Correctly()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                x => x.UseAssemblies(new[] { typeof(RocketWebApplicationBuilderTests).Assembly, })
                            );
        builder.WebHost.UseTestServer();

        await using var host = builder.Build();
        new TestStartup(builder.Environment, builder.Configuration).Configure(host);
        await host.StartAsync();
        var server = host.GetTestServer();
        server.CreateClient();
        await host.StopAsync();
    }
}
#endif
