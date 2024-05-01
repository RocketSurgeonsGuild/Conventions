using Aspire.Hosting;
using Rocket.Surgery.Aspire.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
[PublicAPI]
public static class RocketSurgeryDistributedApplicationExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask ApplyConventionsAsync(
        this IDistributedApplicationBuilder hostBuilder,
        IConventionContext conventionContext,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in conventionContext.Conventions
                                              .Get<
                                                   IDistributedApplicationConvention,
                                                   DistributedApplicationConvention,
                                                   IDistributedApplicationAsyncConvention,
                                                   DistributedApplicationAsyncConvention
                                               >())
        {
            switch (item)
            {
                case IDistributedApplicationConvention convention:
                    convention.Register(conventionContext, hostBuilder);
                    break;
                case DistributedApplicationConvention @delegate:
                    @delegate(conventionContext, hostBuilder);
                    break;
                case IDistributedApplicationAsyncConvention convention:
                    await convention.Register(conventionContext, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case DistributedApplicationAsyncConvention @delegate:
                    await @delegate(conventionContext, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
    }
}
