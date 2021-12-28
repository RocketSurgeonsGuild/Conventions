using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
public static class RocketSurgeryLoggingExtensions
{
    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <param name="outerConfiguration"></param>
    /// <returns></returns>
    public static IConfigurationBuilder ApplyConventions(
        this IConfigurationBuilder configurationBuilder,
        IConventionContext conventionContext,
        IConfiguration? outerConfiguration = null
    )
    {
        outerConfiguration ??= new ConfigurationBuilder().Build();
        foreach (var item in conventionContext.Conventions.Get<IConfigurationConvention, ConfigurationConvention>())
        {
            if (item is IConfigurationConvention convention)
            {
                convention.Register(conventionContext, outerConfiguration, configurationBuilder);
            }
            else if (item is ConfigurationConvention @delegate)
            {
                @delegate(conventionContext, outerConfiguration, configurationBuilder);
            }
        }

        return configurationBuilder;
    }
}
