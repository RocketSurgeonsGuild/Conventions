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
