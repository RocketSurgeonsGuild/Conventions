using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Rocket.Surgery.Extensions.Configuration;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class RocketContext.
    /// </summary>
    internal class RocketContext
    {
        private readonly IHostBuilder _hostBuilder;
        private Func<IConventionContext> getContext;

        private string[]? _args;
        private ICommandLineExecutor? _exec;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketContext" /> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="conventionContextBuilder"></param>
        public RocketContext(IHostBuilder hostBuilder, ConventionContextBuilder conventionContextBuilder)
        {
            _hostBuilder = hostBuilder;
            IConventionContext? context = null;
            getContext = () =>
            {
                context ??= ConventionContext.From(conventionContextBuilder);
                return context;
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketContext" /> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="context"></param>
        public RocketContext(IHostBuilder hostBuilder, IConventionContext context)
        {
            _hostBuilder = hostBuilder;
            getContext = () => context;
        }

        /// <summary>
        /// Construct and compose hosting conventions
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ComposeHostingConvention([NotNull] IConfigurationBuilder configurationBuilder)
        {
            _hostBuilder.ApplyConventions(getContext());
        }

        /// <summary>
        /// Configures the cli.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ConfigureCli([NotNull] IConfigurationBuilder configurationBuilder)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            _exec = getContext().CreateCommandLine().Parse(_args ?? Array.Empty<string>());
            _args = _exec.ApplicationState.RemainingArguments ?? Array.Empty<string>();
            configurationBuilder.AddApplicationState(_exec.ApplicationState);
            getContext().Properties.Set(_exec);
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

            getContext().Properties.AddIfMissing(context.HostingEnvironment);
            configurationBuilder.UseLocalConfiguration(
                getContext().GetOrAdd(() => new ConfigOptions()).UseEnvironment(context.HostingEnvironment.EnvironmentName)
            );

            // Insert after all the normal configuration but before the environment specific configuration

            IConfigurationSource? source = null;
            foreach (var item in configurationBuilder.Sources.Reverse())
            {
                if (item is CommandLineConfigurationSource ||
                    ( item is EnvironmentVariablesConfigurationSource env && ( string.IsNullOrWhiteSpace(env.Prefix) ||
                        string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) ) ||
                    ( item is JsonConfigurationSource a && string.Equals(
                        a.Path,
                        "secrets.json",
                        StringComparison.OrdinalIgnoreCase
                    ) ))
                {
                    continue;
                }

                source = item;
                break;
            }

            var index = source == null
                ? configurationBuilder.Sources.Count - 1
                : configurationBuilder.Sources.IndexOf(source);

            var cb = new ConfigurationBuilder().ApplyConventions(getContext(), configurationBuilder.Build());

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

            getContext().Properties.AddIfMissing(context.Configuration);
            services.AddHealthChecks();

            services.ApplyConventions(getContext());
            new LoggingBuilder(services).ApplyConventions(getContext());

            if (getContext().Properties.TryGetValue(typeof(ICommandLineExecutor), out var o) && o is ICommandLineExecutor exec)
            {
                var result = new CommandLineResult();
                services.AddSingleton(result);
                services.AddSingleton(exec.ApplicationState);
                // Remove the hosted service that bootstraps kestrel, we are executing a command here.
                var webHostedServices = services
                   .Where(x => x.ImplementationType?.FullName?.Contains("Microsoft.AspNetCore.Hosting") == true)
                   .ToArray();
                if (!exec.IsDefaultCommand || exec.Application.IsShowingInformation)
                {
                    services.Configure<ConsoleLifetimeOptions>(x => x.SuppressStatusMessages = true);
                    foreach (var descriptor in webHostedServices)
                    {
                        services.Remove(descriptor);
                    }
                }

                var hasWebHostedService = webHostedServices.Any();
                if (getContext().Properties.TryGetValue(typeof(CommandLineHostedService), out var _) || !exec.IsDefaultCommand)
                {
                    services.AddSingleton<IHostedService>(
                        _ =>
                            new CommandLineHostedService(
                                _,
                                exec,
                                _.GetRequiredService<IHostApplicationLifetime>(),
                                result,
                                hasWebHostedService
                            )
                    );
                }
            }
        }
    }
}