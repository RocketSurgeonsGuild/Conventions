using FluentAssertions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Configuration.Json;
using Rocket.Surgery.Conventions.Configuration.Yaml;
using Xunit;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

public class RocketWebAssemblyHostTests
{
    [Fact]
    public async Task Creates_RocketHost_ForAppDomain()
    {
        var host = await Helpers.CreateWebAssemblyHostBuilder().LaunchWith(RocketBooster.For(AppDomain.CurrentDomain));
        host.Should().BeAssignableTo<WebAssemblyHostBuilder>();
    }

    [Fact]
    public async Task Creates_RocketHost_ForAssemblies()
    {
        var host = await Helpers.CreateWebAssemblyHostBuilder()
                                .LaunchWith(RocketBooster.For(new[] { typeof(RocketWebAssemblyHostTests).Assembly }));
        host.Should().BeAssignableTo<WebAssemblyHostBuilder>();
    }

    [Fact]
    public async Task Creates_RocketHost_WithConfiguration()
    {
        var host = await Helpers.CreateWebAssemblyHostBuilder()
                                .LaunchWith(RocketBooster.For(new[] { typeof(RocketWebAssemblyHostTests).Assembly }));
        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonStreamConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlStreamConfigurationProvider>().Should().HaveCount(6);
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Json()
    {
        var host = await Helpers.CreateWebAssemblyHostBuilder()
                                .LaunchWith(
                                     RocketBooster.For(new[] { typeof(RocketWebAssemblyHostTests).Assembly }),
                                     z => z.ExceptConvention(typeof(YamlConvention))
                                 );

        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonStreamConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlStreamConfigurationProvider>().Should().HaveCount(0);
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Yaml()
    {
        var host = await Helpers.CreateWebAssemblyHostBuilder()
                                .LaunchWith(
                                     RocketBooster.For(new[] { typeof(RocketWebAssemblyHostTests).Assembly }),
                                     z => z.ExceptConvention(typeof(JsonConvention))
                                 );

        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonStreamConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlStreamConfigurationProvider>().Should().HaveCount(6);
    }
}
