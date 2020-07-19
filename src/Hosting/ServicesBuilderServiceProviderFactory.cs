using System;
using System.CommandLine.Parsing;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

#pragma warning disable CA1307

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class ServicesBuilderServiceProviderFactory.
    /// Implements the <see cref="IServiceProviderFactory{IServicesBuilder}" />
    /// </summary>
    /// <seealso cref="IServiceProviderFactory{IServicesBuilder}" />
    public class ServicesBuilderServiceProviderFactory : IServiceProviderFactory<IServicesBuilder>
    {
        private readonly Func<IServiceCollection, IServicesBuilder> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesBuilderServiceProviderFactory" /> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public ServicesBuilderServiceProviderFactory(Func<IServiceCollection, IServicesBuilder> func) => _func = func;

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
        public IServicesBuilder CreateBuilder(IServiceCollection services) => _func(services);

        /// <summary>
        /// Creates the service provider.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>IServiceProvider.</returns>
        public IServiceProvider CreateServiceProvider([NotNull] IServicesBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }
            var webHostedServices = containerBuilder.Services
               .Where(x => x.ImplementationType?.FullName?.StartsWith("Microsoft.AspNetCore.Hosting", StringComparison.Ordinal) == true)
               .ToArray();
            var hasWebHostedService = webHostedServices.Any();

            //if (containerBuilder.Properties[typeof(ParseResult)] is ParseResult parseResult)
            if (containerBuilder.Get<bool>("DefaultShellCommand") && hasWebHostedService)
            {
                var commandLineHostedService = containerBuilder.Services
                   .Where(x => x.ImplementationType == typeof(ShellHostedService));
                // Remove the shell hosted service that bootstraps kestrel, we are executing a command here.
                foreach (var descriptor in commandLineHostedService)
                {
                    containerBuilder.Services.Remove(descriptor);
                }
            }
            else
            {
                // Remove the hosted service that bootstraps kestrel, we are executing a command here.
                foreach (var descriptor in webHostedServices)
                {
                    containerBuilder.Services.Remove(descriptor);
                }
            }

            return containerBuilder.Build();
        }
    }
}