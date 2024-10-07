using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Testing;

#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     A convention test host builder
/// </summary>
public static class DependencyModelTestConventionContextBuilderExtensions
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
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="type">The type that that will be used to load the <see cref="DependencyContext" />.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static ConventionContextBuilder ForTesting(
        this ConventionContextBuilder builder,
        Type type,
        ILoggerFactory? loggerFactory = null,
        string? contentRootPath = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(type);
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return ForTesting(builder, DependencyContext.Load(type.Assembly)!, loggerFactory, contentRootPath);
    }

    /// <summary>
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static ConventionContextBuilder ForTesting(
        this ConventionContextBuilder builder,
        Assembly assembly,
        ILoggerFactory? loggerFactory = null,
        string? contentRootPath = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return ForTesting(builder, DependencyContext.Load(assembly)!, loggerFactory, contentRootPath);
    }

    /// <summary>
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="context">The context that that will be used for the test host.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static ConventionContextBuilder ForTesting(
        this ConventionContextBuilder builder,
        DependencyContext context,
        ILoggerFactory? loggerFactory = null,
        string? contentRootPath = null
    )
    {
        EnsureConfigured(builder);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(context);
        loggerFactory ??= NullLoggerFactory.Instance;
        var logger = loggerFactory.CreateLogger("TestContext");

        return builder
              .Set(HostType.UnitTest)
              .UseDependencyContext(context)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
              .WithContentRoot(contentRootPath);
    }

    private static void EnsureConfigured(ConventionContextBuilder builder)
    {
        if (builder.Properties.ContainsKey("__EnsureConfigured__")) return;

        builder.Set("__EnsureConfigured__", true);
        builder.Set("EnvironmentName", "Test");
        builder.Set<ILoggerFactory>(NullLoggerFactory.Instance);
        builder.Set<ILogger>(NullLogger.Instance);

        builder.ConfigureServices(
            services =>
            {
                var loggerFactory = builder.GetOrAdd<ILoggerFactory>(() => NullLoggerFactory.Instance);
                if (loggerFactory != NullLoggerFactory.Instance)
                    services
                       .RemoveAll(typeof(ILoggerFactory))
                       .AddSingleton(loggerFactory);

                var logger = builder.GetOrAdd<ILogger>(() => NullLogger.Instance);
                if (logger != NullLogger.Instance)
                    services
                       .RemoveAll(typeof(ILogger))
                       .AddSingleton(logger);
            }
        );
    }
}
