using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Hosting;

#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// A convention test host builder
    /// </summary>
    public class TestHost
    {
        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="type">The type that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="hostBuilder">Optional host builder.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestHost For([NotNull] Type type, ILoggerFactory? loggerFactory = null, IHostBuilder? hostBuilder = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return For(
                DependencyContext.Load(type.Assembly),
                type.Assembly,
                loggerFactory,
                hostBuilder
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="instance">The object that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="hostBuilder">Optional host builder.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestHost For([NotNull] object instance, ILoggerFactory? loggerFactory = null, IHostBuilder? hostBuilder = null)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return For(
                DependencyContext.Load(instance.GetType().Assembly),
                instance.GetType().Assembly,
                loggerFactory,
                hostBuilder
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="hostBuilder">Optional host builder.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestHost For([NotNull] Assembly assembly, ILoggerFactory? loggerFactory = null, IHostBuilder? hostBuilder = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return For(
                DependencyContext.Load(assembly),
                assembly,
                loggerFactory,
                hostBuilder
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="context">The context that that will be used for the test host.</param>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="hostBuilder">Optional host builder.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestHost For(DependencyContext context, Assembly assembly, ILoggerFactory? loggerFactory = null, IHostBuilder? hostBuilder = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            hostBuilder ??= Host.CreateDefaultBuilder();

            var logger = loggerFactory.CreateLogger(nameof(TestHost));

            return new TestHost()
                   .WithDependencyContext(context)
                   .With(loggerFactory)
                   .WithLogger(logger)
                   .With(assembly)
                   .With(hostBuilder)
                ;
        }

        private IServiceProviderDictionary _serviceProperties = new ServiceProviderDictionary();
        private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
        private ILogger _logger = NullLogger.Instance;
        private Assembly? _assembly;
        private IHostBuilder? _hostBuilder;
        private ConfigOptions _configOptions = new ConfigOptions();
        private DependencyContext? _dependencyContext;
        private string? _environmentName;
        private bool _includeConventions;

        /// <summary>
        /// Use the specific <see cref="IConventionScanner" />
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public TestHost With(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IConventionScanner" />
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public TestHost WithHostBuilder(IHostBuilder hostBuilder) => With(hostBuilder);

        /// <summary>
        /// Use the specific <see cref="DependencyContext" />
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        public TestHost With(DependencyContext dependencyContext)
        {
            _dependencyContext = dependencyContext;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyCandidateFinder" />
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        public TestHost WithDependencyContext(DependencyContext dependencyContext) => With(dependencyContext);

        /// <summary>
        /// Use the specific <see cref="Assembly" />
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public TestHost With(Assembly assembly)
        {
            _assembly = assembly;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ConfigOptions" />
        /// </summary>
        /// <param name="options">The assembly.</param>
        public TestHost WithConfigOptions(ConfigOptions options) => With(options);

        /// <summary>
        /// Use the specific <see cref="ConfigOptions" />
        /// </summary>
        /// <param name="options">The assembly.</param>
        public TestHost With(ConfigOptions options)
        {
            _configOptions = options;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="Assembly" />
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public TestHost WithAssembly(Assembly assembly) => With(assembly);

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TestHost With(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TestHost WithLogger(ILogger logger) => With(logger);

        /// <summary>
        /// Use the specific <see cref="ILoggerFactory" />
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public TestHost With(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILoggerFactory" />
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public TestHost WithLoggerFactory(ILoggerFactory loggerFactory) => With(loggerFactory);

        /// <summary>
        /// Use the specific environment name
        /// </summary>
        /// <param name="environmentName">The environment name.</param>
        public TestHost WithEnvironmentName(string environmentName)
        {
            _environmentName = environmentName;
            return this;
        }

        /// <summary>
        /// Use the <see cref="BasicConventionScanner" /> to not automatically load conventions from attributes.
        /// </summary>
        /// <returns></returns>
        public TestHost ExcludeConventionAttributes()
        {
            _includeConventions = false;
            return this;
        }

        /// <summary>
        /// <para>Use the <see cref="SimpleConventionScanner" /> to automatically load conventions from attributes.</para>
        /// <para>This is the default</para>
        /// </summary>
        public TestHost IncludeConventionAttributes()
        {
            _includeConventions = true;
            return this;
        }

        /// <summary>
        /// Create the convention test host with the given defaults
        /// </summary>
        /// <returns></returns>
        public TestHostBuilder Create() => Create(_ => { });

        /// <summary>
        /// Create the convention test host with the given defaults
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public TestHostBuilder Create([NotNull] Action<TestHostBuilder> action)
        {
            var assembly = _assembly ?? throw new ArgumentNullException("Assembly");
            var dependencyContext = _dependencyContext ?? throw new ArgumentNullException("DependencyContext");
            var hostBuilder = _hostBuilder ?? Host.CreateDefaultBuilder();
            var environmentName = string.IsNullOrWhiteSpace(_environmentName) ? "Test" : _environmentName;

            var contentRootPath = assembly != null && Directory.Exists(Path.GetDirectoryName(assembly.Location))
                ? Path.GetDirectoryName(assembly.Location)
                : string.Empty;

            return Construct(
                dependencyContext,
                hostBuilder,
                environmentName,
                contentRootPath,
                _includeConventions,
                builder =>
                {
                    builder.Set(_configOptions);
                    action(builder);
                    builder.Set(HostType.UnitTestHost);
                    builder.Set(_logger);
                    builder.Set(_loggerFactory);

                    builder.Get<IHostBuilder>()
                       .ConfigureServices(services => builder.Set(services))
                       .ConfigureLogging(
                        x =>
                        {
                            if (_loggerFactory != null)
                            {
                                x.Services.RemoveAll(typeof(ILoggerFactory));
                                x.Services.AddSingleton(_loggerFactory);
                            }

                            if (_logger != null)
                            {
                                x.Services.RemoveAll(typeof(ILogger));
                                x.Services.AddSingleton(_logger);
                            }
                        }
                    );
                }
            );
        }

        private static TestHostBuilder Construct(
            DependencyContext dependencyContext,
            IHostBuilder hostBuilder,
            string environmentName,
            string contentRootPath,
            bool includeConventions,
            [NotNull] Action<TestHostBuilder> action
        )
        {
            var outerAction = action;
            action = x =>
            {
                x.UseDependencyContext(dependencyContext);
                outerAction(x);
            };
            var builder = new TestHostBuilder(hostBuilder);
            (includeConventions
                    ? hostBuilder.ConfigureRocketSurgery<SimpleConventionScanner>(x => action(builder))
                    : hostBuilder.ConfigureRocketSurgery<BasicConventionScanner>(x => action(builder)))
               .UseEnvironment(environmentName)
               .UseContentRoot(contentRootPath);

            return builder;
        }
    }
}