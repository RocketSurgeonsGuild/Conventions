#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;
using Rocket.Surgery.Web.Hosting;
using Xunit.Abstractions;

// ReSharper disable once CheckNamespace
namespace AspNetCore.Tests;

public class RocketWebApplicationBuilderTests : AutoFakeTest
{
    [Fact]
    public void Should_Build_The_Host_Correctly()
    {
        var builder = WebApplication
                     .CreateBuilder()
                     .ConfigureRocketSurgery(
                          x => x.UseAssemblies(new[] { typeof(RocketWebApplicationBuilderTests).Assembly, })
                      );
        builder.WebHost.UseTestServer();

        using var host = builder.Build();
        new TestStartup(builder.Environment, builder.Configuration).Configure(host);
        host.StartAsync();
        var server = host.GetTestServer();
        server.CreateClient();
        host.StopAsync();
    }

    public RocketWebApplicationBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
}
#endif
