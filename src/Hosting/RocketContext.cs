using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using ConfigurationBuilder = Rocket.Surgery.Conventions.Configuration.ConfigurationBuilder;
using IMsftConfigurationBuilder = Microsoft.Extensions.Configuration.IConfigurationBuilder;
using MsftConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class RocketContext.
    /// </summary>
    internal class RocketContext
    {
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
                var index = sources.IndexOf(source);
                foreach (var newSource in createSourceFrom.Reverse())
                {
                    sources.Insert(index + 1, newSource);
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
            var source = getSource(sources);
            if (source != null)
            {
                var index = sources.IndexOf(source);
                sources.RemoveAt(index);
                foreach (var newSource in createSourceFrom.Reverse())
                {
                    sources.Insert(index, newSource);
                }
            }
        }

        private static void InsertConfigurationSourceBefore<T>(
            IList<IConfigurationSource> sources,
            Func<IList<IConfigurationSource>, T> getSource,
            params Func<T, IConfigurationSource>[] createSourceFrom
        )
            where T : IConfigurationSource
        {
            var source = getSource(sources);
            if (source != null)
            {
                var index = sources.IndexOf(source);
                foreach (var m in createSourceFrom.Reverse())
                {
                    sources.Insert(index, m(source));
                }
            }
        }

        private readonly IHostBuilder _hostBuilder;
        private string[]? _args;
        private ICommandLineExecutor? _exec;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketContext" /> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        public RocketContext(IHostBuilder hostBuilder) => _hostBuilder = hostBuilder;

        /// <summary>
        /// Configures any hosting builder conventions
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ComposeHostingConvention(IMsftConfigurationBuilder configurationBuilder)
        {
            var rocketHostBuilder = RocketHostExtensions.GetConventionalHostBuilder(_hostBuilder);
            Composer.Register(
                rocketHostBuilder.Scanner,
                new HostingConventionContext(rocketHostBuilder, rocketHostBuilder.Logger),
                typeof(IHostingConvention),
                typeof(HostingConventionDelegate)
            );
        }

        /// <summary>
        /// Configures the cli.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ConfigureCli(IMsftConfigurationBuilder configurationBuilder)
        {
            var rocketHostBuilder = RocketHostExtensions.GetConventionalHostBuilder(_hostBuilder);
            var clb = new CommandLineBuilder(
                rocketHostBuilder.Scanner,
                rocketHostBuilder.AssemblyProvider,
                rocketHostBuilder.AssemblyCandidateFinder,
                rocketHostBuilder.Logger,
                rocketHostBuilder.Properties
            );

            _exec = clb.Build().Parse(_args ?? Array.Empty<string>());
            _args = _exec.ApplicationState.RemainingArguments ?? Array.Empty<string>();
            configurationBuilder.AddApplicationState(_exec.ApplicationState);
            rocketHostBuilder.Properties.Add(typeof(ICommandLineExecutor), _exec);
        }

        /// <summary>
        /// Captures the arguments.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void CaptureArguments(IMsftConfigurationBuilder configurationBuilder)
        {
            var commandLineSource = configurationBuilder.Sources.OfType<CommandLineConfigurationSource>()
               .FirstOrDefault();
            if (commandLineSource != null)
            {
                _args = commandLineSource.Args.ToArray();
            }
        }

        /// <summary>
        /// Replaces the arguments.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ReplaceArguments(IMsftConfigurationBuilder configurationBuilder)
        {
            var commandLineSource = configurationBuilder.Sources.OfType<CommandLineConfigurationSource>()
               .FirstOrDefault();
            if (commandLineSource != null)
            {
                commandLineSource.Args = _args;
            }
        }

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ConfigureAppConfiguration(
            HostBuilderContext context,
            IMsftConfigurationBuilder configurationBuilder
        )
        {
            var rocketHostBuilder = RocketHostExtensions.GetConventionalHostBuilder(_hostBuilder);

            var configurationOptions = rocketHostBuilder.GetOrAdd(() => new ConfigurationOptions());

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
                new ProxyConfigurationBuilder(configurationBuilder).Apply(configurationOptions.ApplicationConfiguration)
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
                    configurationOptions.EnvironmentConfiguration,
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
                   .Apply(configurationOptions.EnvironmentConfiguration, "local").GetAdditionalSources()
            );

            InsertConfigurationSourceBefore(
                configurationBuilder.Sources,
                sources => sources.OfType<EnvironmentVariablesConfigurationSource>()
                   .FirstOrDefault(x => string.IsNullOrWhiteSpace(x.Prefix)),
                s => new EnvironmentVariablesConfigurationSource
                {
                    Prefix = "RSG_"
                }
            );

            IConfigurationSource? source = null;
            foreach (var item in configurationBuilder.Sources.Reverse())
            {
                if (item is CommandLineConfigurationSource ||
                    (item is EnvironmentVariablesConfigurationSource env && (string.IsNullOrWhiteSpace(env.Prefix) ||
                        string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase))) ||
                    (item is JsonConfigurationSource a && string.Equals(
                        a.Path,
                        "secrets.json",
                        StringComparison.OrdinalIgnoreCase
                    )))
                {
                    continue;
                }

                source = item;
                break;
            }

            var index = source == null
                ? configurationBuilder.Sources.Count - 1
                : configurationBuilder.Sources.IndexOf(source);

            var cb = new ConfigurationBuilder(
                rocketHostBuilder.Scanner,
                context.HostingEnvironment.Convert(),
                new MsftConfigurationBuilder().AddConfiguration(context.Configuration)
                   .AddConfiguration(configurationBuilder.Build()).Build(),
                configurationBuilder,
                rocketHostBuilder.Logger,
                rocketHostBuilder.Properties
            );

            configurationBuilder.Sources.Insert(
                index + 1,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build()
                }
            );
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="services">The services.</param>
        public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var rocketHostBuilder = RocketHostExtensions.GetConventionalHostBuilder(_hostBuilder);
            services.AddSingleton(rocketHostBuilder.AssemblyCandidateFinder);
            services.AddSingleton(rocketHostBuilder.AssemblyProvider);
            services.AddSingleton(rocketHostBuilder.Scanner);
#if !(NETSTANDARD2_0)
            services.AddHealthChecks();
#endif
        }

        /// <summary>
        /// Defaults the services.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="services">The services.</param>
        public void DefaultServices(HostBuilderContext context, IServiceCollection services)
        {
            var conventionalBuilder = RocketHostExtensions.GetConventionalHostBuilder(_hostBuilder);
            _hostBuilder.UseServiceProviderFactory(
                new ServicesBuilderServiceProviderFactory(
                    collection =>
                        new ServicesBuilder(
                            conventionalBuilder.Scanner,
                            conventionalBuilder.AssemblyProvider,
                            conventionalBuilder.AssemblyCandidateFinder,
                            collection,
                            context.Configuration,
                            context.HostingEnvironment.Convert(),
                            conventionalBuilder.Logger,
                            conventionalBuilder.Properties
                        )
                )
            );
        }
    }
}