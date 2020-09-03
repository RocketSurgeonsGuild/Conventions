using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// A generic host service provider that includes the convention host builder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public abstract class HostServiceProviderFactory<T> : IServiceProviderFactory<T>
    {
        private readonly IHostBuilder _hostBuilder;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="webAssemblyHostBuilder"></param>
        protected HostServiceProviderFactory(IHostBuilder webAssemblyHostBuilder) => _hostBuilder = webAssemblyHostBuilder;

        /// <summary>
        /// Create the service builder using the provided values
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        protected abstract T CreateServiceBuilder(IConventionHostBuilder hostBuilder, IServiceCollection services);

        /// <inheritdoc />
        public T CreateBuilder(IServiceCollection services) => CreateServiceBuilder(_hostBuilder.Properties.GetConventions(), services);

        /// <inheritdoc />
        public abstract IServiceProvider CreateServiceProvider(T containerBuilder);
    }
}