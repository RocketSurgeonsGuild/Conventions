using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;


namespace Rocket.Surgery.Hosting.AspNetCore.Tests;

public class RocketWebApplicationBuilderTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
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
