using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.WebAssembly.Hosting;

internal class WrappedWebAssemblyHostBuilder : IWebAssemblyHostBuilder
{
    public WrappedWebAssemblyHostBuilder(WebAssemblyHostBuilder webAssemblyHostBuilder)
    {
        WebAssemblyHostBuilder = webAssemblyHostBuilder;
    }

    public WebAssemblyHostConfiguration Configuration => WebAssemblyHostBuilder.Configuration;
    public RootComponentMappingCollection RootComponents => WebAssemblyHostBuilder.RootComponents;
    public IServiceCollection Services => WebAssemblyHostBuilder.Services;
    public IWebAssemblyHostEnvironment HostEnvironment => WebAssemblyHostBuilder.HostEnvironment;
    public ILoggingBuilder Logging => WebAssemblyHostBuilder.Logging;

    public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder>? configure = null) where TBuilder : notnull
    {
        WebAssemblyHostBuilder.ConfigureContainer(factory, configure!);
    }

    public WebAssemblyHost Build()
    {
        return WebAssemblyHostBuilder.Build();
    }

    internal WebAssemblyHostBuilder WebAssemblyHostBuilder { get; }
}
