using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.Configuration.Json;
using Rocket.Surgery.Conventions.Configuration.Yaml;

namespace Rocket.Surgery.Hosting.Tests;

public class RocketHostTests
{
    [Fact]
    public async Task Creates_RocketHost_ForAppDomain()
    {
        using var host = await Host.CreateApplicationBuilder().LaunchWith(RocketBooster.For(AppDomain.CurrentDomain));
        host.Should().BeAssignableTo<IHostApplicationBuilder>();
    }

    [Fact]
    public async Task Creates_RocketHost_ForAssemblies()
    {
        using var host = await Host
                        .CreateApplicationBuilder()
                        .LaunchWith(RocketBooster.For(new[] { typeof(RocketHostTests).Assembly, }));
        host.Should().BeAssignableTo<IHostApplicationBuilder>();
    }

    [Fact]
    public async Task Creates_RocketHost_WithConfiguration()
    {
        using var host = await Host
                        .CreateApplicationBuilder()
                        .LaunchWith(RocketBooster.For(Imports.Instance));
        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Json()
    {
        using var host = await Host
                        .CreateApplicationBuilder()
                        .LaunchWith(
                             RocketBooster.For(Imports.Instance),
                             z => z.ExceptConvention(typeof(YamlConvention))
                         );

        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(0);
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Yaml()
    {
        using var host = await Host
                        .CreateApplicationBuilder()
                        .LaunchWith(
                             RocketBooster.For(Imports.Instance),
                             z => z.ExceptConvention(typeof(JsonConvention))
                         );

        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(0);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
    }
}
