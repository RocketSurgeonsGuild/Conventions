#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8603, CS8602, CS8618
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Clavus;

namespace {RocketUsing};

internal static partial class GeneratedRocketHostApplicationBuilderExtensions
{
    public static ValueTask<IHost> ConfigureClavus(this HostApplicationBuilder builder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var contextBuilder = ClavusContextBuilder.Create(_LoadClavusParts.OrCallerConventions());
        return ConfigureClavus(builder, contextBuilder, cancellationToken);
    }

    public static async ValueTask<IHost> ConfigureClavus(this Task<HostApplicationBuilder> builder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var contextBuilder = ClavusContextBuilder.Create(_LoadClavusParts.OrCallerConventions());
        return await ConfigureClavus(await builder, contextBuilder, cancellationToken);
    }

    public static async ValueTask<IHost> ConfigureClavus(this HostApplicationBuilder builder, Func<ClavusContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(_LoadClavusParts.OrCallerConventions());
        await action(contextBuilder, cancellationToken);
        return await ConfigureClavus(builder, contextBuilder, cancellationToken);
    }

    public static async ValueTask<IHost> ConfigureClavus(this Task<HostApplicationBuilder> builder, Func<ClavusContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(_LoadClavusParts.OrCallerConventions());
        await action(contextBuilder, cancellationToken);
        return await ConfigureClavus(await builder, contextBuilder, cancellationToken);
    }

    public static async ValueTask<IHost> ConfigureClavus(this HostApplicationBuilder builder, Func<ClavusContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(_LoadClavusParts.OrCallerConventions());
        await action(contextBuilder);
        return await ConfigureClavus(builder, contextBuilder, cancellationToken);
    }

    public static async ValueTask<IHost> ConfigureClavus(this Task<HostApplicationBuilder> builder, Func<ClavusContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(_LoadClavusParts.OrCallerConventions());
        await action(contextBuilder);
        return await ConfigureClavus(await builder, contextBuilder, cancellationToken);
    }

    public static ValueTask<IHost> ConfigureClavus(this HostApplicationBuilder builder, Action<ClavusContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(_LoadClavusParts.OrCallerConventions());
        action(contextBuilder);
        return ConfigureClavus(builder, contextBuilder, cancellationToken);
    }

    public static async ValueTask<IHost> ConfigureClavus(this Task<HostApplicationBuilder> builder, Action<ClavusContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = ClavusContextBuilder.Create(_LoadClavusParts.OrCallerConventions());
        action(contextBuilder);
        return await ConfigureClavus(await builder, action, cancellationToken);
    }

    public static async ValueTask<IHost> ConfigureClavus(this HostApplicationBuilder builder, ClavusContextBuilder contextBuilder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextBuilder);
        return await global::Clavus.Hosting.RocketHostApplicationExtensions.Configure(builder, static b => b.Build(), contextBuilder, cancellationToken);
    }

    public static async ValueTask<IHost> ConfigureClavus(this Task<HostApplicationBuilder> builder, ClavusContextBuilder contextBuilder, CancellationToken cancellationToken = default)
        => await ConfigureClavus(await builder, contextBuilder, cancellationToken);
}
#pragma warning restore CS0105, CA1002, CA1034, CA1822, CS8603, CS8602, CS8618
