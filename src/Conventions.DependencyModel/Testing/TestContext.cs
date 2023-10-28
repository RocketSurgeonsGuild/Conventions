using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions.Testing;

/// <summary>
///     A convention test host builder
/// </summary>
public static class DependencyModelTestConventionContextBuilderExtensions
{
    /// <summary>
    ///     Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="type">The type that that will be used to load the <see cref="DependencyContext" />.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="contentRootPath">The content root path for the host environment.</param>
    public static ConventionContextBuilder ForTesting(
        this ConventionContextBuilder builder, Type type, ILoggerFactory? loggerFactory = null, string? contentRootPath = null
    )
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
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
        this ConventionContextBuilder builder, Assembly assembly, ILoggerFactory? loggerFactory = null, string? contentRootPath = null
    )
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
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
        this ConventionContextBuilder builder, DependencyContext context, ILoggerFactory? loggerFactory = null, string? contentRootPath = null
    )
    {
        loggerFactory ??= NullLoggerFactory.Instance;
        var logger = loggerFactory.CreateLogger("TestContext");

        return builder
              .Set(HostType.UnitTest)
              .UseDependencyContext(context)
              .WithLoggerFactory(loggerFactory)
              .WithLogger(logger)
              .WithContentRoot(contentRootPath);
    }
}
