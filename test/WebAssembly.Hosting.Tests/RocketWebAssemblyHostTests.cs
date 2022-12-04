using System.Reflection;
using System.Text.Json;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;
using Rocket.Surgery.Conventions.Configuration.Yaml;
using Xunit;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

internal class Helpers
{
    public static WebAssemblyHostBuilder CreateWebAssemblyHostBuilder()
    {
        var interop = new BunitJSInterop();
        interop.Setup<string>("Blazor._internal.navigationManager.getUnmarshalledBaseURI").SetResult("https://localhost:5000/");
        interop.Setup<string>("Blazor._internal.navigationManager.getUnmarshalledLocationHref").SetResult("https://localhost:5000/app/");
        interop.Setup<string>("Blazor._internal.getApplicationEnvironment").SetResult("UnitTesting");
        interop.Setup<int>("Blazor._internal.registeredComponents.getRegisteredComponentsCount").SetResult(0);
        interop.Setup<string>("Blazor._internal.getPersistedState").SetResult("");
        interop.Setup<byte[]>("Blazor._internal.getConfig", _ => true).SetResult("{}"u8.ToArray());

        var builder = (WebAssemblyHostBuilder)Activator.CreateInstance(
            typeof(WebAssemblyHostBuilder),
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
            null, // Type.DefaultBinder,
            new object[]
            {
                interop.JSRuntime,
                new JsonSerializerOptions()
            },
            null
        )!;
        builder.Services.RemoveAll(typeof(IJSRuntime));
        builder.Services.AddSingleton(interop);
        builder.Services.AddSingleton(interop.JSRuntime);
        return builder;
    }
}

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
    public async Task Creates_RocketHost_WithModifiedConfiguration()
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
}
