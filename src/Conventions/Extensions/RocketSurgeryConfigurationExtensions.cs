using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions.Configuration;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

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
        IConventionContext context,
        IConfiguration? outerConfiguration = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        outerConfiguration ??= new ConfigurationBuilder().Build();

        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IConfigurationConvention>(convention => convention.Register(context, outerConfiguration, configurationBuilder))
                      .AddHandler<IConfigurationAsyncConvention>(convention => convention.Register(context, outerConfiguration, configurationBuilder, cancellationToken))
                      .AddHandler<ConfigurationConvention>(convention => convention(context, outerConfiguration, configurationBuilder))
                      .AddHandler<ConfigurationAsyncConvention>(convention => convention(context, outerConfiguration, configurationBuilder, cancellationToken))
              )
             .ConfigureAwait(false);
        return configurationBuilder;
    }
}
