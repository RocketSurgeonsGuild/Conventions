using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
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
    /// <param name="conventionContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IServiceCollection> ApplyConventionsAsync(
        this IServiceCollection services,
        IConventionContext conventionContext,
        CancellationToken cancellationToken = default
    )
    {
        var configuration = conventionContext.Get<IConfiguration>();
        if (configuration is null)
        {
            configuration = new ConfigurationBuilder().Build();
            conventionContext.Logger.LogWarning("Configuration was not found in context");
        }

        services.AddSingleton(conventionContext.AssemblyProvider);

        foreach (var item in conventionContext.Conventions.Get<IServiceConvention, ServiceConvention, IServiceAsyncConvention, ServiceAsyncConvention>())
        {
            switch (item)
            {
                case IServiceConvention convention:
                    convention.Register(conventionContext, configuration, services);
                    break;
                case ServiceConvention @delegate:
                    @delegate(conventionContext, configuration, services);
                    break;
                case IServiceAsyncConvention convention:
                    await convention.Register(conventionContext, configuration, services, cancellationToken).ConfigureAwait(false);
                    break;
                case ServiceAsyncConvention @delegate:
                    await @delegate(conventionContext, configuration, services, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return services;
    }
}

internal class LoggingBuilder : ILoggingBuilder
{
    public LoggingBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}
