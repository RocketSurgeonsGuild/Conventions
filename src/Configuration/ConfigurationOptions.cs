using System.Collections.Generic;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// Options for configuring a hosting environment
    /// </summary>
    public class ConfigOptions
    {
        /// <summary>
        /// Add an application configuration
        /// </summary>
        public ConfigOptions AddApplicationConfiguration(ConfigOptionApplicationDelegate @delegate)
        {
            ApplicationConfiguration.Add(@delegate);
            return this;
        }

        /// <summary>
        /// Add an environment configuration
        /// </summary>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public ConfigOptions AddEnvironmentConfiguration(ConfigOptionEnvironmentDelegate @delegate)
        {
            EnvironmentConfiguration.Add(@delegate);
            return this;
        }

        /// <summary>
        /// Additional settings providers to be inserted after the default application settings file (typically appsettings.json)
        /// </summary>
        internal List<ConfigOptionApplicationDelegate> ApplicationConfiguration { get; } =
            new List<ConfigOptionApplicationDelegate>();

        /// <summary>
        /// Additional settings providers to be inserted after the default environment application settings file (typically
        /// appsettings.{env}.json)
        /// </summary>
        internal List<ConfigOptionEnvironmentDelegate> EnvironmentConfiguration { get; } =
            new List<ConfigOptionEnvironmentDelegate>();
    }
}