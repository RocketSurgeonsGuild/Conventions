using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Rocket.Surgery.Conventions;

#pragma warning disable CA2000

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// A convention test host builder
    /// </summary>
    public class TestWebAssemblyHost
    {

        /// <summary>
        /// Create a convention test host build for the given the list of assemblies.
        /// </summary>
        /// <param name="assemblies">The application domain that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestWebAssemblyHost For(IEnumerable<Assembly> assemblies, ILoggerFactory? loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            var logger = loggerFactory.CreateLogger(nameof(TestWebAssemblyHost));

            return new TestWebAssemblyHost()
                   .UseAssemblies(assemblies)
                   .WithLoggerFactory(loggerFactory)
                   .WithLogger(logger)
                ;
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="AppDomain" /> in the assembly.
        /// </summary>
        /// <param name="appDomain">The application domain that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static TestWebAssemblyHost For(AppDomain appDomain, ILoggerFactory? loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            var logger = loggerFactory.CreateLogger(nameof(TestWebAssemblyHost));

            return new TestWebAssemblyHost()
                   .UseAppDomain(appDomain)
                   .WithLoggerFactory(loggerFactory)
                   .WithLogger(logger)
                ;
        }

        private static readonly ConditionalWeakTable<object, IConfiguration> _sharedConfigurations = new ConditionalWeakTable<object, IConfiguration>();
        private readonly ConventionContextBuilder _contextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>()).Set(HostType.UnitTest);
        private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
        private ILogger _logger = NullLogger.Instance;
        private IConfiguration? _reuseConfiguration;
        private object? _sharedConfigurationKey;
        private string? _environmentName;
        private IJSRuntime? _jsRuntime;
        private NavigationManager? _navigationManager;
        private INavigationInterception? _navigationInterception;

        /// <summary>
        /// Use the specific <see cref="AppDomain" />
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        public TestWebAssemblyHost UseAppDomain(AppDomain appDomain)
        {
            _contextBuilder.UseAppDomain(appDomain);
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IEnumerable&lt;Assembly&gt;" />
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public TestWebAssemblyHost UseAssemblies(IEnumerable<Assembly> assemblies)
        {
            _contextBuilder.UseAssemblies(assemblies);
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
        /// Use to not automatically load conventions from attributes.
        /// </summary>
        /// <returns></returns>
        public TestWebAssemblyHost ExcludeConventions()
        {
            _contextBuilder.DisableConventionAttributes();
            return this;
        }

        /// <summary>
        /// <para>Use to automatically load conventions from attributes.</para>
        /// <para>This is the default</para>
        /// </summary>
        public TestWebAssemblyHost IncludeConventions()
        {
            _contextBuilder.EnableConventionAttributes();
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
        public TestWebAssemblyHostBuilder Create([NotNull] Action<ConventionContextBuilder> action)
        {
            var environmentName = string.IsNullOrWhiteSpace(_environmentName) ? "Test" : _environmentName;

            var outerAction = action;
            action = builder =>
            {
                builder.Set(_logger);
                builder.Set(_loggerFactory);
                if (_navigationInterception != null)
                    builder.Set(_navigationInterception);
                if (_navigationManager != null)
                    builder.Set(_navigationManager);
                if (_jsRuntime != null)
                    builder.Set(_jsRuntime);

                outerAction(builder);
            };

            action(_contextBuilder);
            var context = ConventionContext.From(_contextBuilder);
            var builder = new TestWebAssemblyHostBuilder(context, environmentName);

            if (_loggerFactory != NullLoggerFactory.Instance)
            {
                builder.Logging.Services.RemoveAll(typeof(ILoggerFactory)).AddSingleton(_loggerFactory);
            }

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

            return builder;
        }
    }
}