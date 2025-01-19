﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Generated_DistributedApplicationBuilder_Extensions.g.cs
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8603, CS8602, CS8618
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using global::Aspire.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<global::Aspire.Hosting.IDistributedApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

namespace Rocket.Surgery.Aspire.Hosting;

internal static partial class GeneratedRocketDistributedApplicationBuilderExtensions
{
    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.IDistributedApplicationBuilder builder, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.IDistributedApplicationBuilder> builder, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.IDistributedApplicationBuilder builder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.IDistributedApplicationBuilder> builder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.IDistributedApplicationBuilder builder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.IDistributedApplicationBuilder> builder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
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
    public static ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.IDistributedApplicationBuilder builder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.IDistributedApplicationBuilder> builder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
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
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this global::Aspire.Hosting.IDistributedApplicationBuilder builder, ConventionContextBuilder contextBuilder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextBuilder);
        return await global::Rocket.Surgery.Aspire.Hosting.RocketDistributedApplicationExtensions.Configure(builder, static b => b.Build(), contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<global::Aspire.Hosting.DistributedApplication> ConfigureRocketSurgery(this Task<global::Aspire.Hosting.IDistributedApplicationBuilder> builder, ConventionContextBuilder contextBuilder, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
}