using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Rocket.Surgery.Aspire.Hosting;
using Rocket.Surgery.Aspire.Hosting.Testing;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
[PublicAPI]
public static class RocketSurgeryDistributedApplicationTestingExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask ApplyConventionsAsync(
        this IDistributedApplicationTestingBuilder hostBuilder,
        IConventionContext conventionContext,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in conventionContext.Conventions
                                              .Get<
                                                   IDistributedApplicationTestingConvention,
                                                   DistributedApplicationTestingConvention,
                                                   IDistributedApplicationTestingAsyncConvention,
                                                   DistributedApplicationTestingAsyncConvention
                                               >())
        {
            switch (item)
            {
                case IDistributedApplicationTestingConvention convention:
                    convention.Register(conventionContext, hostBuilder);
                    break;
                case DistributedApplicationTestingConvention @delegate:
                    @delegate(conventionContext, hostBuilder);
                    break;
                case IDistributedApplicationTestingAsyncConvention convention:
                    await convention.Register(conventionContext, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case DistributedApplicationTestingAsyncConvention @delegate:
                    await @delegate(conventionContext, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
    }
}
