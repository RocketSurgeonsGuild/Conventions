using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;
using JetBrains.Annotations;
#pragma warning disable CA1031
#pragma warning disable CA2000

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class RocketHostExtensions.
    /// </summary>
    public static class RocketHostExtensions
    {
        /// <summary>
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder ConfigureRocketSurgery(
            [NotNull] this IHostBuilder builder,
            [NotNull] Action<IRocketHostBuilder> action
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(GetOrCreateBuilder(builder));
            return builder;
        }

        /// <summary>
        /// Uses the rocket booster.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="func">The function.</param>
        /// <param name="action">The action.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder UseRocketBooster(
            [NotNull] this IHostBuilder builder,
            [NotNull] Func<IHostBuilder, IRocketHostBuilder> func,
            Action<IRocketHostBuilder>? action = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var b = func(builder);
            action?.Invoke(b);
            return builder;
        }

        /// <summary>
        /// Launches the with.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="func">The function.</param>
        /// <param name="action">The action.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder LaunchWith(
            [NotNull] this IHostBuilder builder,
            [NotNull] Func<IHostBuilder, IRocketHostBuilder> func,
            Action<IRocketHostBuilder>? action = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var b = func(builder);
            action?.Invoke(b);
            return builder;
        }

        /// <summary>
        /// Uses the scanner.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scanner">The scanner.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseScanner(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] IConventionScanner scanner
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }

            return Swap(builder, GetOrCreateBuilder(builder).With(scanner));
        }

        /// <summary>
        /// Uses the assembly candidate finder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseAssemblyCandidateFinder(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] IAssemblyCandidateFinder assemblyCandidateFinder
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblyCandidateFinder == null)
            {
                throw new ArgumentNullException(nameof(assemblyCandidateFinder));
            }

            return Swap(builder, GetOrCreateBuilder(builder).With(assemblyCandidateFinder));
        }

        /// <summary>
        /// Uses the assembly provider.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseAssemblyProvider(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] IAssemblyProvider assemblyProvider
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblyProvider == null)
            {
                throw new ArgumentNullException(nameof(assemblyProvider));
            }

            return Swap(builder, GetOrCreateBuilder(builder).With(assemblyProvider));
        }

        /// <summary>
        /// Uses the diagnostic source.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseDiagnosticSource(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] DiagnosticSource diagnosticSource
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (diagnosticSource == null)
            {
                throw new ArgumentNullException(nameof(diagnosticSource));
            }

            return Swap(builder, GetOrCreateBuilder(builder).With(diagnosticSource));
        }

        /// <summary>
        /// Uses the dependency context.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseDependencyContext(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] DependencyContext dependencyContext,
            DiagnosticSource? diagnosticSource = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (dependencyContext == null)
            {
                throw new ArgumentNullException(nameof(dependencyContext));
            }

            return RocketBooster.ForDependencyContext(dependencyContext, diagnosticSource)(builder.Builder);
        }

        /// <summary>
        /// Uses the application domain.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseAppDomain(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] AppDomain appDomain,
            DiagnosticSource? diagnosticSource = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDomain == null)
            {
                throw new ArgumentNullException(nameof(appDomain));
            }

            return RocketBooster.ForAppDomain(appDomain, diagnosticSource)(builder.Builder);
        }

        /// <summary>
        /// Uses the assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseAssemblies(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] IEnumerable<Assembly> assemblies,
            DiagnosticSource? diagnosticSource = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            return RocketBooster.ForAssemblies(assemblies, diagnosticSource)(builder.Builder);
        }

        /// <summary>
        /// Uses the diagnostic logging.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseDiagnosticLogging(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] Action<ILoggingBuilder> action
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            (builder.DiagnosticSource is DiagnosticListener listener
                ? listener
                : new DiagnosticListener("DiagnosticLogger")).SubscribeWithAdapter(
                new DiagnosticListenerLoggingAdapter(
                    new ServiceCollection()
                       .AddLogging(action)
                       .BuildServiceProvider()
                       .GetRequiredService<ILoggerFactory>()
                       .CreateLogger("DiagnosticLogger")
                )
            );
            return builder;
        }

        /// <summary>
        /// Uses the command line.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseCommandLine(this IRocketHostBuilder builder)
            => builder.UseCommandLine(x => x.SuppressStatusMessages = true);

        /// <summary>
        /// Uses the command line.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseCommandLine(
            [NotNull] this IRocketHostBuilder builder,
            [NotNull] Action<ConsoleLifetimeOptions> configureOptions
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            builder.Builder.Properties[typeof(CommandLineHostedService)] = true;
            builder.Builder
               .UseConsoleLifetime()
               .ConfigureServices(services => services.Configure(configureOptions));
            return GetOrCreateBuilder(builder);
        }

        /// <summary>
        /// Runs the cli.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public static async Task<int> RunCli([NotNull] this IHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureRocketSurgery(x => x.UseCommandLine());
            using (var host = builder.Build())
            {
                var logger = host.Services.GetService<ILoggerFactory>()
                   .CreateLogger("Cli");
                var result = host.Services.GetRequiredService<CommandLineResult>();
                try
                {
                    await host.RunAsync().ConfigureAwait(false);
                    return result.Value;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Application exception");
                    return -1;
                }
            }
        }

        /// <summary>
        /// Gets the conventional host builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal static RocketHostBuilder GetConventionalHostBuilder(IHostBuilder builder)
            => GetOrCreateBuilder(builder);

        /// <summary>
        /// Gets the or create builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal static RocketHostBuilder GetOrCreateBuilder(IRocketHostBuilder builder)
            => GetOrCreateBuilder(builder.Builder);

        /// <summary>
        /// Gets the or create builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal static RocketHostBuilder GetOrCreateBuilder(IHostBuilder builder)
        {
            if (!Builders.TryGetValue(builder, out var conventionalBuilder))
            {
                var diagnosticSource = new DiagnosticListener("Rocket.Surgery.Hosting");
                var dependencyContext = DependencyContext.Default;
                var logger = new DiagnosticLogger(diagnosticSource);
                var serviceProviderDictionary = new ServiceProviderDictionary(builder.Properties);
                serviceProviderDictionary.Set<ILogger>(logger);
                serviceProviderDictionary.Set(HostType.Live);
                var assemblyCandidateFinder = new DependencyContextAssemblyCandidateFinder(dependencyContext, logger);
                var assemblyProvider = new DependencyContextAssemblyProvider(dependencyContext, logger);
                var scanner = new SimpleConventionScanner(assemblyCandidateFinder, serviceProviderDictionary, logger);
                conventionalBuilder = new RocketHostBuilder(
                    builder,
                    scanner,
                    assemblyCandidateFinder,
                    assemblyProvider,
                    diagnosticSource,
                    serviceProviderDictionary
                );

                conventionalBuilder.Set(
                    new ConfigurationOptions
                    {
                        ApplicationConfiguration =
                        {
                            b => b.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true),
                            b => b.AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true),
                            b => b.AddYamlFile("appsettings.yaml", optional: true, reloadOnChange: true),
                            b => b.AddIniFile("appsettings.ini", optional: true, reloadOnChange: true)
                        },
                        EnvironmentConfiguration =
                        {
                            (b, environmentName) => b.AddJsonFile(
                                $"appsettings.{environmentName}.json",
                                optional: true,
                                reloadOnChange: true
                            ),
                            (b, environmentName) => b.AddYamlFile(
                                $"appsettings.{environmentName}.yml",
                                optional: true,
                                reloadOnChange: true
                            ),
                            (b, environmentName) => b.AddYamlFile(
                                $"appsettings.{environmentName}.yaml",
                                optional: true,
                                reloadOnChange: true
                            ),
                            (b, environmentName) => b.AddIniFile(
                                $"appsettings.{environmentName}.ini",
                                optional: true,
                                reloadOnChange: true
                            )
                        }
                    }
                );

                var host = new RocketContext(builder);
                builder
                   .ConfigureHostConfiguration(host.ComposeHostingConvention)
                   .ConfigureHostConfiguration(host.CaptureArguments)
                   .ConfigureHostConfiguration(host.ConfigureCli)
                   .ConfigureAppConfiguration(host.ReplaceArguments)
                   .ConfigureAppConfiguration(host.ConfigureAppConfiguration)
                   .ConfigureServices(host.ConfigureServices)
                   .ConfigureServices(host.DefaultServices);
                Builders.Add(builder, conventionalBuilder);
            }

            return conventionalBuilder;
        }

        /// <summary>
        /// Swaps the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="newRocketBuilder">The new rocket builder.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal static RocketHostBuilder Swap(IRocketHostBuilder builder, RocketHostBuilder newRocketBuilder)
        {
            Builders.Remove(builder.Builder);
            Builders.Add(builder.Builder, newRocketBuilder);
            return newRocketBuilder;
        }

        private static readonly ConditionalWeakTable<IHostBuilder, RocketHostBuilder> Builders =
            new ConditionalWeakTable<IHostBuilder, RocketHostBuilder>();
    }
}