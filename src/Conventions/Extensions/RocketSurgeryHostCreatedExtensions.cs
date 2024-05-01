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
    /// <param name="conventionContext"></param>
    /// <param name="host">The host</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IConventionContext> ApplyHostCreatedConventionsAsync<THost>(
        this IConventionContext conventionContext,
        THost host,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in conventionContext.Conventions
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
                    convention.Register(conventionContext, host);
                    break;
                case HostCreatedConvention<THost> @delegate:
                    @delegate(conventionContext, host);
                    break;
                case IHostCreatedAsyncConvention<THost> convention:
                    await convention.Register(conventionContext, host, cancellationToken).ConfigureAwait(false);
                    break;
                case HostCreatedAsyncConvention<THost> @delegate:
                    await @delegate(conventionContext, host, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return conventionContext;
    }
}
