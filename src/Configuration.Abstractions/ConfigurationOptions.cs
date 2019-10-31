using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

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
        public List<SettingsConfigurationSourceProvider> SettingsConfigurationSourceProviders { get; } = new List<SettingsConfigurationSourceProvider>();

        /// <summary>
        /// Additional settings providers to be inserted after the default environment application settings file (typically appsettings.{env}.json)
        /// </summary>
        public List<EnvironmentSettingsConfigurationSourceProvider> EnvironmentSettingsConfigurationSourceProviders { get; } = new List<EnvironmentSettingsConfigurationSourceProvider>();

        /// <summary>
        /// Additional settings providers to be inserted after the environment application settings file and after the first local config file (typically appsettings.local.json)
        /// .local files are not generally checked into source control.
        /// </summary>
        public List<SettingsConfigurationSourceProvider> LocalSettingsConfigurationSourceProvider { get; } = new List<SettingsConfigurationSourceProvider>();
    }
}
