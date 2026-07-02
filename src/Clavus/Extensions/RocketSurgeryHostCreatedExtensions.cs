using Rocket.Surgery.Clavus.Hosting;

namespace Rocket.Surgery.Clavus.Extensions;

/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
internal static class RocketSurgeryHostCreatedExtensions
{
    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="context"></param>
    /// <param name="host">The host</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IClavusContext> ApplyHostCreatedPartsAsync<THost>(
        this IClavusContext context,
        THost host,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IHostCreatedPart<THost>>(convention => convention.Register(context, host))
                      .AddHandler<IHostCreatedAsyncPart<THost>>(convention => convention.Register(context, host, cancellationToken))
                      .AddHandler<HostCreatedPart<THost>>(convention => convention(context, host))
                      .AddHandler<HostCreatedAsyncPart<THost>>(convention => convention(context, host, cancellationToken))
              )
             .ConfigureAwait(false);
        return context;
    }
}
