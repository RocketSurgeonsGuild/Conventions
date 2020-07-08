using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;

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
            [NotNull] Action<IConventionHostBuilder> action
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
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder ConfigureRocketSurgery<TScanner>(
            [NotNull] this IHostBuilder builder,
            [NotNull] Action<IConventionHostBuilder> action
        ) where TScanner : IConventionScanner
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(GetOrCreateBuilder(builder, typeof(TScanner)));
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
            [NotNull] Func<IHostBuilder, IConventionHostBuilder> func,
            Action<IConventionHostBuilder>? action = null
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
            [NotNull] Func<IHostBuilder, IConventionHostBuilder> func,
            Action<IConventionHostBuilder>? action = null
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
        /// <remarks>
        /// Swapping out the scanner must be done very early on the bootstrapping process.
        /// This can result in some unexpected behaviors.
        /// </remarks>
        /// <param name="builder">The builder.</param>
        /// <param name="scanner">The scanner.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseScannerUnsafe(
            [NotNull] this IConventionHostBuilder builder,
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
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseAssemblyCandidateFinder(
            [NotNull] this IConventionHostBuilder builder,
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
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseAssemblyProvider(
            [NotNull] this IConventionHostBuilder builder,
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
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseDiagnosticSource(
            [NotNull] this IConventionHostBuilder builder,
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
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseDependencyContext(
            [NotNull] this IConventionHostBuilder builder,
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

            return RocketBooster.ForDependencyContext(dependencyContext, diagnosticSource)(builder.Get<IHostBuilder>());
        }

        /// <summary>
        /// Uses the application domain.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseAppDomain(
            [NotNull] this IConventionHostBuilder builder,
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

            return RocketBooster.ForAppDomain(appDomain, diagnosticSource)(builder.Get<IHostBuilder>());
        }

        /// <summary>
        /// Uses the assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseAssemblies(
            [NotNull] this IConventionHostBuilder builder,
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

            return RocketBooster.ForAssemblies(assemblies, diagnosticSource)(builder.Get<IHostBuilder>());
        }

        /// <summary>
        /// Uses the diagnostic logging.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseDiagnosticLogging(
            [NotNull] this IConventionHostBuilder builder,
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

            ( builder.DiagnosticSource is DiagnosticListener listener
                ? listener
                : new DiagnosticListener("DiagnosticLogger") ).SubscribeWithAdapter(
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
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseCommandLine(this IConventionHostBuilder builder)
            => builder.UseCommandLine(x => x.SuppressStatusMessages = true);

        /// <summary>
        /// Uses the command line.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseCommandLine(
            [NotNull] this IConventionHostBuilder builder,
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

            builder.ServiceProperties[typeof(CommandLineHostedService)] = true;
            builder.Get<IHostBuilder>()
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
        /// Gets the or create builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scannerType">The default type of scanner to use.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal static RocketHostBuilder GetOrCreateBuilder(IConventionHostBuilder builder, Type? scannerType = null)
            => GetOrCreateBuilder(builder.Get<IHostBuilder>(), scannerType);

        /// <summary>
        /// Gets the or create builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scannerType">The default type of scanner to use.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal static RocketHostBuilder GetOrCreateBuilder(IHostBuilder builder, Type? scannerType = null)
        {
            var conventionHostBuilder = builder.GetConventions();
            if (conventionHostBuilder is RocketHostBuilder rocketHostBuilder)
            {
                return rocketHostBuilder;
            }

            if (conventionHostBuilder is UninitializedConventionHostBuilder uninitializedHostBuilder)
            {
                var diagnosticSource = new DiagnosticListener("Rocket.Surgery.Hosting");
                var dependencyContext = DependencyContext.Default;
                var logger = new DiagnosticLogger(diagnosticSource);
                var serviceProviderDictionary = uninitializedHostBuilder.ServiceProperties;
                serviceProviderDictionary.Set<ILogger>(logger);
                serviceProviderDictionary.Set(HostType.Live);
                serviceProviderDictionary.Set(builder);
                var assemblyCandidateFinder = new DependencyContextAssemblyCandidateFinder(dependencyContext, logger);
                var assemblyProvider = new DependencyContextAssemblyProvider(dependencyContext, logger);
                var scanner = ConvertTo(
                    uninitializedHostBuilder.Scanner,
                    assemblyCandidateFinder,
                    serviceProviderDictionary,
                    logger,
                    scannerType ?? typeof(BasicConventionScanner)
                );

                var host = new RocketContext(builder);
                builder
                   .ConfigureHostConfiguration(host.ComposeHostingConvention)
                   .ConfigureHostConfiguration(host.CaptureArguments)
                   .ConfigureHostConfiguration(host.ConfigureCli)
                   .ConfigureAppConfiguration(host.ReplaceArguments)
                   .UseLocalConfiguration(() => serviceProviderDictionary.GetOrAdd(() => new ConfigOptions()))
                   .ConfigureAppConfiguration(host.ConfigureAppConfiguration)
                   .ConfigureServices(host.ConfigureServices)
                   .ConfigureServices(host.DefaultServices);

                rocketHostBuilder = new RocketHostBuilder(
                    builder,
                    scanner,
                    assemblyCandidateFinder,
                    assemblyProvider,
                    diagnosticSource,
                    serviceProviderDictionary
                );
                serviceProviderDictionary[typeof(IConventionHostBuilder)] = rocketHostBuilder;
                serviceProviderDictionary.Set(scanner);
                serviceProviderDictionary.Set<IAssemblyCandidateFinder>(assemblyCandidateFinder);
                serviceProviderDictionary.Set<IAssemblyProvider>(assemblyProvider);
                serviceProviderDictionary.Set<ILogger>(logger);
                serviceProviderDictionary.Set(diagnosticSource);

                return rocketHostBuilder;
            }

            throw new NotSupportedException("Unsupported configuration");
        }


        private static IConventionScanner ConvertTo(
            UninitializedConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceProviderDictionary serviceProviderDictionary,
            ILogger logger,
            Type toType)
        {
            if (toType == typeof(BasicConventionScanner))
            {
                return new BasicConventionScanner(
                    serviceProviderDictionary,
                    scanner._appendedConventions,
                    scanner._prependedConventions,
                    scanner._exceptConventions
                );
            }

            if (toType == typeof(SimpleConventionScanner))
            {
                return new SimpleConventionScanner(
                    assemblyCandidateFinder,
                    serviceProviderDictionary,
                    logger,
                    scanner._appendedConventions,
                    scanner._prependedConventions,
                    scanner._exceptConventions,
                    scanner._exceptAssemblyConventions
                );
            }

            if (toType == typeof(AggregateConventionScanner))
            {
                return new AggregateConventionScanner(
                    assemblyCandidateFinder,
                    serviceProviderDictionary,
                    logger,
                    scanner._appendedConventions,
                    scanner._prependedConventions,
                    scanner._exceptConventions,
                    scanner._exceptAssemblyConventions
                );
            }

            throw new NotSupportedException("Scanner type is not supported by calling this method, try UseScannerUnsafe instead.");
        }

        /// <summary>
        /// Swaps the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="newRocketBuilder">The new rocket builder.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal static IConventionHostBuilder Swap(
            IConventionHostBuilder builder,
            IConventionHostBuilder newRocketBuilder
        )
        {
            builder.Set(newRocketBuilder);
            return newRocketBuilder;
        }
    }
}