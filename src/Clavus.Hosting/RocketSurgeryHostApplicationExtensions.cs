using Clavus.Hosting;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Clavus;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
[PublicAPI]
public static class RocketSurgeryHostApplicationExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask ApplyPartsAsync<TBuilder>(
        this TBuilder hostBuilder,
        IClavusContext context,
        CancellationToken cancellationToken = default
    ) where TBuilder : IHostApplicationBuilder => await context
                                                       .RegisterConventions(
                                                            e => e
                                                                .AddHandler<IHostApplicationPart<TBuilder>>(convention => convention.Register(context, hostBuilder))
                                                                .AddHandler<IHostApplicationAsyncPart<TBuilder>>(convention => convention.Register(context, hostBuilder, cancellationToken))
                                                                .AddHandler<HostApplicationPart<TBuilder>>(convention => convention(context, hostBuilder))
                                                                .AddHandler<HostApplicationAsyncPart<TBuilder>>(convention => convention(context, hostBuilder, cancellationToken))
                                                        )
                                                       .ConfigureAwait(false);
}
