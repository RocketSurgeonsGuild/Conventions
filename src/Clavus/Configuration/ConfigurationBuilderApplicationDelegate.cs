using Microsoft.Extensions.Configuration;

namespace Clavus.Configuration;

/// <summary>
///     Delegate for defining application configuration
/// </summary>
/// <param name="builder"></param>
[PublicAPI]
public delegate IEnumerable<ConfigurationBuilderDelegateResult> ConfigurationBuilderApplicationDelegate(IConfigurationBuilder builder);
