//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Generated_WebAssemblyBuilder_Extensions.g.cs
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

namespace Rocket.Surgery.WebAssembly.Hosting;

internal static partial class GeneratedRocketWebAssemblyHostBuilderExtensions
{
    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions());
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions());
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions());
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions());
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions());
        await action(contextBuilder);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions());
        await action(contextBuilder);
        return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions());
        action(contextBuilder);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ConventionContextBuilder.Create(global::TestProject.Conventions.Imports.Instance.OrCallerConventions());
        action(contextBuilder);
        return await ConfigureRocketSurgery(await builder, action, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder builder, ConventionContextBuilder contextBuilder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextBuilder);
        return await global::Rocket.Surgery.WebAssembly.Hosting.RocketWebAssemblyExtensions.Configure(builder, static b => b.Build(), contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost> ConfigureRocketSurgery(this Task<global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder> builder, ConventionContextBuilder contextBuilder, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
}