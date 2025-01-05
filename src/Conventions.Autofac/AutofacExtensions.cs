using Autofac;

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;

/// <summary>
///     Autofac extension methods
/// </summary>
public static class AutofacExtensions
{
    /// <summary>
    ///     Applies conventions to the given ContainerBuilder
    /// </summary>
    /// <param name="containerBuilder"></param>
    /// <param name="context"></param>
    /// <param name="services"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static async ValueTask<ContainerBuilder> ApplyConventionsAsync(
        this ContainerBuilder containerBuilder,
        IConventionContext context,
        IServiceCollection services,
        CancellationToken cancellationToken = default
    )
    {
        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IAutofacConvention>(convention => convention.Register(context, context.Configuration, services, containerBuilder))
                      .AddHandler<IAutofacAsyncConvention>(convention => convention.Register(context, context.Configuration, services, containerBuilder, cancellationToken))
                      .AddHandler<AutofacConvention>(convention => convention(context, context.Configuration, services, containerBuilder))
                      .AddHandler<AutofacAsyncConvention>(convention => convention(context, context.Configuration, services, containerBuilder, cancellationToken))
              )
             .ConfigureAwait(false);
        return containerBuilder;
    }
}
