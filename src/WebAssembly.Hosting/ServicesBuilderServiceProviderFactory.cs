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
    /// Class ServicesBuilderServiceProviderFactory.
    /// Implements the <see cref="IServiceProviderFactory{IServicesBuilder}" />
    /// </summary>
    /// <seealso cref="IServiceProviderFactory{IServicesBuilder}" />
    public class ServicesBuilderServiceProviderFactory : WebAssemblyServiceProviderFactory<IServicesBuilder>
    {
        private readonly IWebAssemblyHostBuilder _webAssemblyHostBuilder;
        private readonly Func<IConventionHostBuilder, IServiceCollection, IServicesBuilder> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesBuilderServiceProviderFactory" /> class.
        /// </summary>
        /// <param name="webAssemblyHostBuilder"></param>
        /// <param name="func">The function.</param>
        public ServicesBuilderServiceProviderFactory(IWebAssemblyHostBuilder webAssemblyHostBuilder, Func<IConventionHostBuilder, IServiceCollection, IServicesBuilder> func)
            : base(webAssemblyHostBuilder)
        {
            _webAssemblyHostBuilder = webAssemblyHostBuilder;
            _func = func;
        }

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="webAssemblyHostBuilder"></param>
        /// <param name="services">The collection of services</param>
        /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
        protected override IServicesBuilder CreateServiceBuilder(IConventionHostBuilder webAssemblyHostBuilder, IServiceCollection services) => _func(webAssemblyHostBuilder, services);

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