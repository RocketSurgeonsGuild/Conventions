using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions;

#pragma warning disable CA2000

namespace Rocket.Surgery.Hosting;

/// <summary>
///     A convention test host builder
/// </summary>
public class TestHost
{
    /// <summary>
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="type">The type that that will be used to load the <see cref="DependencyContext" />.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static TestHost For(Type type, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
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
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="instance">The object that that will be used to load the <see cref="DependencyContext" />.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static TestHost For(object instance, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
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
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static TestHost For(Assembly assembly, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
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
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="context">The context that that will be used for the test host.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static TestHost For(DependencyContext context, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
    {
        loggerFactory ??= NullLoggerFactory.Instance;

        var logger = loggerFactory.CreateLogger(nameof(TestHost));

        return new TestHost()
              .UseDependencyContext(context)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
              .WithContentRoot(contentRootPath)
            ;
    }

    /// <summary>
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="appDomain">The application domain that that will be used for the test host.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static TestHost For(AppDomain appDomain, ILoggerFactory? loggerFactory = null, string? contentRootPath = null)
    {
        loggerFactory ??= NullLoggerFactory.Instance;
        var logger = loggerFactory.CreateLogger(nameof(TestHost));

        return new TestHost()
              .UseAppDomain(appDomain)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
              .WithContentRoot(contentRootPath)
            ;
    }

    /// <summary>
    ///     Create a convention test host build for the given the list of assemblies.
    /// </summary>
    /// <param name="assemblies">The application domain that that will be used for the test host.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    public static TestHost For(IEnumerable<Assembly> assemblies, ILoggerFactory? loggerFactory = null)
    {
        loggerFactory ??= NullLoggerFactory.Instance;
        var logger = loggerFactory.CreateLogger(nameof(TestHost));

        return new TestHost()
              .UseAssemblies(assemblies)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
            ;
    }

    private readonly ConventionContextBuilder _contextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>()).Set(HostType.UnitTest);
    private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
    private ILogger _logger = NullLogger.Instance;
    private IConfiguration? _reuseConfiguration;
    private object? _sharedConfigurationKey;
    private string? _environmentName;
    private string[]? _arguments;
    private string? _contentRootPath;

    /// <summary>
    ///     Use the given content root path
    /// </summary>
    /// <param name="contentRootPath"></param>
    /// <returns></returns>
    public TestHost WithContentRoot(string? contentRootPath)
    {
        if (string.IsNullOrWhiteSpace(contentRootPath))
            return this;
        _contentRootPath = contentRootPath;
        return this;
    }

    /// <summary>
    ///     Use the specific <see cref="AppDomain" />
    /// </summary>
    /// <param name="appDomain">The app domain.</param>
    public TestHost UseAppDomain(AppDomain appDomain)
    {
        _contextBuilder.UseAppDomain(appDomain);
        return this;
    }

    /// <summary>
    ///     Use the specific <see cref="DependencyContext" />
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    public TestHost UseDependencyContext(DependencyContext dependencyContext)
    {
        _contextBuilder.UseDependencyContext(dependencyContext);
        return this;
    }

    /// <summary>
    ///     Use the specific <see cref="IEnumerable&lt;Assembly&gt;" />
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    public TestHost UseAssemblies(IEnumerable<Assembly> assemblies)
    {
        _contextBuilder.UseAssemblies(assemblies);
        return this;
    }

    /// <summary>
    ///     Use the specific <see cref="ILogger" />
    /// </summary>
    /// <param name="logger">The logger.</param>
    public TestHost WithLogger(ILogger logger)
    {
        _logger = logger;
        return this;
    }

    /// <summary>
    ///     Supply command line arguments for the test host
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public TestHost WithArguments(string[] arguments)
    {
        _arguments = arguments;
        return this;
    }

    /// <summary>
    ///     Use the specific <see cref="ILoggerFactory" />
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    public TestHost WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        return this;
    }

    /// <summary>
    ///     Use the specific environment name
    /// </summary>
    /// <param name="environmentName">The environment name.</param>
    public TestHost WithEnvironmentName(string environmentName)
    {
        _environmentName = environmentName;
        return this;
    }

    /// <summary>
    ///     Use to not automatically load conventions from attributes.
    /// </summary>
    /// <returns></returns>
    public TestHost ExcludeConventions()
    {
        _contextBuilder.DisableConventionAttributes();
        return this;
    }

    /// <summary>
    ///     <para>Use to automatically load conventions from attributes.</para>
    ///     <para>This is the default</para>
    /// </summary>
    public TestHost IncludeConventions()
    {
        _contextBuilder.EnableConventionAttributes();
        return this;
    }

    /// <summary>
    ///     Use a specific configuration object with the test host
    ///     (This can help avoid re-reading the same configuration over and over)
    /// </summary>
    /// <param name="sharedConfiguration">The shared configuration.</param>
    /// <returns>ConventionTestHostBuilder.</returns>
    public TestHost WithConfiguration(IConfiguration sharedConfiguration)
    {
        _reuseConfiguration = sharedConfiguration;
        return this;
    }

    /// <summary>
    ///     Use a specific configuration object with the test host
    ///     (This can help avoid re-reading the same configuration over and over)
    /// </summary>
    /// <param name="key">The object to use as a key for shared configuration</param>
    /// <returns>ConventionTestHostBuilder.</returns>
    public TestHost ShareConfiguration(object key)
    {
        _sharedConfigurationKey = key;
        return this;
    }

    /// <summary>
    ///     Create the convention test host with the given defaults
    /// </summary>
    /// <returns></returns>
    public TestHostBuilder Create()
    {
        return Create(_ => { });
    }

    /// <summary>
    ///     Create the convention test host with the given defaults
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public TestHostBuilder Create(Action<ConventionContextBuilder> action)
    {
        var environmentName = string.IsNullOrWhiteSpace(_environmentName) ? "Test" : _environmentName;

        var outerAction = action;
        action = contextBuilder =>
        {
            contextBuilder.Set(_logger);
            contextBuilder.Set(_loggerFactory);

            outerAction(contextBuilder);
        };

        // copy properties
        action(_contextBuilder);
        var context = ConventionContext.From(_contextBuilder);
        var builder = new TestHostBuilder(context);
        RocketHostExtensions.ConfigureRocketSurgery(builder, context);
        if (_loggerFactory != NullLoggerFactory.Instance)
        {
            builder.ConfigureServices(services => services.RemoveAll(typeof(ILoggerFactory)).AddSingleton(_loggerFactory));
        }
        // builder
        //    .ConfigureAppConfiguration(
        //         (hostingContext, config) =>
        //         {
        //             IHostEnvironment env = hostingContext.HostingEnvironment;
        //
        //             bool reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
        //
        //             config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
        //                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);
        //
        //             if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
        //             {
        //                 var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        //                 if (appAssembly != null)
        //                 {
        //                     config.AddUserSecrets(appAssembly, optional: true);
        //                 }
        //             }
        //
        //             config.AddEnvironmentVariables();
        //
        //             if (_arguments != null)
        //             {
        //                 config.AddCommandLine(_arguments);
        //             }
        //         }
        //     );

        if (_reuseConfiguration != null)
        {
            builder.Set(typeof(IConfiguration).FullName!, _reuseConfiguration);
        }
        else if (_sharedConfigurationKey != null)
        {
            builder.Set(typeof(IConfiguration).FullName!, _sharedConfigurationKey);
        }

        builder.UseEnvironment(environmentName);
        if (_contentRootPath != null)
        {
            builder.UseContentRoot(_contentRootPath);
        }

        return builder;
    }
}
