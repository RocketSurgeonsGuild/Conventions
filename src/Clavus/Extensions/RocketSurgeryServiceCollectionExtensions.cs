using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Clavus.DependencyInjection;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1848
namespace Rocket.Surgery.Clavus;

/// <summary>
///     Extension method to apply service conventions
/// </summary>
public static class RocketSurgeryServiceCollectionExtensions
{
    /// <summary>
    ///     Apply service conventions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IServiceCollection> ApplyConventionsAsync(
        this IServiceCollection services,
        IClavusContext context,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IServicePart>(convention => convention.Register(context, context.Configuration, services))
                      .AddHandler<IServiceAsyncPart>(convention => convention.Register(context, context.Configuration, services, cancellationToken))
                      .AddHandler<ServicePart>(convention => convention(context, context.Configuration, services))
                      .AddHandler<ServiceAsyncPart>(convention => convention(context, context.Configuration, services, cancellationToken))
              )
             .ConfigureAwait(false);
        return services;
    }
}
