using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration.Json;
using Rocket.Surgery.Conventions.Configuration.Yaml;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.Hosting.AspNetCore.Tests;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;
using Xunit.Abstractions;

// ReSharper disable once CheckNamespace
namespace AspNetCore.Tests;

public class RocketWebApplicationTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
{
    [Fact]
    public async Task Should_Start_Application()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                x => x.UseAssemblies(new[] { typeof(RocketWebApplicationTests).Assembly, })
                            );
        builder.WebHost.UseTestServer();

        await using var host = builder.Build();
        new SimpleStartup().Configure(host);
        await host.StartAsync();
        var server = host.GetTestServer();
        var response = await server
                            .CreateRequest("/")
                            .GetAsync();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("SimpleStartup -> Configure");
        await host.StopAsync();
    }

    [Fact]
    public async Task Creates_RocketHost_ForAppDomain()
    {
        var host = await WebApplication
                        .CreateBuilder()
                        .LaunchWith(RocketBooster.For(AppDomain.CurrentDomain));
        host.Should().BeAssignableTo<WebApplicationBuilder>();
    }

    [Fact]
    public async Task Creates_RocketHost_ForAssemblies()
    {
        var host = await WebApplication
                        .CreateBuilder()
                        .LaunchWith(RocketBooster.For(new[] { typeof(RocketWebApplicationTests).Assembly, }));
        host.Should().BeAssignableTo<WebApplicationBuilder>();
    }

    [Fact]
    public async Task Creates_RocketHost_WithConfiguration()
    {
        var host = await WebApplication
                        .CreateBuilder()
                        .LaunchWith(RocketBooster.For(Imports.GetConventions));
        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Json()
    {
        var host = await WebApplication
                        .CreateBuilder()
                        .LaunchWith(
                             RocketBooster.For(Imports.GetConventions),
                             z => z.ExceptConvention(typeof(YamlConvention))
                         );

        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(0);
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Yaml()
    {
        var host = await WebApplication
                        .CreateBuilder()
                        .LaunchWith(
                             RocketBooster.For(Imports.GetConventions),
                             z => z.ExceptConvention(typeof(JsonConvention))
                         );

        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(0);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
    }
}
