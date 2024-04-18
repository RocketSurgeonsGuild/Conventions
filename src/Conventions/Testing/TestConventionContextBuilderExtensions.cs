using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions.Testing;

/// <summary>
///     A convention test host builder
/// </summary>
public static class TestConventionContextBuilderExtensions
{
    /// <summary>
    ///     Create a convention test host build for the given <see cref="AppDomain" /> in the assembly.
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="appDomain">The application domain that that will be used for the test host.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static ConventionContextBuilder ForTesting(
        this ConventionContextBuilder builder,
        AppDomain appDomain,
        ILoggerFactory? loggerFactory = null,
        string? contentRootPath = null
    )
    {
        EnsureConfigured(builder);
        loggerFactory ??= NullLoggerFactory.Instance;
        var logger = loggerFactory.CreateLogger("TestContext");

        return builder
              .Set(HostType.UnitTest)
              .UseAppDomain(appDomain)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
              .WithContentRoot(contentRootPath);
    }

    /// <summary>
    ///     Create a convention test host build for the given <see cref="IConventionFactory" /> in the assembly.
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="factory">The factory that that will be used for the test host.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static ConventionContextBuilder ForTesting(
        this ConventionContextBuilder builder,
        IConventionFactory factory,
        ILoggerFactory? loggerFactory = null,
        string? contentRootPath = null
    )
    {
        EnsureConfigured(builder);
        loggerFactory ??= NullLoggerFactory.Instance;
        var logger = loggerFactory.CreateLogger("TestContext");

        return builder
              .Set(HostType.UnitTest)
              .UseConventionFactory(factory)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
              .WithContentRoot(contentRootPath);
    }

    /// <summary>
    ///     Create a convention test host
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static ConventionContextBuilder ForTesting(
        this ConventionContextBuilder builder,
        ILoggerFactory? loggerFactory = null,
        string? contentRootPath = null
    )
    {
        EnsureConfigured(builder);
        loggerFactory ??= NullLoggerFactory.Instance;
        var logger = loggerFactory.CreateLogger("TestContext");

        return builder
              .Set(HostType.UnitTest)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
              .WithContentRoot(contentRootPath);
    }

    /// <summary>
    ///     Create a convention test host build for the given the list of assemblies.
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="assemblies">The application domain that that will be used for the test host.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static ConventionContextBuilder ForTesting(
        this ConventionContextBuilder builder,
        IEnumerable<Assembly> assemblies,
        ILoggerFactory? loggerFactory = null,
        string? contentRootPath = null
    )
    {
        EnsureConfigured(builder);
        loggerFactory ??= NullLoggerFactory.Instance;
        var logger = loggerFactory.CreateLogger("TestContext");

        return builder
              .Set(HostType.UnitTest)
              .UseAssemblies(assemblies)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
              .WithContentRoot(contentRootPath);
    }

    /// <summary>
    ///     Use the given content root path
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="contentRootPath"></param>
    /// <returns></returns>
    public static ConventionContextBuilder WithContentRoot(this ConventionContextBuilder builder, string? contentRootPath)
    {
        EnsureConfigured(builder);
        if (string.IsNullOrWhiteSpace(contentRootPath))
            return builder;
        return builder.Set("ContentRoot", contentRootPath);
    }

    /// <summary>
    ///     Use the specific <see cref="ILogger" />
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="logger">The logger.</param>
    public static ConventionContextBuilder WithLogger(this ConventionContextBuilder builder, ILogger logger)
    {
        EnsureConfigured(builder);
        return builder.Set(logger);
    }

    /// <summary>
    ///     Use the specific <see cref="ILoggerFactory" />
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public static ConventionContextBuilder WithLoggerFactory(this ConventionContextBuilder builder, ILoggerFactory loggerFactory)
    {
        EnsureConfigured(builder);
        return builder.Set(loggerFactory);
    }

    /// <summary>
    ///     Use the specific environment name
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="environmentName">The environment name.</param>
    public static ConventionContextBuilder WithEnvironmentName(this ConventionContextBuilder builder, string environmentName)
    {
        EnsureConfigured(builder);
        return builder.Set("EnvironmentName", environmentName);
    }

    /// <summary>
    ///     Use to not automatically load conventions from attributes.
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <returns></returns>
    public static ConventionContextBuilder ExcludeConventions(this ConventionContextBuilder builder)
    {
        EnsureConfigured(builder);
        return builder.DisableConventionAttributes();
    }

    /// <summary>
    ///     <para>Use to automatically load conventions from attributes.</para>
    ///     <para>This is the default</para>
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    public static ConventionContextBuilder IncludeConventions(this ConventionContextBuilder builder)
    {
        EnsureConfigured(builder);
        return builder.EnableConventionAttributes();
    }

    private static void EnsureConfigured(ConventionContextBuilder builder)
    {
        if (builder.Properties.ContainsKey("__EnsureConfigured__"))
        {
            return;
        }

        builder.Set("__EnsureConfigured__", true);
        builder.Set("EnvironmentName", "Test");
        builder.Set<ILoggerFactory>(NullLoggerFactory.Instance);
        builder.Set<ILogger>(NullLogger.Instance);

        builder.ConfigureServices(
            services =>
            {
                var loggerFactory = builder.GetOrAdd<ILoggerFactory>(() => NullLoggerFactory.Instance);
                if (loggerFactory != NullLoggerFactory.Instance)
                {
                    services
                       .RemoveAll(typeof(ILoggerFactory))
                       .AddSingleton(loggerFactory);
                }

                var logger = builder.GetOrAdd<ILogger>(() => NullLogger.Instance);
                if (logger != NullLogger.Instance)
                {
                    services
                       .RemoveAll(typeof(ILogger))
                       .AddSingleton(logger);
                }
            }
        );
    }

//    /// <summary>
//    ///     Use a specific configuration object with the test host
//    ///     (This can help avoid re-reading the same configuration over and over)
//    /// </summary>
//    /// <param name="sharedConfiguration">The shared configuration.</param>
//    /// <returns>ConventionTestHostBuilder.</returns>
//    public static ConventionContextBuilder WithConfiguration(this ConventionContextBuilder builder, IConfiguration sharedConfiguration)
//    {EnsureConfigured(builder);
//        _contextBuilder.Set(sharedConfiguration);
//        return this;
//    }
//
//    /// <summary>
//    ///     Use a specific configuration object with the test host
//    ///     (This can help avoid re-reading the same configuration over and over)
//    /// </summary>
//    /// <param name="key">The object to use as a key for shared configuration</param>
//    /// <returns>ConventionTestHostBuilder.</returns>
//    public static ConventionContextBuilder ShareConfiguration(this ConventionContextBuilder builder, object key)
//    {EnsureConfigured(builder);
//        _contextBuilder.Set("ShareConfigurationKey", key);
//        return this;
//    }

//    /// <summary>
//    ///     Create the convention test host with the given defaults
//    /// </summary>
//    /// <exception cref="ArgumentNullException"></exception>
//    /// <returns></returns>
//    public TestHostBuilder Create(Action<TestContext> action)
//    {
//        var environmentName = this.GetOrAdd("EnvironmentName", () => "Test");
//        if (string.IsNullOrEmpty(environmentName)) throw new ArgumentException("Provided environment name was null!");
//
//        // copy properties
//        action(this);
//        var context = ConventionContext.From(_contextBuilder);
//        var builder = new TestHostBuilder(context);
//        RocketHostExtensions.ConfigureRocketSurgery(builder, context);
//        if (_loggerFactory != NullLoggerFactory.Instance)
//        {
//            builder.ConfigureServices(services => services.RemoveAll(typeof(ILoggerFactory)).AddSingleton(_loggerFactory));
//        }
//        // builder
//        //    .ConfigureAppConfiguration(
//        //         (hostingContext, config) =>
//        //         {
//        //             IHostEnvironment env = hostingContext.HostingEnvironment;
//        //
//        //             bool reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
//        //
//        //             config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
//        //                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);
//        //
//        //             if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
//        //             {
//        //                 var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
//        //                 if (appAssembly != null)
//        //                 {
//        //                     config.AddUserSecrets(appAssembly, optional: true);
//        //                 }
//        //             }
//        //
//        //             config.AddEnvironmentVariables();
//        //
//        //             if (_arguments != null)
//        //             {
//        //                 config.AddCommandLine(_arguments);
//        //             }
//        //         }
//        //     );
//
//        if (_reuseConfiguration != null)
//        {
//            builder.Set(typeof(IConfiguration).FullName!, _reuseConfiguration);
//        }
//        else if (_sharedConfigurationKey != null)
//        {
//            builder.Set(typeof(IConfiguration).FullName!, _sharedConfigurationKey);
//        }
//
//        builder.UseEnvironment(environmentName);
//        if (_contentRootPath != null)
//        {
//            builder.UseContentRoot(_contentRootPath);
//        }
//
//        return builder;
//    }
}