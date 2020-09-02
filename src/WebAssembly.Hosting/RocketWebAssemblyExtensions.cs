using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

#pragma warning disable CA1031
#pragma warning disable CA2000

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// Class RocketWebAssemblyExtensions.
    /// </summary>
    public static class RocketWebAssemblyExtensions
    {
        /// <summary>
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder ConfigureRocketSurgery([NotNull] this IWebAssemblyHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return ConfigureRocketSurgery(builder, _ => { });
        }

        /// <summary>
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder ConfigureRocketSurgery([NotNull] this WebAssemblyHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return ConfigureRocketSurgery(new WrappedWebAssemblyHostBuilder(builder), _ => { });
        }

        /// <summary>
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder ConfigureRocketSurgery([NotNull] this IWebAssemblyHostBuilder builder, [NotNull] Action<IConventionHostBuilder> action)
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
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder ConfigureRocketSurgery([NotNull] this WebAssemblyHostBuilder builder, [NotNull] Action<IConventionHostBuilder> action)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var innerBuilder = new WrappedWebAssemblyHostBuilder(builder);
            action(GetOrCreateBuilder(innerBuilder));
            return innerBuilder;
        }

        /// <summary>
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder ConfigureRocketSurgery<TScanner>([NotNull] this IWebAssemblyHostBuilder builder)
            where TScanner : IConventionScanner
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return ConfigureRocketSurgery<TScanner>(builder, _ => { });
        }

        /// <summary>
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder ConfigureRocketSurgery<TScanner>([NotNull] this WebAssemblyHostBuilder builder)
            where TScanner : IConventionScanner
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return ConfigureRocketSurgery<TScanner>(new WrappedWebAssemblyHostBuilder(builder), _ => { });
        }

        /// <summary>
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder ConfigureRocketSurgery<TScanner>(
            [NotNull] this IWebAssemblyHostBuilder builder,
            [NotNull] Action<IConventionHostBuilder> action
        )
            where TScanner : IConventionScanner
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
        /// Configures the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder ConfigureRocketSurgery<TScanner>(
            [NotNull] this WebAssemblyHostBuilder builder,
            [NotNull] Action<IConventionHostBuilder> action
        )
            where TScanner : IConventionScanner
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var innerBuilder = new WrappedWebAssemblyHostBuilder(builder);
            action(GetOrCreateBuilder(innerBuilder, typeof(TScanner)));
            return innerBuilder;
        }

        /// <summary>
        /// Uses the rocket booster.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="func">The function.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder UseRocketBooster(
            [NotNull] this IWebAssemblyHostBuilder builder,
            [NotNull] Func<IWebAssemblyHostBuilder, IConventionHostBuilder> func,
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
        /// Uses the rocket booster.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="func">The function.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder UseRocketBooster(
            [NotNull] this WebAssemblyHostBuilder builder,
            [NotNull] Func<IWebAssemblyHostBuilder, IConventionHostBuilder> func,
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

            var innerBuilder = new WrappedWebAssemblyHostBuilder(builder);
            var b = func(innerBuilder);
            action?.Invoke(b);
            return innerBuilder;
        }

        /// <summary>
        /// Launches the with.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="func">The function.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder LaunchWith(
            [NotNull] this IWebAssemblyHostBuilder builder,
            [NotNull] Func<IWebAssemblyHostBuilder, IConventionHostBuilder> func,
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
        /// <returns>IWebAssemblyHostBuilder.</returns>
        public static IWebAssemblyHostBuilder LaunchWith(
            [NotNull] this WebAssemblyHostBuilder builder,
            [NotNull] Func<IWebAssemblyHostBuilder, IConventionHostBuilder> func,
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

            var innerBuilder = new WrappedWebAssemblyHostBuilder(builder);
            var b = func(innerBuilder);
            action?.Invoke(b);
            return innerBuilder;
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
        /// <param name="logger">The assembly candidate finder.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseDiagnosticLogger(
            [NotNull] this IConventionHostBuilder builder,
            [NotNull] ILogger logger
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return Swap(builder, GetOrCreateBuilder(builder).With(logger));
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
        /// Uses the dependency context.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseDependencyContext(
            [NotNull] this IConventionHostBuilder builder,
            [NotNull] DependencyContext dependencyContext,
            ILogger? diagnosticLogger = null
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

            return RocketBooster.ForDependencyContext(dependencyContext, diagnosticLogger)(builder.Get<IWebAssemblyHostBuilder>()!);
        }

        /// <summary>
        /// Uses the application domain.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseAppDomain(
            [NotNull] this IConventionHostBuilder builder,
            [NotNull] AppDomain appDomain,
            ILogger? diagnosticLogger = null
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

            return RocketBooster.ForAppDomain(appDomain, diagnosticLogger)(builder.Get<IWebAssemblyHostBuilder>()!);
        }

        /// <summary>
        /// Uses the assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseAssemblies(
            [NotNull] this IConventionHostBuilder builder,
            [NotNull] IEnumerable<Assembly> assemblies,
            ILogger? diagnosticLogger = null
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

            return RocketBooster.ForAssemblies(assemblies, diagnosticLogger)(builder.Get<IWebAssemblyHostBuilder>()!);
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

            builder.UseDiagnosticLogger(
                new ServiceCollection()
                   .AddLogging(action)
                   .BuildServiceProvider()
                   .GetRequiredService<ILoggerFactory>()
                   .CreateLogger("DiagnosticLogger")
            );

            return builder;
        }

        /// <summary>
        /// Gets the or create builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scannerType">The default type of scanner to use.</param>
        /// <returns>RocketWebAssemblyBuilder.</returns>
        internal static RocketWebAssemblyBuilder GetOrCreateBuilder(IConventionHostBuilder builder, Type? scannerType = null)
            => GetOrCreateBuilder(builder.Get<IWebAssemblyHostBuilder>()!, scannerType);

        /// <summary>
        /// Gets the or create builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scannerType">The default type of scanner to use.</param>
        /// <returns>RocketWebAssemblyBuilder.</returns>
        internal static RocketWebAssemblyBuilder GetOrCreateBuilder(IWebAssemblyHostBuilder builder, Type? scannerType = null)
        {
            var conventionHostBuilder = builder.GetConventions();
            if (conventionHostBuilder is RocketWebAssemblyBuilder rocketWebAssemblyBuilder)
            {
                return rocketWebAssemblyBuilder;
            }

            if (conventionHostBuilder is UninitializedConventionHostBuilder uninitializedHostBuilder)
            {
                var appDomain = AppDomain.CurrentDomain;
                var logger = NullLogger.Instance;
                var serviceProviderDictionary = uninitializedHostBuilder.ServiceProperties;
                serviceProviderDictionary
                   .Set<ILogger>(logger)
                   .Set(builder)
                   .Set(HostType.Live);
                var assemblyCandidateFinder = new AppDomainAssemblyCandidateFinder(appDomain, logger);
                var assemblyProvider = new AppDomainAssemblyProvider(appDomain, logger);
                var scanner = ConvertTo(
                    uninitializedHostBuilder.Scanner,
                    assemblyCandidateFinder,
                    serviceProviderDictionary,
                    logger,
                    scannerType ?? typeof(SimpleConventionScanner)
                );

                builder.ConfigureContainer(new ServicesBuilderServiceProviderFactory(
                    builder,
                    (hostBuilder, collection) => new ServicesBuilder(
                        hostBuilder.Scanner,
                        hostBuilder.AssemblyProvider,
                        hostBuilder.AssemblyCandidateFinder,
                        collection,
                        builder.Configuration,
                        hostBuilder.Get<ILogger>()!,
                        hostBuilder.ServiceProperties
                    )
                ));

                rocketWebAssemblyBuilder = new RocketWebAssemblyBuilder(builder, scanner, assemblyCandidateFinder, assemblyProvider, logger, serviceProviderDictionary);
                serviceProviderDictionary[typeof(IConventionHostBuilder)] = rocketWebAssemblyBuilder;

                builder.Services.RemoveAll<IConventionHostBuilder>();
                builder.Services.Insert(0, ServiceDescriptor.Singleton<IConventionHostBuilder>(rocketWebAssemblyBuilder));

                return rocketWebAssemblyBuilder;
            }

            throw new NotSupportedException("Unsupported configuration");
        }


        private static IConventionScanner ConvertTo(
            UninitializedConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceProviderDictionary serviceProviderDictionary,
            ILogger logger,
            Type toType
        )
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
        /// <returns>RocketWebAssemblyBuilder.</returns>
        internal static IConventionHostBuilder Swap(
            IConventionHostBuilder builder,
            IConventionHostBuilder newRocketBuilder
        )
        {
            builder.Set(newRocketBuilder);
            return newRocketBuilder;
        }

        /// <summary>
        /// Gets the convention host builder or creates one of it's missing.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        internal static IConventionHostBuilder GetConventions(this IWebAssemblyHostBuilder hostBuilder)
        {
            var builder = hostBuilder.Services.Select(z => z.ImplementationInstance).OfType<IConventionHostBuilder>().FirstOrDefault();
            if (builder == null)
            {
                var properties = new Dictionary<object, object?>();
                var conventionHostBuilder = builder = new UninitializedConventionHostBuilder(properties);
                conventionHostBuilder.ServiceProperties.Set(hostBuilder.HostEnvironment);
                hostBuilder.Services.Insert(0, ServiceDescriptor.Singleton<IConventionHostBuilder>(conventionHostBuilder));
            }
            return builder!;
        }

        /// <summary>
        /// Gets the convention host builder or creates one of it's missing.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        internal static IConventionHostBuilder GetConventions(this WebAssemblyHostBuilder hostBuilder) => new WrappedWebAssemblyHostBuilder(hostBuilder).GetConventions();
    }
}