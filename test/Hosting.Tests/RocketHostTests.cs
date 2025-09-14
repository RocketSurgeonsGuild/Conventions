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
    public async Task Creates_RocketHost_WithConfiguration()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery();
        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        #if NET10_0_OR_GREATER
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(6);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(12);
        #else
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(6);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(12);
        #endif
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Json()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(z => z.ExceptConvention(typeof(YamlConvention)));

        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        #if NET10_0_OR_GREATER
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(6);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(0);
        #else
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(6);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(0);
        #endif
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Yaml()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(z => z.ExceptConvention(typeof(JsonConvention)));

        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        #if NET10_0_OR_GREATER
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(0);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(12);
        #else
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(0);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(12);
        #endif
    }
}
