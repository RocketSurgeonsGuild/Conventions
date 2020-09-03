using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// A generic web assembly host service provider that includes the convention host builder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public abstract class WebAssemblyHostServiceProviderFactory<T> : IServiceProviderFactory<T>
    {
        private readonly IWebAssemblyHostBuilder _webAssemblyHostBuilder;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="webAssemblyHostBuilder"></param>
        protected WebAssemblyHostServiceProviderFactory(IWebAssemblyHostBuilder webAssemblyHostBuilder) => _webAssemblyHostBuilder = webAssemblyHostBuilder;

        private void ConfigureHost(IWebAssemblyHostBuilder webAssemblyHostBuilder)
        {
            var context = new RocketWebAssemblyContext(webAssemblyHostBuilder);
            context.ConfigureAppConfiguration();
            context.ConfigureServices();
        }

        /// <summary>
        /// Create the service builder using the provided values
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        protected abstract T CreateServiceBuilder(IConventionHostBuilder hostBuilder, IServiceCollection services);

        /// <inheritdoc />
        public T CreateBuilder(IServiceCollection services)
        {
            ConfigureHost(_webAssemblyHostBuilder);
            var conventionHostBuilder = _webAssemblyHostBuilder.Services.Select(z => z.ImplementationInstance).OfType<IConventionHostBuilder>().Single();
            _webAssemblyHostBuilder.Services.RemoveAll<IConventionHostBuilder>();
            return CreateServiceBuilder(conventionHostBuilder, services);
        }

        /// <inheritdoc />
        public abstract IServiceProvider CreateServiceProvider(T containerBuilder);
    }
}