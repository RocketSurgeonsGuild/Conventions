using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Clavus.Configuration;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Clavus;

/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
public static class RocketSurgeryConfigurationExtensions
{
    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="context"></param>
    /// <param name="outerConfiguration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IConfigurationBuilder> ApplyConventionsAsync(
        this IConfigurationBuilder configurationBuilder,
        IClavusContext context,
        IConfiguration? outerConfiguration = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        outerConfiguration ??= new ConfigurationBuilder().Build();

        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IConfigurationPart>(convention => convention.Register(context, outerConfiguration, configurationBuilder))
                      .AddHandler<IConfigurationAsyncPart>(convention => convention.Register(context, outerConfiguration, configurationBuilder, cancellationToken))
                      .AddHandler<ConfigurationPart>(convention => convention(context, outerConfiguration, configurationBuilder))
                      .AddHandler<ConfigurationAsyncPart>(convention => convention(context, outerConfiguration, configurationBuilder, cancellationToken))
              )
             .ConfigureAwait(false);
        return configurationBuilder;
    }
}
