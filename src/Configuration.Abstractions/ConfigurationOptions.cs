using System;
using System.Collections.Generic;
using IMsftConfigurationBuilder = Microsoft.Extensions.Configuration.IConfigurationBuilder;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// Options for configuring a hosting environment
    /// </summary>
    public class ConfigurationOptions
    {
        /// <summary>
        /// Additional settings providers to be inserted after the default application settings file (typically appsettings.json)
        /// </summary>
        public List<Func<IMsftConfigurationBuilder, IMsftConfigurationBuilder>> ApplicationConfiguration { get; } =
            new List<Func<IMsftConfigurationBuilder, IMsftConfigurationBuilder>>();

        /// <summary>
        /// Additional settings providers to be inserted after the default environment application settings file (typically
        /// appsettings.{env}.json)
        /// </summary>
        public List<Func<IMsftConfigurationBuilder, string, IMsftConfigurationBuilder>> EnvironmentConfiguration { get; } =
            new List<Func<IMsftConfigurationBuilder, string, IMsftConfigurationBuilder>>();
    }
}