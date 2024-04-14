using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.AspNetCore.Tests;

public class RocketWebHostBuilderTests : AutoFakeTest
{
    [Fact]
    public void Should_Build_The_Host_Correctly()
    {
        var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
                          .ConfigureWebHostDefaults(x => x.UseStartup<TestStartup>().UseTestServer())
                          .ConfigureRocketSurgery(x => x.UseAssemblies(new[] { typeof(RocketWebHostBuilderTests).Assembly }));
        using var host = builder.Build();
        host.StartAsync();
        var server = host.GetTestServer();
        server.CreateClient();
        host.StopAsync();
    }

    public RocketWebHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
}
