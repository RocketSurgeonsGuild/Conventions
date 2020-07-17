using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IHostBuilder" />
    /// </summary>
    public static class ConfigHostBuilderExtensions
    {
        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="options"></param>
        public static IHostBuilder UseLocalConfiguration(
            this IHostBuilder hostBuilder,
            ConfigOptions options
        ) => UseLocalConfiguration(hostBuilder, () => options);

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configOptionsAction"></param>
        public static IHostBuilder UseLocalConfiguration(
            this IHostBuilder hostBuilder,
            Func<ConfigOptions> configOptionsAction
        ) => UseLocalConfiguration(hostBuilder, _ => configOptionsAction());

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configOptionsAction"></param>
        public static IHostBuilder UseLocalConfiguration(
            this IHostBuilder hostBuilder,
            Action<ConfigOptions> configOptionsAction
        ) => UseLocalConfiguration(
            hostBuilder,
            _ =>
            {
                configOptionsAction(_);
                return _;
            }
        );

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configOptionsAction"></param>
        public static IHostBuilder UseLocalConfiguration(
            this IHostBuilder hostBuilder,
            Func<ConfigOptions, ConfigOptions> configOptionsAction
        )
        {
            hostBuilder.ConfigureAppConfiguration(
                (context, configurationBuilder) =>
                {
                    var options = configOptionsAction(new ConfigOptions());
                    InsertConfigurationSourceAfter(
                        configurationBuilder.Sources,
                        sources => sources.OfType<JsonConfigurationSource>().FirstOrDefault(
                            x =>
                                string.Equals(
                                    x.Path,
                                    $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                                    StringComparison.OrdinalIgnoreCase
                                )
                        ),
                        new IConfigurationSource[]
                        {
                            new JsonConfigurationSource
                            {
                                FileProvider = configurationBuilder.GetFileProvider(),
                                Path = "appsettings.local.json",
                                Optional = true,
                                ReloadOnChange = true
                            }
                        }
                    );

                    ReplaceConfigurationSourceAt(
                        configurationBuilder.Sources,
                        sources => sources.OfType<JsonConfigurationSource>().FirstOrDefault(
                            x => string.Equals(x.Path, "appsettings.json", StringComparison.OrdinalIgnoreCase)
                        ),
                        new ProxyConfigurationBuilder(configurationBuilder).Apply(options.ApplicationConfiguration)
                           .GetAdditionalSources()
                    );

                    ReplaceConfigurationSourceAt(
                        configurationBuilder.Sources,
                        sources => sources.OfType<JsonConfigurationSource>().FirstOrDefault(
                            x =>
                                string.Equals(
                                    x.Path,
                                    $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                                    StringComparison.OrdinalIgnoreCase
                                )
                        ),
                        new ProxyConfigurationBuilder(configurationBuilder).Apply(
                            options.EnvironmentConfiguration,
                            context.HostingEnvironment.EnvironmentName
                        ).GetAdditionalSources()
                    );

                    ReplaceConfigurationSourceAt(
                        configurationBuilder.Sources,
                        sources => sources.OfType<JsonConfigurationSource>().FirstOrDefault(
                            x =>
                                string.Equals(x.Path, "appsettings.local.json", StringComparison.OrdinalIgnoreCase)
                        ),
                        new ProxyConfigurationBuilder(configurationBuilder)
                           .Apply(options.EnvironmentConfiguration, "local").GetAdditionalSources()
                    );
                }
            );

            return hostBuilder;
        }

        private static void InsertConfigurationSourceAfter<T>(
            IList<IConfigurationSource> sources,
            Func<IList<IConfigurationSource>, T> getSource,
            IEnumerable<IConfigurationSource> createSourceFrom
        )
            where T : IConfigurationSource
        {
            var source = getSource(sources);
            if (source != null)
            {
                var index =  sources.IndexOf(source) ;
                foreach (var newSource in createSourceFrom.Reverse())
                {
                    sources.Insert(index + 1, newSource);
                }
            }
            else
            {
                foreach (var newSource in createSourceFrom.Reverse())
                {
                    sources.Add(newSource);
                }
            }
        }

        private static void ReplaceConfigurationSourceAt<T>(
            IList<IConfigurationSource> sources,
            Func<IList<IConfigurationSource>, T> getSource,
            IEnumerable<IConfigurationSource> createSourceFrom
        )
            where T : IConfigurationSource
        {
            var iConfigurationSources = createSourceFrom as IConfigurationSource[] ?? createSourceFrom.ToArray();
            if (iConfigurationSources.Length == 0)
                return;
            var source = getSource(sources);
            if (source != null)
            {
                var index = sources.IndexOf(source);
                sources.RemoveAt(index);
                foreach (var newSource in iConfigurationSources.Reverse())
                {
                    sources.Insert(index, newSource);
                }
            }
            else
            {
                foreach (var newSource in iConfigurationSources.Reverse())
                {
                    sources.Add(newSource);
                }
            }
        }
    }
}