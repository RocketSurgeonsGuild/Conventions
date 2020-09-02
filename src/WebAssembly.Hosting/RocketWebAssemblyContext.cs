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
        /// Construct and compose hosting conventions
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void ComposeWebAssemblyHostingConvention()
        {
            Composer.Register(
                ConventionHostBuilder.Scanner,
                new WebAssemblyHostingConventionContext(ConventionHostBuilder, ConventionHostBuilder.Get<IWebAssemblyHostBuilder>()!, ConventionHostBuilder.Get<ILogger>()!),
                typeof(IWebAssemblyHostingConvention),
                typeof(WebAssemblyHostingConventionDelegate)
            );
        }

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
        }
    }
}