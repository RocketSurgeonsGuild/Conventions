//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Generated_WebAssemblyBuilder_Extensions_Serilog.g.cs
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8603, CS8602, CS8618
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using global::Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.WebAssembly.Hosting;

internal static partial class GeneratedRocketWebAssemblyHostBuilderExtensions
{
    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, ILogger logger, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, ILogger logger, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, ILogger logger, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, ILogger logger, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, ILogger logger, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, ILogger logger, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
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
    public static ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, ILogger logger, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, ILogger logger, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions()).UseLogger(logger);
        action(contextBuilder);
        return await ConfigureRocketSurgery(await builder, action, cancellationToken);
    }
}