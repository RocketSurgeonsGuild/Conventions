using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions.TestHost
{
    /// <summary>
    /// A convention test host builder
    /// </summary>
    public class ConventionTestHostBuilder
    {
        private IServiceProviderDictionary _serviceProperties = new ServiceProviderDictionary();
        private IConventionScanner? _scanner;
        private IAssemblyCandidateFinder? _assemblyCandidateFinder;
        private IAssemblyProvider? _assemblyProvider;
        private ILoggerFactory? _loggerFactory;
        private ILogger? _logger;
        private DiagnosticSource? _diagnosticSource;
        private IRocketEnvironment? _environment;

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="type">The type that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For(Type type, ILoggerFactory? loggerFactory = null)
        {
            return For(DependencyContext.Load(type.Assembly), loggerFactory);
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="instance">The object that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For(object instance, ILoggerFactory? loggerFactory = null)
        {
            return For(DependencyContext.Load(instance.GetType().Assembly), loggerFactory);
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For(Assembly assembly, ILoggerFactory? loggerFactory = null)
        {
            return For(DependencyContext.Load(assembly), loggerFactory);
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="context">The context that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
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
        public ConventionTestHost Create()
        {
            var assemblyCandidateFinder = _assemblyCandidateFinder ?? throw new ArgumentNullException("AssemblyCandidateFinder");
            var assemblyProvider = _assemblyProvider ?? throw new ArgumentNullException("AssemblyProvider");
            return new ConventionTestHost(
                _scanner ?? new SimpleConventionScanner(assemblyCandidateFinder, _serviceProperties, _logger ?? NullLogger.Instance),
                assemblyCandidateFinder,
                assemblyProvider,
                _diagnosticSource ?? new DiagnosticListener(nameof(ConventionTestHost)),
                _serviceProperties,
                _loggerFactory ?? NullLoggerFactory.Instance,
                _environment ?? new RocketEnvironment("Test", nameof(ConventionTestHost), null, null)
            );
        }
    }
}
