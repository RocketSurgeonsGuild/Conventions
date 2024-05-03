using Rocket.Surgery.Conventions.Hosting;
using Rocket.Surgery.Conventions.Setup;

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
        foreach (var item in context.Conventions
                                              .Get<
                                                   IHostCreatedConvention<THost>,
                                                   HostCreatedConvention<THost>,
                                                   IHostCreatedAsyncConvention<THost>,
                                                   HostCreatedAsyncConvention<THost>>()
                )
        {
            switch (item)
            {
                case IHostCreatedConvention<THost> convention:
                    convention.Register(context, host);
                    break;
                case HostCreatedConvention<THost> @delegate:
                    @delegate(context, host);
                    break;
                case IHostCreatedAsyncConvention<THost> convention:
                    await convention.Register(context, host, cancellationToken).ConfigureAwait(false);
                    break;
                case HostCreatedAsyncConvention<THost> @delegate:
                    await @delegate(context, host, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return context;
    }
}
