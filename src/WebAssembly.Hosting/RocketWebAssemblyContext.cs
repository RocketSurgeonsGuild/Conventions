using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// Class RocketWebAssemblyContext.
    /// </summary>
    internal class RocketWebAssemblyContext
    {
        private readonly IWebAssemblyHostBuilder _hostBuilder;
        private IConventionHostBuilder ConventionHostBuilder => _hostBuilder.GetConventions();

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketWebAssemblyContext" /> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        public RocketWebAssemblyContext(IWebAssemblyHostBuilder hostBuilder) => _hostBuilder = hostBuilder;

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        public void ConfigureAppConfiguration()
        {
            var cb = new ConfigBuilder(
                ConventionHostBuilder.Scanner,
                _hostBuilder.Configuration,
                ConventionHostBuilder.Get<ILogger>()!,
                ConventionHostBuilder.ServiceProperties
            );

            _hostBuilder.Configuration.Add(
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true
                }
            );
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        public void ConfigureServices()
        {
            _hostBuilder.Services.AddSingleton(ConventionHostBuilder.AssemblyCandidateFinder);
            _hostBuilder.Services.AddSingleton(ConventionHostBuilder.AssemblyProvider);
            _hostBuilder.Services.AddSingleton(ConventionHostBuilder.Scanner);

            Composer.Register<ILoggingConventionContext, ILoggingConvention, LoggingConventionDelegate>(
                ConventionHostBuilder.Scanner,
                new LoggingBuilder(
                    ConventionHostBuilder.Scanner,
                    ConventionHostBuilder.AssemblyProvider,
                    ConventionHostBuilder.AssemblyCandidateFinder,
                    _hostBuilder.Services,
                    _hostBuilder.Configuration,
                    ConventionHostBuilder.DiagnosticLogger,
                    ConventionHostBuilder.ServiceProperties
                )
            );
        }
    }
}