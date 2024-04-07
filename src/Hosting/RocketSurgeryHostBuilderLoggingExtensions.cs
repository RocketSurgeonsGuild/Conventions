using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
[PublicAPI]
public static class RocketSurgeryHostBuilderLoggingExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask ApplyConventionsAsync(
        this IHostApplicationBuilder hostBuilder,
        IConventionContext conventionContext,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in conventionContext.Conventions
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
                    convention.Register(conventionContext, hostBuilder);
                    break;
                case HostApplicationConvention @delegate:
                    @delegate(conventionContext, hostBuilder);
                    break;
                case IHostApplicationAsyncConvention convention:
                    await convention.Register(conventionContext, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case HostApplicationAsyncConvention @delegate:
                    await @delegate(conventionContext, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
    }
}
