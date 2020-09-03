using System;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    internal class WrappedWebAssemblyHostBuilder : IWebAssemblyHostBuilder
    {
        private readonly WebAssemblyHostBuilder _webAssemblyHostBuilder;
        public WrappedWebAssemblyHostBuilder(WebAssemblyHostBuilder webAssemblyHostBuilder) => _webAssemblyHostBuilder = webAssemblyHostBuilder;

        public WebAssemblyHostConfiguration Configuration => _webAssemblyHostBuilder.Configuration;
        public RootComponentMappingCollection RootComponents => _webAssemblyHostBuilder.RootComponents;
        public IServiceCollection Services => _webAssemblyHostBuilder.Services;
        public IWebAssemblyHostEnvironment HostEnvironment => _webAssemblyHostBuilder.HostEnvironment;
        public ILoggingBuilder Logging => _webAssemblyHostBuilder.Logging;
        public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder>? configure = null)
            => _webAssemblyHostBuilder.ConfigureContainer(factory, configure!);
        public WebAssemblyHost Build() => _webAssemblyHostBuilder.Build();
        internal WebAssemblyHostBuilder WebAssemblyHostBuilder => _webAssemblyHostBuilder;
    }
}