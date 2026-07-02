using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Clavus.Configuration;

/// <summary>
///     Register additional configuration providers with the configuration builder
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
[PublicAPI]
public delegate void ConfigurationPart(IClavusContext context, IConfiguration configuration, IConfigurationBuilder builder);
