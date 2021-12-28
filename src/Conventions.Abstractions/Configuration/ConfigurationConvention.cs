using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     Register additional configuration providers with the configuration builder
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
public delegate void ConfigurationConvention(IConventionContext context, IConfiguration configuration, IConfigurationBuilder builder);
