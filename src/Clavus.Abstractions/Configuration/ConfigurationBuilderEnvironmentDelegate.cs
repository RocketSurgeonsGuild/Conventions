using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Clavus.Configuration;

/// <summary>
///     Delegate for defining application configuration
/// </summary>
/// <param name="builder"></param>
/// <param name="environmentName"></param>
[PublicAPI]
public delegate IEnumerable<ConfigurationBuilderDelegateResult> ConfigurationBuilderEnvironmentDelegate(IConfigurationBuilder builder, string environmentName);
