//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Generated_HostApplicationBuilder_Extensions_Serilog.g.cs
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8603, CS8602, CS8618
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using global::Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<global::Microsoft.Extensions.Hosting.HostApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.Hosting;

internal static partial class GeneratedRocketHostApplicationBuilderExtensions
{
    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<global::Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this global::Microsoft.Extensions.Hosting.HostApplicationBuilder builder, ILogger logger, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<global::Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, ILogger logger, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this global::Microsoft.Extensions.Hosting.HostApplicationBuilder builder, ILogger logger, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<global::Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, ILogger logger, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this global::Microsoft.Extensions.Hosting.HostApplicationBuilder builder, ILogger logger, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        await action(contextBuilder);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<global::Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, ILogger logger, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        await action(contextBuilder);
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<global::Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this global::Microsoft.Extensions.Hosting.HostApplicationBuilder builder, ILogger logger, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        action(contextBuilder);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<global::Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, ILogger logger, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        action(contextBuilder);
        return await ConfigureRocketSurgery(await builder, action, cancellationToken);
    }
}