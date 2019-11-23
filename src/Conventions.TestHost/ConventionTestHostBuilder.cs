using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;
using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions.TestHost
{
    /// <summary>
    /// A convention test host builder
    /// </summary>
    public class ConventionTestHostBuilder
    {
        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="type">The type that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For([NotNull] Type type, ILoggerFactory? loggerFactory = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return For(
                DependencyContext.Load(type.Assembly),
                type.Assembly,
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="instance">The object that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For([NotNull] object instance, ILoggerFactory? loggerFactory = null)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return For(
                DependencyContext.Load(instance.GetType().Assembly),
                instance.GetType().Assembly,
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For([NotNull] Assembly assembly, ILoggerFactory? loggerFactory = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return For(
                DependencyContext.Load(assembly),
                assembly,
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="context">The context that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        [Obsolete(
            "No longer to be used, has been replaced with the overload that takes the assembly and loads the context from that"
        )]
        public static ConventionTestHostBuilder For(DependencyContext context, ILoggerFactory? loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            var logger = loggerFactory.CreateLogger(nameof(ConventionTestHostBuilder));
            return new ConventionTestHostBuilder()
                   .With(loggerFactory)
                   .With(new DependencyContextAssemblyProvider(context, logger))
                   .With(new DependencyContextAssemblyCandidateFinder(context, logger))
                ;
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="context">The context that that will be used for the test host.</param>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For(
            DependencyContext context,
            Assembly assembly,
            ILoggerFactory? loggerFactory = null
        )
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            var logger = loggerFactory.CreateLogger(nameof(ConventionTestHostBuilder));
            return new ConventionTestHostBuilder()
                   .With(loggerFactory)
                   .With(assembly)
                   .With(new DependencyContextAssemblyProvider(context, logger))
                   .With(new DependencyContextAssemblyCandidateFinder(context, logger))
                ;
        }

        private IServiceProviderDictionary _serviceProperties = new ServiceProviderDictionary();
        private IConventionScanner? _scanner;
        private IAssemblyCandidateFinder? _assemblyCandidateFinder;
        private IAssemblyProvider? _assemblyProvider;
        private ILoggerFactory? _loggerFactory;
        private ILogger? _logger;
        private DiagnosticSource? _diagnosticSource;
        private IRocketEnvironment? _environment;
        private Assembly? _assembly;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConventionTestHostBuilder() => _serviceProperties.Set(
            new ConfigurationOptions
            {
                ApplicationConfiguration =
                {
                    builder => builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true),
                    builder => builder.AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true),
                    builder => builder.AddYamlFile("appsettings.yaml", optional: true, reloadOnChange: true),
                    builder => builder.AddIniFile("appsettings.ini", optional: true, reloadOnChange: true)
                },
                EnvironmentConfiguration =
                {
                    (builder, environmentName) => builder.AddJsonFile(
                        $"appsettings.{environmentName}.json",
                        optional: true,
                        reloadOnChange: true
                    ),
                    (builder, environmentName) => builder.AddYamlFile(
                        $"appsettings.{environmentName}.yml",
                        optional: true,
                        reloadOnChange: true
                    ),
                    (builder, environmentName) => builder.AddYamlFile(
                        $"appsettings.{environmentName}.yaml",
                        optional: true,
                        reloadOnChange: true
                    ),
                    (builder, environmentName) => builder.AddIniFile(
                        $"appsettings.{environmentName}.ini",
                        optional: true,
                        reloadOnChange: true
                    )
                }
            }
        );

        /// <summary>
        /// Use the specific <see cref="IConventionScanner" />
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public ConventionTestHostBuilder With(IConventionScanner scanner)
        {
            _scanner = scanner;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyCandidateFinder" />
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        public ConventionTestHostBuilder With(IAssemblyCandidateFinder assemblyCandidateFinder)
        {
            _assemblyCandidateFinder = assemblyCandidateFinder;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="Assembly" />
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public ConventionTestHostBuilder With(Assembly assembly)
        {
            _assembly = assembly;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyProvider" />
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        public ConventionTestHostBuilder With(IAssemblyProvider assemblyProvider)
        {
            _assemblyProvider = assemblyProvider;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IServiceProviderDictionary" />
        /// </summary>
        /// <param name="serviceProperties">The service provider dictionary.</param>
        public ConventionTestHostBuilder With(IServiceProviderDictionary serviceProperties)
        {
            _serviceProperties = serviceProperties;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ConventionTestHostBuilder With(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILoggerFactory" />
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public ConventionTestHostBuilder With(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        public ConventionTestHostBuilder With(DiagnosticSource diagnosticSource)
        {
            _diagnosticSource = diagnosticSource;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IRocketEnvironment" />
        /// </summary>
        /// <param name="environment">The environment.</param>
        public ConventionTestHostBuilder With(IRocketEnvironment environment)
        {
            _environment = environment;
            return this;
        }

        /// <summary>
        /// Create the convention test host with the given defaults
        /// </summary>
        /// <returns></returns>
        public ConventionTestHost Create() => Create(_ => { });

        /// <summary>
        /// Create the convention test host with the given defaults
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public ConventionTestHost Create([NotNull] Action<IConventionHostBuilder> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#pragma warning disable CA2208
            // ReSharper disable NotResolvedInText
            var assemblyCandidateFinder =
                _assemblyCandidateFinder ?? throw new ArgumentNullException("AssemblyCandidateFinder");
            var assemblyProvider = _assemblyProvider ?? throw new ArgumentNullException("AssemblyProvider");
            // ReSharper restore NotResolvedInText
#pragma warning restore CA2208

            var contentRootPath = _assembly != null && Directory.Exists(Path.GetDirectoryName(_assembly.Location))
                ? Path.GetDirectoryName(_assembly.Location)
                : string.Empty;
            var contentProvider = !string.IsNullOrWhiteSpace(contentRootPath)
                ? new PhysicalFileProvider(contentRootPath)
                : new NullFileProvider() as IFileProvider;
            var builder = new ConventionTestHost(
                _scanner ?? new SimpleConventionScanner(
                    assemblyCandidateFinder,
                    _serviceProperties,
                    _logger ?? NullLogger.Instance
                ),
                assemblyCandidateFinder,
                assemblyProvider,
                _diagnosticSource ?? new DiagnosticListener(nameof(ConventionTestHost)),
                _serviceProperties,
                _loggerFactory ?? NullLoggerFactory.Instance,
                _environment ?? new RocketEnvironment(
                    "Test",
                    nameof(ConventionTestHost),
                    contentRootPath,
                    contentProvider
                )
            );
            action(builder);
            return builder;
        }
    }
}