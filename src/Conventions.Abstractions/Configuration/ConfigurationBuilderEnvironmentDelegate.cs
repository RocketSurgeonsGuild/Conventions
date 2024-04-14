using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     Delegate for defining application configuration
/// </summary>
/// <param name="builder"></param>
/// <param name="environmentName"></param>
public delegate IEnumerable<ConfigurationBuilderDelegateResult> ConfigurationBuilderEnvironmentDelegate(IConfigurationBuilder builder, string environmentName);
