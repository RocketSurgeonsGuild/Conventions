using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;
using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.AspNetCore.Tests;

public class RocketWebApplicationBuilderTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper))
{
    [Fact]
    public async Task Should_Build_The_Host_Correctly()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        await using var host = await builder.ConfigureRocketSurgery();

        new TestStartup(builder.Environment, builder.Configuration).Configure(host);
        await host.StartAsync();
        var server = host.GetTestServer();
        server.CreateClient();
        await host.StopAsync();
    }
}
