using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        var configuration = context.Get<IConfiguration>();
        if (configuration is null)
        {
            configuration = new ConfigurationBuilder().Build();
            context.Logger.LogWarning("Configuration was not found in context");
        }

        services.AddSingleton(context.AssemblyProvider);

        foreach (var item in context.Conventions.Get<IServiceConvention, ServiceConvention, IServiceAsyncConvention, ServiceAsyncConvention>())
        {
            switch (item)
            {
                case IServiceConvention convention:
                    convention.Register(context, configuration, services);
                    break;
                case ServiceConvention @delegate:
                    @delegate(context, configuration, services);
                    break;
                case IServiceAsyncConvention convention:
                    await convention.Register(context, configuration, services, cancellationToken).ConfigureAwait(false);
                    break;
                case ServiceAsyncConvention @delegate:
                    await @delegate(context, configuration, services, cancellationToken).ConfigureAwait(false);
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