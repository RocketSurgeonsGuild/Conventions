using DryIoc;

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc;

/// <summary>
///     Extension methods for dryioc
/// </summary>
[PublicAPI]
public static class DryIocExtensions
{
    /// <summary>
    ///     Applies conventions from the given context onto the container
    /// </summary>
    /// <param name="container"></param>
    /// <param name="context"></param>
    /// <param name="services"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static async ValueTask<IContainer> ApplyConventionsAsync(
        this IContainer container,
        IConventionContext context,
        IServiceCollection services,
        CancellationToken cancellationToken = default
    )
    {
#pragma warning disable CA2012
        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IDryIocConvention>(convention => convention.Register(context, context.Configuration, services, container))
                      .AddHandler<IDryIocAsyncConvention>(convention => convention.Register(context, context.Configuration, services, container, cancellationToken))
                      .AddHandler<DryIocConvention>(convention => convention(context, context.Configuration, services, container))
                      .AddHandler<DryIocAsyncConvention>(convention => convention(context, context.Configuration, services, container, cancellationToken))
              )
             .ConfigureAwait(false);
#pragma warning restore CA2012

        return container;
    }
}
