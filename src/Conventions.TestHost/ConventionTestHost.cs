using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Logging;

namespace Rocket.Surgery.Conventions.TestHost
{
    /// <summary>
    /// Class ConventionTestHostBuilder.
    /// Implements the <see cref="ConventionHostBuilder{ConventionTestHostBuilder}" />
    /// </summary>
    /// <seealso cref="ConventionHostBuilder{ConventionTestHostBuilder}" />
    public class ConventionTestHost : ConventionHostBuilder<ConventionTestHost>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRocketEnvironment _environment;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionTestHost"/> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="serviceProperties">The service properties.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="environment">The environment.</param>
        internal ConventionTestHost(IConventionScanner scanner, IAssemblyCandidateFinder assemblyCandidateFinder, IAssemblyProvider assemblyProvider, DiagnosticSource diagnosticSource, IServiceProviderDictionary serviceProperties, ILoggerFactory loggerFactory, IRocketEnvironment environment) : base(scanner, assemblyCandidateFinder, assemblyProvider, diagnosticSource, serviceProperties)
        {
            serviceProperties.Set(HostType.UnitTestHost);
            _loggerFactory = loggerFactory;
            _environment = environment;
            _logger = _loggerFactory.CreateLogger(nameof(ConventionTestHost));
        }

        /// <summary>
        /// Use the <see cref="BasicConventionScanner" /> to not automatically load conventions from attributes.
        /// </summary>
        /// <returns></returns>
        public ConventionTestHost ExcludeConventionAttributes()
        {
            return new ConventionTestHost(new BasicConventionScanner(ServiceProperties), AssemblyCandidateFinder, AssemblyProvider, DiagnosticSource, ServiceProperties, _loggerFactory, _environment);
        }

        /// <summary>
        /// <para>Use the <see cref="SimpleConventionScanner" /> to automatically load conventions from attributes.</para>
        /// <para>This is the default</para>
        /// </summary>
        public ConventionTestHost IncludeConventionAttributes()
        {
            return new ConventionTestHost(new SimpleConventionScanner(AssemblyCandidateFinder, ServiceProperties, _logger), AssemblyCandidateFinder, AssemblyProvider, DiagnosticSource, ServiceProperties, _loggerFactory, _environment);
        }


        /// <summary>
        /// Build the configuration and service provider based on the input environment.
        /// </summary>
        public (IConfigurationRoot Configuration, IServiceProvider ServiceProvider) Build()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var cb = new Rocket.Surgery.Extensions.Configuration.ConfigurationBuilder(
                Scanner,
                _environment,
                new ConfigurationRoot(new List<IConfigurationProvider>()),
                configurationBuilder,
                _logger,
                ServiceProperties
            );

            cb.Build();
            var configuration = configurationBuilder.Build();

            var servicesBuilder = new ServicesBuilder(
                Scanner,
                AssemblyProvider,
                AssemblyCandidateFinder,
                new ServiceCollection(),
                configuration,
                _environment,
                _logger,
                ServiceProperties
            );

            servicesBuilder.Services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.Services.AddSingleton(_loggerFactory);
            });

            var serviceProvider = servicesBuilder.Build();

            return (configuration, serviceProvider);
        }
    }
}
