using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Hosting;

#pragma warning disable CA2000

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// A convention test host builder
    /// </summary>
    public class TestWebAssemblyHost
    {
        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="type">The type that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestWebAssemblyHost For([NotNull] Type type, ILoggerFactory? loggerFactory = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return For(
                DependencyContext.Load(type.Assembly),
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="instance">The object that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestWebAssemblyHost For([NotNull] object instance, ILoggerFactory? loggerFactory = null)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return For(
                DependencyContext.Load(instance.GetType().Assembly),
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestWebAssemblyHost For([NotNull] Assembly assembly, ILoggerFactory? loggerFactory = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return For(
                DependencyContext.Load(assembly),
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="context">The context that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestWebAssemblyHost For(DependencyContext context, ILoggerFactory? loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;

            var logger = loggerFactory.CreateLogger(nameof(TestWebAssemblyHost));

            return new TestWebAssemblyHost()
                   .WithDependencyContext(context)
                   .WithLoggerFactory(loggerFactory)
                   .WithLogger(logger)
                ;
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="appDomain">The application domain that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestWebAssemblyHost For(AppDomain appDomain, ILoggerFactory? loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            var logger = loggerFactory.CreateLogger(nameof(TestWebAssemblyHost));

            return new TestWebAssemblyHost()
                   .WithAppDomain(appDomain)
                   .WithLoggerFactory(loggerFactory)
                   .WithLogger(logger)
                ;
        }

        private static readonly ConditionalWeakTable<object, IConfiguration> _sharedConfigurations = new ConditionalWeakTable<object, IConfiguration>();
        private readonly IServiceProviderDictionary _serviceProperties = new ServiceProviderDictionary();
        private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
        private ILogger _logger = NullLogger.Instance;
        private IConfiguration? _reuseConfiguration;
        private object? _sharedConfigurationKey;
        private DependencyContext? _dependencyContext;
        private AppDomain? _appDomain;
        private string? _environmentName;
        private bool _includeConventions = true;
        private IEnumerable<Assembly>? _assemblies;
        private IJSRuntime? _jsRuntime;
        private NavigationManager? _navigationManager;
        private INavigationInterception? _navigationInterception;

        /// <summary>
        /// Use the specific <see cref="AppDomain" />
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        public TestWebAssemblyHost WithAppDomain(AppDomain appDomain)
        {
            _appDomain = appDomain;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyCandidateFinder" />
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        public TestWebAssemblyHost WithDependencyContext(DependencyContext dependencyContext)
        {
            _dependencyContext = dependencyContext;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyCandidateFinder" />
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public TestWebAssemblyHost WithAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TestWebAssemblyHost WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILoggerFactory" />
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public TestWebAssemblyHost WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Use the specific environment name
        /// </summary>
        /// <param name="environmentName">The environment name.</param>
        public TestWebAssemblyHost WithEnvironmentName(string environmentName)
        {
            _environmentName = environmentName;
            return this;
        }

        /// <summary>
        /// Use the specific js runtime
        /// </summary>
        /// <param name="jsRuntime">The environment name.</param>
        public TestWebAssemblyHost WithJSRuntime(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            return this;
        }

        /// <summary>
        /// Use the specific navigation manager
        /// </summary>
        /// <param name="navigationManager">The environment name.</param>
        public TestWebAssemblyHost WithNavigationManager(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            return this;
        }

        /// <summary>
        /// Use the specific navigation manager
        /// </summary>
        /// <param name="navigationInterception">The environment name.</param>
        public TestWebAssemblyHost WithNavigationInterception(INavigationInterception navigationInterception)
        {
            _navigationInterception = navigationInterception;
            return this;
        }

        /// <summary>
        /// Use the <see cref="BasicConventionScanner" /> to not automatically load conventions from attributes.
        /// </summary>
        /// <returns></returns>
        public TestWebAssemblyHost ExcludeConventions()
        {
            _includeConventions = false;
            return this;
        }

        /// <summary>
        /// <para>Use the <see cref="SimpleConventionScanner" /> to automatically load conventions from attributes.</para>
        /// <para>This is the default</para>
        /// </summary>
        public TestWebAssemblyHost IncludeConventions()
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
        public TestWebAssemblyHost WithConfiguration(IConfiguration sharedConfiguration)
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
        public TestWebAssemblyHost ShareConfiguration(object key)
        {
            _sharedConfigurationKey = key;
            return this;
        }

        /// <summary>
        /// Create the convention test host with the given defaults
        /// </summary>
        /// <returns></returns>
        public TestWebAssemblyHostBuilder Create() => Create(_ => { });

        /// <summary>
        /// Create the convention test host with the given defaults
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public TestWebAssemblyHostBuilder Create([NotNull] Action<TestWebAssemblyHostBuilder> action)
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

                builder.ServiceProperties[typeof(HostType)] = HostType.UnitTestHost;
                builder.Set(_logger);
                builder.Set(_loggerFactory);
                if (_navigationInterception != null)
                    builder.Set(_navigationInterception);
                if (_navigationManager != null)
                    builder.Set(_navigationManager);
                if (_jsRuntime != null)
                    builder.Set(_jsRuntime);

                if (_loggerFactory != NullLoggerFactory.Instance)
                {
                    builder.Get<IWebAssemblyHostBuilder>()!.Logging.Services.RemoveAll(typeof(ILoggerFactory));
                    builder.Get<IWebAssemblyHostBuilder>()!.Logging.Services.AddSingleton(_loggerFactory);
                }

                outerAction(builder);
            };

            var builder = new TestWebAssemblyHostBuilder(_serviceProperties, environmentName);
            builder.Set<IWebAssemblyHostBuilder>(builder);
            IWebAssemblyHostEnvironment env = builder.HostEnvironment;

            var reloadOnChange = builder.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);

            if (_reuseConfiguration != null)
            {
                builder.Configuration.AddConfiguration(_reuseConfiguration, false);
            }
            else if (_sharedConfigurationKey != null && _sharedConfigurations.TryGetValue(_sharedConfigurationKey, out var sharedConfiguration))
            {
                builder.Configuration.AddConfiguration(sharedConfiguration, false);
            }
            else
            {
                var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
                   .AddJsonFile($"appsettings.{env.Environment}.json", optional: true, reloadOnChange: reloadOnChange)
                   .Build();
                if (_sharedConfigurationKey != null)
                {
                    _sharedConfigurations.Add(_sharedConfigurationKey, config);
                    builder.Configuration.AddConfiguration(config, false);
                }
                else
                {
                    builder.Configuration.AddConfiguration(config, true);
                }
            }

            if (_includeConventions)
            {
                builder.ConfigureRocketSurgery<SimpleConventionScanner>(x => action(builder));
            }
            else
            {
                builder.ConfigureRocketSurgery<BasicConventionScanner>(x => action(builder));
            }

            return builder;
        }
    }
}