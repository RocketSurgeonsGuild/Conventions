using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    [PublicAPI]
    public abstract class WebAssemblyServiceProviderFactory<T> : IServiceProviderFactory<T>
    {
        private readonly IWebAssemblyHostBuilder _webAssemblyHostBuilder;

        protected WebAssemblyServiceProviderFactory(IWebAssemblyHostBuilder webAssemblyHostBuilder) => _webAssemblyHostBuilder = webAssemblyHostBuilder;

        private void ConfigureHost(IWebAssemblyHostBuilder webAssemblyHostBuilder)
        {
            var context = new RocketWebAssemblyContext(webAssemblyHostBuilder);
            context.ComposeWebAssemblyHostingConvention();
            context.ConfigureAppConfiguration();
            context.ConfigureServices();
        }

        protected abstract T CreateServiceBuilder(IConventionHostBuilder hostBuilder, IServiceCollection services);

        public T CreateBuilder(IServiceCollection services)
        {
            ConfigureHost(_webAssemblyHostBuilder);
            var conventionHostBuilder = _webAssemblyHostBuilder.Services.Select(z => z.ImplementationInstance).OfType<IConventionHostBuilder>().Single();
            _webAssemblyHostBuilder.Services.RemoveAll<IConventionHostBuilder>();
            return CreateServiceBuilder(conventionHostBuilder, services);
        }

        public abstract IServiceProvider CreateServiceProvider(T containerBuilder);
    }
}