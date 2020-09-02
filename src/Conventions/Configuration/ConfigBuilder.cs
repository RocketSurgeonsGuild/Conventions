using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Configuration
{
    /// <summary>
    /// Logging Builder
    /// Implements the <see cref="ConventionContainerBuilder{TBuilder,TConvention,TDelegate}" />
    /// Implements the <see cref="IConfigurationBuilder" />
    /// Implements the <see cref="IConfigConvention" />
    /// Implements the <see cref="IConfigConventionContext" />
    /// Implements the <see cref="ConfigConventionDelegate" />
    /// </summary>
    /// <seealso
    ///     cref="ConventionContainerBuilder{IConfigurationBuilder, IConfigurationConvention, ConfigurationConventionDelegate}" />
    /// <seealso cref="IConfigurationBuilder" />
    /// <seealso cref="IConfigConvention" />
    /// <seealso cref="IConfigConventionContext" />
    /// <seealso cref="ConfigConventionDelegate" />
    public sealed class ConfigBuilder :
        ConventionContainerBuilder<ConfigBuilder, IConfigConvention, ConfigConventionDelegate>,
        IConfigConventionContext
    {
        private readonly IConfigurationBuilder _builder = new ConfigurationBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigBuilder" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">
        /// scanner
        /// or
        /// builder
        /// or
        /// configuration
        /// or
        /// diagnosticSource
        /// </exception>
        public ConfigBuilder(
            IConventionScanner scanner,
            IConfiguration configuration,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        ) : base(scanner, properties)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
        }

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
        /// Builds this instance.
        /// </summary>
        public IConfigurationRoot Build()
        {
            Composer.Register(Scanner, this, typeof(IConfigConvention), typeof(ConfigConventionDelegate));
            return _builder.Build();
        }

        IConfigurationBuilder IConfigurationBuilder.Add(IConfigurationSource source) => _builder.Add(source);
        IDictionary<string, object> IConfigurationBuilder.Properties => _builder.Properties;
        IList<IConfigurationSource> IConfigurationBuilder.Sources => _builder.Sources;
    }
}