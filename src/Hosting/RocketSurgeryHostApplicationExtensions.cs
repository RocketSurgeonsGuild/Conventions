using Microsoft.Extensions.Hosting;

using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

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
    public static async ValueTask ApplyConventionsAsync<TBuilder>(
        this TBuilder hostBuilder,
        IConventionContext context,
        CancellationToken cancellationToken = default
    ) where TBuilder : IHostApplicationBuilder => await context
        .RegisterConventions(
             e => e
                 .AddHandler<IHostApplicationConvention<TBuilder>>(convention => convention.Register(context, hostBuilder))
                 .AddHandler<IHostApplicationAsyncConvention<TBuilder>>(convention => convention.Register(context, hostBuilder, cancellationToken))
                 .AddHandler<HostApplicationConvention<TBuilder>>(convention => convention(context, hostBuilder))
                 .AddHandler<HostApplicationAsyncConvention<TBuilder>>(convention => convention(context, hostBuilder, cancellationToken))
         )
        .ConfigureAwait(false);
}
