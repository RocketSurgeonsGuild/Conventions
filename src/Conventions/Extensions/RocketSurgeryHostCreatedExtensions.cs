using Rocket.Surgery.Conventions.Hosting;

namespace Rocket.Surgery.Conventions.Extensions;

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
    public static async ValueTask<IConventionContext> ApplyHostCreatedConventionsAsync<THost>(
        this IConventionContext context,
        THost host,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IHostCreatedConvention<THost>>(convention => convention.Register(context, host))
                      .AddHandler<IHostCreatedAsyncConvention<THost>>(convention => convention.Register(context, host, cancellationToken))
                      .AddHandler<HostCreatedConvention<THost>>(convention => convention(context, host))
                      .AddHandler<HostCreatedAsyncConvention<THost>>(convention => convention(context, host, cancellationToken))
              )
             .ConfigureAwait(false);
        return context;
    }
}
