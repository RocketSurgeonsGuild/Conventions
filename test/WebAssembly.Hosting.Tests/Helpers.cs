using System.Reflection;
using System.Text.Json;
using Bunit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

internal sealed class Helpers
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

// ReSharper disable once ClassNeverInstantiated.Global
