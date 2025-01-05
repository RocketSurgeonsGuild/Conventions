using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions.DependencyInjection;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1848
namespace Rocket.Surgery.Conventions;

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
        IConventionContext context,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IServiceConvention>(convention => convention.Register(context, context.Configuration, services))
                      .AddHandler<IServiceAsyncConvention>(convention => convention.Register(context, context.Configuration, services, cancellationToken))
                      .AddHandler<ServiceConvention>(convention => convention(context, context.Configuration, services))
                      .AddHandler<ServiceAsyncConvention>(convention => convention(context, context.Configuration, services, cancellationToken))
              )
             .ConfigureAwait(false);
        return services;
    }
}
