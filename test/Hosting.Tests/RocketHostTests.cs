using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetEscapades.Configuration.Yaml;
using Rocket.Surgery.Conventions.Configuration.Yaml;
using Xunit;

namespace Rocket.Surgery.Hosting.Tests;

public class RocketHostTests
{
    [Fact]
    public void Creates_RocketHost_ForAppDomain()
    {
        var host = Host.CreateDefaultBuilder().LaunchWith(RocketBooster.For(AppDomain.CurrentDomain));
        host.Should().BeAssignableTo<IHostBuilder>();
    }

    [Fact]
    public void Creates_RocketHost_ForAssemblies()
    {
        var host = Host.CreateDefaultBuilder()
                       .LaunchWith(RocketBooster.For(new[] { typeof(RocketHostTests).Assembly }));
        host.Should().BeAssignableTo<IHostBuilder>();
    }

    [Fact]
    public void Creates_RocketHost_WithConfiguration()
    {
        var host = Host.CreateDefaultBuilder()
                       .LaunchWith(RocketBooster.For(new[] { typeof(RocketHostTests).Assembly }));
        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
    }

    [Fact]
    public void Creates_RocketHost_WithModifiedConfiguration()
    {
        var host = Host.CreateDefaultBuilder()
                       .LaunchWith(
                            RocketBooster.For(new[] { typeof(RocketHostTests).Assembly }),
                            z => z.ExceptConvention(typeof(YamlConvention))
                        );

        var configuration = (IConfigurationRoot)host.Build().Services.GetRequiredService<IConfiguration>();

        configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
        configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(0);
    }
}
