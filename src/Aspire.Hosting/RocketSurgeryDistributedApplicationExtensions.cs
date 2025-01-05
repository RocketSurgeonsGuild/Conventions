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
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static ValueTask ApplyConventionsAsync(
        this IDistributedApplicationBuilder hostBuilder,
        IConventionContext context,
        CancellationToken cancellationToken = default
    ) => context
        .RegisterConventions(
             e => e
                 .AddHandler<IDistributedApplicationConvention>(convention => convention.Register(context, hostBuilder))
                 .AddHandler<IDistributedApplicationAsyncConvention>(convention => convention.Register(context, hostBuilder, cancellationToken))
                 .AddHandler<DistributedApplicationConvention>(convention => convention(context, hostBuilder))
                 .AddHandler<DistributedApplicationAsyncConvention>(convention => convention(context, hostBuilder, cancellationToken))
         );
}
