using System;
using System.Collections.Generic;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration.Memory;
using Rocket.Surgery.Conventions.Shell;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class RocketContext.
    /// </summary>
    internal class RocketContext
    {
        private readonly IHostBuilder _hostBuilder;

        private string[]? _args;
        //private ParseResult? _parseResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketContext" /> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        public RocketContext(IHostBuilder hostBuilder) => _hostBuilder = hostBuilder;

        /// <summary>
        /// Construct and compose hosting conventions
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ComposeHostingConvention([NotNull] IConfigurationBuilder configurationBuilder)
        {
            var rocketHostBuilder = _hostBuilder.GetConventions();
            Composer.Register(
                rocketHostBuilder.Scanner,
                new HostingConventionContext(rocketHostBuilder, rocketHostBuilder.Get<IHostBuilder>(), rocketHostBuilder.Get<ILogger>()),
                typeof(IHostingConvention),
                typeof(HostingConventionDelegate)
            );
        }

        /// <summary>
        /// Captures the arguments.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void CaptureArguments([NotNull] IConfigurationBuilder configurationBuilder)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            var commandLineSource = configurationBuilder.Sources.OfType<CommandLineConfigurationSource>()
               .FirstOrDefault();
            if (commandLineSource != null)
            {
                _args = commandLineSource.Args.ToArray();
            }
        }

        /// <summary>
        /// Configures the cli.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ConfigureShell([NotNull] IConfigurationBuilder configurationBuilder)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            var rocketHostBuilder = _hostBuilder.GetConventions();
            var clb = new ShellBuilder(
                rocketHostBuilder.Scanner,
                rocketHostBuilder.AssemblyProvider,
                rocketHostBuilder.AssemblyCandidateFinder,
                rocketHostBuilder.Get<ILogger>(),
                rocketHostBuilder.ServiceProperties
            );

            var parseResult = clb.Parse(_args ?? Array.Empty<string>());
            var isDefault = parseResult.RootCommandResult.Command == parseResult.CommandResult.Command;
            rocketHostBuilder.AddIfMissing("RunAsShell", () => !isDefault);
            rocketHostBuilder.Set("DefaultShellCommand", isDefault);
            if (rocketHostBuilder.Get<bool>("RunAsShell"))
            {
                _hostBuilder.ConfigureServices(
                    services =>
                    {
                        services.AddSingleton(parseResult);
                        services.AddSingleton(clb.Console);
                        services.AddSingleton(_ => new InvocationContext(parseResult, clb.Console));
                        services.AddSingleton(_ => _.GetRequiredService<InvocationContext>().BindingContext);
                        services.AddSingleton(_ => _.GetRequiredService<InvocationContext>().Console);
                        services.AddTransient(_ => _.GetRequiredService<InvocationContext>().InvocationResult);

                        var result = new CommandLineResult();
                        services.AddSingleton(result);
                        services.AddSingleton<IHostedService, ShellHostedService>();
                    }
                );
            }

            _args = parseResult.UnparsedTokens.ToArray();
            configurationBuilder.AddShellState(parseResult);
            rocketHostBuilder.Set(parseResult);
        }

        /// <summary>
        /// Replaces the arguments.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ReplaceArguments([NotNull] IConfigurationBuilder configurationBuilder)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

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
            [NotNull] HostBuilderContext context,
            [NotNull] IConfigurationBuilder configurationBuilder
        )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            var rocketHostBuilder = _hostBuilder.GetConventions();
            // Insert after all the normal configuration but before the environment specific configuration

            IConfigurationSource? source = null;
            foreach (var item in configurationBuilder.Sources.Reverse())
            {
                if (item is CommandLineConfigurationSource
                 || ( item is EnvironmentVariablesConfigurationSource env &&
                        ( string.IsNullOrWhiteSpace(env.Prefix) || string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) )
                 || ( item is JsonConfigurationSource a && string.Equals(a.Path, "secrets.json", StringComparison.OrdinalIgnoreCase) )
                )
                {
                    continue;
                }

                source = item;
                break;
            }

            var index = source == null
                ? configurationBuilder.Sources.Count - 1
                : configurationBuilder.Sources.IndexOf(source);

            var cb = new ConfigBuilder(
                rocketHostBuilder.Scanner,
                context.HostingEnvironment,
                new ConfigurationBuilder()
                   .AddConfiguration(context.Configuration, false)
                   .AddConfiguration(configurationBuilder.Build(), true)
                   .Build(),
                rocketHostBuilder.Get<ILogger>(),
                rocketHostBuilder.ServiceProperties
            );

            configurationBuilder.Sources.Insert(
                index + 1,
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
        /// <param name="context">The context.</param>
        /// <param name="services">The services.</param>
        public void ConfigureServices([NotNull] HostBuilderContext context, [NotNull] IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var rocketHostBuilder = _hostBuilder.GetConventions();
            services.AddSingleton(rocketHostBuilder.AssemblyCandidateFinder);
            services.AddSingleton(rocketHostBuilder.AssemblyProvider);
            services.AddSingleton(rocketHostBuilder.Scanner);
            services.AddHealthChecks();
        }

        /// <summary>
        /// Defaults the services.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="services">The services.</param>
        public IServiceProviderFactory<IServicesBuilder> DefaultServices([NotNull] HostBuilderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var conventionalBuilder = _hostBuilder.GetConventions();
            return new ServicesBuilderServiceProviderFactory(
                collection =>
                    new ServicesBuilder(
                        conventionalBuilder.Scanner,
                        conventionalBuilder.AssemblyProvider,
                        conventionalBuilder.AssemblyCandidateFinder,
                        collection,
                        context.Configuration,
                        context.HostingEnvironment,
                        conventionalBuilder.Get<ILogger>(),
                        conventionalBuilder.ServiceProperties
                    )
            );
        }
    }
}