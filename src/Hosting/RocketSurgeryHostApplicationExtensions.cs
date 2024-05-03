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
    public static async ValueTask ApplyConventionsAsync(
        this IHostApplicationBuilder hostBuilder,
        IConventionContext context,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in context.Conventions
                                    .Get<
                                         IHostApplicationConvention,
                                         HostApplicationConvention,
                                         IHostApplicationAsyncConvention,
                                         HostApplicationAsyncConvention
                                     >())
        {
            switch (item)
            {
                case IHostApplicationConvention convention:
                    convention.Register(context, hostBuilder);
                    break;
                case HostApplicationConvention @delegate:
                    @delegate(context, hostBuilder);
                    break;
                case IHostApplicationAsyncConvention convention:
                    await convention.Register(context, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case HostApplicationAsyncConvention @delegate:
                    await @delegate(context, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
    }
}