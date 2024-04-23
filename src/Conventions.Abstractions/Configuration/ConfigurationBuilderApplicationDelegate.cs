using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     Delegate for defining application configuration
/// </summary>
/// <param name="builder"></param>
[PublicAPI]
public delegate IEnumerable<ConfigurationBuilderDelegateResult> ConfigurationBuilderApplicationDelegate(IConfigurationBuilder builder);