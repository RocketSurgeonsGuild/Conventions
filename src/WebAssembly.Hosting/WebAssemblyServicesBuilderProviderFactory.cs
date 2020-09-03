using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

#pragma warning disable CA1307

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// Class WebAssemblyServicesBuilderProviderFactory.
    /// Implements the <see cref="IServiceProviderFactory{IServicesBuilder}" />
    /// </summary>
    /// <seealso cref="IServiceProviderFactory{IServicesBuilder}" />
    public class WebAssemblyServicesBuilderProviderFactory : WebAssemblyHostServiceProviderFactory<IServicesBuilder>
    {
        private readonly IWebAssemblyHostBuilder _webAssemblyHostBuilder;
        private readonly Func<IConventionHostBuilder, IServiceCollection, IServicesBuilder> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAssemblyServicesBuilderProviderFactory" /> class.
        /// </summary>
        /// <param name="webAssemblyHostBuilder"></param>
        /// <param name="func">The function.</param>
        public WebAssemblyServicesBuilderProviderFactory(IWebAssemblyHostBuilder webAssemblyHostBuilder, Func<IConventionHostBuilder, IServiceCollection, IServicesBuilder> func)
            : base(webAssemblyHostBuilder)
        {
            _webAssemblyHostBuilder = webAssemblyHostBuilder;
            _func = func;
        }

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="services">The collection of services</param>
        /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
        protected override IServicesBuilder CreateServiceBuilder(IConventionHostBuilder hostBuilder, IServiceCollection services) => _func(hostBuilder, services);

        /// <summary>
        /// Creates the service provider.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>IServiceProvider.</returns>
        public override IServiceProvider CreateServiceProvider([NotNull] IServicesBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            return containerBuilder.Build();
        }
    }
}