using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        /// <param name="loggerFactory">Optional logger factory.</param>
        /// <param name="contentRootPath">The content root path for the host environment.</param>
        public static TestHost For([NotNull] Type type, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return For(
                DependencyContext.Load(type.Assembly),
                loggerFactory,
                contentRootPath
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="instance">The object that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        /// <param name="contentRootPath">The content root path for the host environment.</param>
        public static TestHost For([NotNull] object instance, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return For(
                DependencyContext.Load(instance.GetType().Assembly),
                loggerFactory,
                contentRootPath
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        /// <param name="contentRootPath">The content root path for the host environment.</param>
        public static TestHost For([NotNull] Assembly assembly, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return For(
                DependencyContext.Load(assembly),
                loggerFactory,
                contentRootPath
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="context">The context that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        /// <param name="contentRootPath">The content root path for the host environment.</param>
        public static TestHost For(DependencyContext context, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;

            var logger = loggerFactory.CreateLogger(nameof(TestHost));

            return new TestHost()
                   .WithDependencyContext(context)
                   .WithLoggerFactory(loggerFactory)
                   .WithLogger(logger)
                   .WithContentRoot(contentRootPath)
                ;
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="appDomain">The application domain that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        /// <param name="contentRootPath">The content root path for the host environment.</param>
        public static TestHost For(AppDomain appDomain, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            var logger = loggerFactory.CreateLogger(nameof(TestHost));

            return new TestHost()
                   .WithAppDomain(appDomain)
                   .WithLoggerFactory(loggerFactory)
                   .WithLogger(logger)
                   .WithContentRoot(contentRootPath)
                ;
        }

        private IServiceProviderDictionary _serviceProperties = new ServiceProviderDictionary();
        private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
        private ILogger _logger = NullLogger.Instance;
        private IConfiguration? _reuseConfiguration;
        private object? _sharedConfigurationKey;
        private DependencyContext? _dependencyContext;
        private AppDomain? _appDomain;
        private string? _environmentName;
        private string[]? _arguments;
        private bool _includeConventions = true;
        private IEnumerable<Assembly>? _assemblies;
        private string? _contentRootPath;

        public TestHost WithContentRoot(string? contentRootPath)
        {
            if (string.IsNullOrWhiteSpace(contentRootPath))
                return this;
            _contentRootPath = contentRootPath;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="AppDomain" />
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        public TestHost WithAppDomain(AppDomain appDomain)
        {
            _appDomain = appDomain;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyCandidateFinder" />
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        public TestHost WithDependencyContext(DependencyContext dependencyContext)
        {
            _dependencyContext = dependencyContext;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyCandidateFinder" />
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public TestHost WithAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TestHost WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Supply command line arguments for the test host
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public TestHost WithArguments(string[] arguments)
        {
            _arguments = arguments;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILoggerFactory" />
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public TestHost WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

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
        public TestHost ExcludeConventions()
        {
            _includeConventions = false;
            return this;
        }

        /// <summary>
        /// <para>Use the <see cref="SimpleConventionScanner" /> to automatically load conventions from attributes.</para>
        /// <para>This is the default</para>
        /// </summary>
        public TestHost IncludeConventions()
        {
            _includeConventions = true;
            return this;
        }

        /// <summary>
        /// Use a specific configuration object with the test host
        /// (This can help avoid re-reading the same configuration over and over)
        /// </summary>
        /// <param name="sharedConfiguration">The shared configuration.</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public TestHost WithConfiguration(IConfiguration sharedConfiguration)
        {
            _reuseConfiguration = sharedConfiguration;
            return this;
        }

        /// <summary>
        /// Use a specific configuration object with the test host
        /// (This can help avoid re-reading the same configuration over and over)
        /// </summary>
        /// <param name="key">The object to use as a key for shared configuration</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public TestHost ShareConfiguration(object key)
        {
            _sharedConfigurationKey = key;
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
            if (_dependencyContext == null && _appDomain == null)
                throw new NotSupportedException("Must supply appdomain, dependency context, or list of assemblies");
            var environmentName = string.IsNullOrWhiteSpace(_environmentName) ? "Test" : _environmentName;


            var outerAction = action;
            action = builder =>
            {
                if (_dependencyContext != null)
                {
                    builder.UseDependencyContext(_dependencyContext);
                }
                else if (_appDomain != null)
                {
                    builder.UseAppDomain(_appDomain);
                }
                else if (_assemblies != null)
                {
                    builder.UseAssemblies(_assemblies);
                }

                builder.Properties[typeof(HostType)] = HostType.UnitTestHost;
                builder.Set(_logger);
                builder.Set(_loggerFactory);

                builder.Get<IHostBuilder>()
                   .ConfigureLogging(
                        x =>
                        {
                            if (_loggerFactory != NullLoggerFactory.Instance)
                            {
                                x.Services.RemoveAll(typeof(ILoggerFactory));
                                x.Services.AddSingleton(_loggerFactory);
                            }
                        }
                    );

                outerAction(builder);
            };

            var builder = new TestHostBuilder(_serviceProperties.ToDictionary(x => x.Key, x => x.Value));
            builder
               .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        IHostEnvironment env = hostingContext.HostingEnvironment;

                        bool reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);

                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);

                        if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                        {
                            var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                            if (appAssembly != null)
                            {
                                config.AddUserSecrets(appAssembly, optional: true);
                            }
                        }

                        config.AddEnvironmentVariables();

                        if (_arguments != null)
                        {
                            config.AddCommandLine(_arguments);
                        }
                    }
                );

            if (_reuseConfiguration != null)
            {
                builder.Set(typeof(IConfiguration).FullName, _reuseConfiguration);
            }
            else if (_sharedConfigurationKey != null)
            {
                builder.Set(typeof(IConfiguration).FullName, _sharedConfigurationKey);
            }

            ( _includeConventions
                    ? builder.ConfigureRocketSurgery<SimpleConventionScanner>(x => action(builder))
                    : builder.ConfigureRocketSurgery<BasicConventionScanner>(x => action(builder)) )
               .UseEnvironment(environmentName);
            if (_contentRootPath != null)
            {
                builder.UseContentRoot(_contentRootPath);
            }

            return builder;
        }
    }
}