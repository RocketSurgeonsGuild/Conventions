using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Clavus.Configuration;

/// <summary>
///     IConfigurationPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IConfigurationPart : IClavusPart
{
    /// <summary>
    ///     Register additional configuration providers with the configuration builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IClavusContext context, IConfiguration configuration, IConfigurationBuilder builder);
}
