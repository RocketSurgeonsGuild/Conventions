using DryIoc;
using Microsoft.Extensions.Configuration;
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
        var configuration = context.Get<IConfiguration>() ?? throw new ArgumentException("Configuration was not found in context");
        foreach (var item in context.Conventions.Get<IDryIocConvention, DryIocConvention>())
        {
            container = item switch
                        {
                            IDryIocConvention convention => convention.Register(context, configuration, services, container),
                            DryIocConvention @delegate   => @delegate(context, configuration, services, container),
                            IDryIocAsyncConvention convention => await convention.Register(
                                context,
                                configuration,
                                services,
                                container,
                                cancellationToken
                            ),
                            DryIocAsyncConvention @delegate => await @delegate(context, configuration, services, container, cancellationToken),
                            _                               => container,
                        };
        }

        return container;
    }
}