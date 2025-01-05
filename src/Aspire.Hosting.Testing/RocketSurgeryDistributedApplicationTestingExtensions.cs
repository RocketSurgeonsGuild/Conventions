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
    public static ValueTask ApplyConventionsAsync(
        this IDistributedApplicationTestingBuilder hostBuilder,
        IConventionContext context,
        CancellationToken cancellationToken = default
    ) => context
       .RegisterConventions(
            e => e
                .AddHandler<IDistributedApplicationTestingConvention>(convention => convention.Register(context, hostBuilder))
                .AddHandler<IDistributedApplicationTestingAsyncConvention>(convention => convention.Register(context, hostBuilder, cancellationToken))
                .AddHandler<DistributedApplicationTestingConvention>(convention => convention(context, hostBuilder))
                .AddHandler<DistributedApplicationTestingAsyncConvention>(convention => convention(context, hostBuilder, cancellationToken))
        );
}
