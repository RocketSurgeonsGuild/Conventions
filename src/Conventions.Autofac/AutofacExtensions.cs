using Autofac;
using Microsoft.Extensions.Configuration;
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
    /// <param name="conventionContext"></param>
    /// <param name="services"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static async ValueTask<ContainerBuilder> ApplyConventionsAsync(
        this ContainerBuilder containerBuilder,
        IConventionContext conventionContext,
        IServiceCollection services,
        CancellationToken cancellationToken = default
    )
    {
        var configuration = conventionContext.Get<IConfiguration>() ?? throw new ArgumentException("Configuration was not found in context");
        foreach (var item in conventionContext.Conventions.Get<IAutofacConvention, AutofacConvention>())
        {
            switch (item)
            {
                case IAutofacConvention convention:
                    convention.Register(conventionContext, configuration, services, containerBuilder);
                    break;
                case AutofacConvention @delegate:
                    @delegate(conventionContext, configuration, services, containerBuilder);
                    break;
                case IAutofacAsyncConvention convention:
                    await convention.Register(conventionContext, configuration, services, containerBuilder, cancellationToken);
                    break;
                case AutofacAsyncConvention @delegate:
                    await @delegate(conventionContext, configuration, services, containerBuilder, cancellationToken);
                    break;
            }
        }

        return containerBuilder;
    }
}