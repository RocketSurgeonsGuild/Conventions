using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.DependencyInjection;

#pragma warning disable CA1307

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class ServicesBuilderProviderFactory.
    /// Implements the <see cref="IServiceProviderFactory{IServicesBuilder}" />
    /// </summary>
    /// <seealso cref="IServiceProviderFactory{IServicesBuilder}" />
    public class ServicesBuilderProviderFactory : HostServiceProviderFactory<IServicesBuilder>
    {
        private readonly IHostBuilder _hostBuilder;
        private readonly Func<IConventionHostBuilder, IServiceCollection, IServicesBuilder> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesBuilderProviderFactory" /> class.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="func">The function.</param>
        public ServicesBuilderProviderFactory(IHostBuilder hostBuilder, Func<IConventionHostBuilder, IServiceCollection, IServicesBuilder> func) : base(hostBuilder)
        {
            _hostBuilder = hostBuilder;
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

            if (containerBuilder.Properties[typeof(ICommandLineExecutor)] is ICommandLineExecutor exec)
            {
                var result = new CommandLineResult();
                containerBuilder.Services.AddSingleton(result);
                containerBuilder.Services.AddSingleton(exec.ApplicationState);
                // Remove the hosted service that bootstraps kestrel, we are executing a command here.
                var webHostedServices = containerBuilder.Services
                   .Where(x => x.ImplementationType?.FullName?.Contains("Microsoft.AspNetCore.Hosting") == true)
                   .ToArray();
                if (!exec.IsDefaultCommand || exec.Application.IsShowingInformation)
                {
                    containerBuilder.Services.Configure<ConsoleLifetimeOptions>(x => x.SuppressStatusMessages = true);
                    foreach (var descriptor in webHostedServices)
                    {
                        containerBuilder.Services.Remove(descriptor);
                    }
                }

                var hasWebHostedService = webHostedServices.Any();
                if (containerBuilder.Properties.TryGetValue(typeof(CommandLineHostedService), out var _) ||
                    !exec.IsDefaultCommand)
                {
                    containerBuilder.Services.AddSingleton<IHostedService>(
                        _ =>
                            new CommandLineHostedService(
                                _,
                                exec,
                                _.GetRequiredService<IHostApplicationLifetime>(),
                                result,
                                hasWebHostedService
                            )
                    );
                }
            }

            return containerBuilder.Build();
        }
    }
}