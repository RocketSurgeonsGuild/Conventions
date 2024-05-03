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
        outerConfiguration ??= new ConfigurationBuilder().Build();
        foreach (var item in context.Conventions.Get<
                     IConfigurationConvention,
                     ConfigurationConvention,
                     IConfigurationAsyncConvention,
                     ConfigurationAsyncConvention
                 >())
        {
            switch (item)
            {
                case IConfigurationConvention convention:
                    convention.Register(context, outerConfiguration, configurationBuilder);
                    break;
                case ConfigurationConvention @delegate:
                    @delegate(context, outerConfiguration, configurationBuilder);
                    break;
                case IConfigurationAsyncConvention convention:
                    await convention.Register(context, outerConfiguration, configurationBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case ConfigurationAsyncConvention @delegate:
                    await @delegate(context, outerConfiguration, configurationBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return configurationBuilder;
    }
}