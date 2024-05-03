using Aspire.Hosting.Testing;
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
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask ApplyConventionsAsync(
        this IDistributedApplicationTestingBuilder hostBuilder,
        IConventionContext context,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in context.Conventions
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
                    convention.Register(context, hostBuilder);
                    break;
                case DistributedApplicationTestingConvention @delegate:
                    @delegate(context, hostBuilder);
                    break;
                case IDistributedApplicationTestingAsyncConvention convention:
                    await convention.Register(context, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case DistributedApplicationTestingAsyncConvention @delegate:
                    await @delegate(context, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
    }
}