#if NET6_0_OR_GREATER
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using NetEscapades.Configuration.Yaml;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;
using Rocket.Surgery.Web.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.AspNetCore.Tests;

public class RocketWebApplicationTests : AutoFakeTest
{
    [Fact]
    public async Task Should_Start_Application()
    {
        var builder = _baseBuilder;
        builder.WebHost.UseTestServer();

        await using var host = builder.Build();
        new SimpleStartup().Configure(host);
        await host.StartAsync().ConfigureAwait(false);
        var server = host.GetTestServer();
        var response = await server.CreateRequest("/")
                                   .GetAsync().ConfigureAwait(false);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.Should().Be("SimpleStartup -> Configure");
        await host.StopAsync().ConfigureAwait(false);
    }

    [Fact]
    public void Creates_RocketHost_ForAppDomain()
    {
        var host = WebApplication
                  .CreateBuilder().LaunchWith(Web.Hosting.RocketBooster.For(AppDomain.CurrentDomain));
        host.Should().BeAssignableTo<WebApplicationBuilder>();
    }

    [Fact]
    public void Creates_RocketHost_ForAssemblies()
    {
        var host = WebApplication
                  .CreateBuilder()
                  .LaunchWith(Web.Hosting.RocketBooster.For(new[] { typeof(RocketWebApplicationTests).Assembly }));
        host.Should().BeAssignableTo<WebApplicationBuilder>();
    }

    [Fact]
    public void Creates_RocketHost_WithConfiguration()
    {
        var host = WebApplication
                  .CreateBuilder()
                  .LaunchWith(Web.Hosting.RocketBooster.For(new[] { typeof(RocketWebApplicationTests).Assembly }))
                  .ConfigureRocketSurgery(c => c.GetOrAdd(() => new ConfigOptions()).UseJson().UseYaml().UseYml().UseIni());
        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
        configuration.Providers.OfType<IniConfigurationProvider>().Should().HaveCount(3);
    }

    [Fact]
    public void Creates_RocketHost_WithModifiedConfiguration()
    {
        var host = WebApplication
                  .CreateBuilder()
                  .LaunchWith(Web.Hosting.RocketBooster.For(new[] { typeof(RocketWebApplicationTests).Assembly }));

        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(0);
        configuration.Providers.OfType<IniConfigurationProvider>().Should().HaveCount(0);
    }

    public RocketWebApplicationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        _baseBuilder = WebApplication
                      .CreateBuilder()
                      .ConfigureRocketSurgery(x => x.UseAssemblies(new[] { typeof(RocketWebHostBuilderTests).Assembly }));
    }

    private readonly WebApplicationBuilder _baseBuilder;
}
#endif
