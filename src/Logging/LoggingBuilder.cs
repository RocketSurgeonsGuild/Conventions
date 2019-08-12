﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Logging;

namespace Rocket.Surgery.Extensions.Logging
{
    /// <summary>
    /// Logging Builder
    /// Implements the <see cref="ConventionBuilder{ILoggingBuilder, ILoggingConvention, LoggingConventionDelegate}" />
    /// Implements the <see cref="ILoggingBuilder" />
    /// Implements the <see cref="ILoggingConvention" />
    /// Implements the <see cref="ILoggingConventionContext" />
    /// Implements the <see cref="LoggingConventionDelegate" />
    /// </summary>
    /// <seealso cref="ConventionBuilder{ILoggingBuilder, ILoggingConvention, LoggingConventionDelegate}" />
    /// <seealso cref="ILoggingBuilder" />
    /// <seealso cref="ILoggingConvention" />
    /// <seealso cref="ILoggingConventionContext" />
    /// <seealso cref="LoggingConventionDelegate" />
    public class LoggingBuilder : ConventionBuilder<ILoggingBuilder, ILoggingConvention, LoggingConventionDelegate>, ILoggingBuilder, ILoggingConventionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingBuilder"/> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">
        /// environment
        /// or
        /// services
        /// or
        /// configuration
        /// or
        /// diagnosticSource
        /// </exception>
        public LoggingBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IRocketEnvironment environment,
            IConfiguration configuration,
            ILogger diagnosticSource,
            IDictionary<object, object> properties) : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
        }

        /// <summary>
        /// Gets the <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> where Logging services are configured.
        /// </summary>
        /// <value>The services.</value>
        public IServiceCollection Services { get; }
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }
        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        public IRocketEnvironment Environment { get; }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            new ConventionComposer(Scanner).Register(
                this,
                typeof(ILoggingConvention),
                typeof(LoggingConventionDelegate)
            );
        }
    }
}
