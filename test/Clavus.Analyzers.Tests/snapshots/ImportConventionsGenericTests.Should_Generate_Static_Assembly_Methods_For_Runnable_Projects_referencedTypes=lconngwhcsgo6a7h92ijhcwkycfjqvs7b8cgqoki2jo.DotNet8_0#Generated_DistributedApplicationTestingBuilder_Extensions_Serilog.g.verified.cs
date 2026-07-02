//HintName: Rocket.Surgery.Clavus.Analyzers/Rocket.Surgery.Clavus.ClavusAttributesGenerator/Generated_DistributedApplicationTestingBuilder_Extensions_Serilog.g.cs
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8603, CS8602, CS8618
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using global::Aspire.Hosting.Testing;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Clavus;
using AppDelegate =
    System.Func<global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Clavus.ClavusContextBuilder>>;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.Clavus.Aspire.Testing;

internal static partial class GeneratedRocketDistributedApplicationTestingBuilderExtensions
{
    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder builder, ILogger logger, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);
        var contextBuilder = ClavusContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder> builder, ILogger logger, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);
        var contextBuilder = ClavusContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder builder, ILogger logger, Func<ClavusContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder> builder, ILogger logger, Func<ClavusContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder builder, ILogger logger, Func<ClavusContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        await action(contextBuilder);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder> builder, ILogger logger, Func<ClavusContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        await action(contextBuilder);
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder builder, ILogger logger, Action<ClavusContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        action(contextBuilder);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder> builder, ILogger logger, Action<ClavusContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        action(contextBuilder);
        return await ConfigureRocketSurgery(await builder, action, cancellationToken);
    }
}