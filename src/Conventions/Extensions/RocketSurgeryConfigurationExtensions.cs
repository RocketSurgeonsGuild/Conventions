using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
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
    /// <param name="conventionContext"></param>
    /// <param name="outerConfiguration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IConfigurationBuilder> ApplyConventionsAsync(
        this IConfigurationBuilder configurationBuilder,
        IConventionContext conventionContext,
        IConfiguration? outerConfiguration = null,
        CancellationToken cancellationToken = default
    )
    {
        outerConfiguration ??= new ConfigurationBuilder().Build();
        foreach (var item in conventionContext.Conventions.Get<
                     IConfigurationConvention,
                     ConfigurationConvention,
                     IConfigurationAsyncConvention,
                     ConfigurationAsyncConvention
                 >())
        {
            switch (item)
            {
                case IConfigurationConvention convention:
                    convention.Register(conventionContext, outerConfiguration, configurationBuilder);
                    break;
                case ConfigurationConvention @delegate:
                    @delegate(conventionContext, outerConfiguration, configurationBuilder);
                    break;
                case IConfigurationAsyncConvention convention:
                    await convention.Register(conventionContext, outerConfiguration, configurationBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case ConfigurationAsyncConvention @delegate:
                    await @delegate(conventionContext, outerConfiguration, configurationBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return configurationBuilder;
    }
}
