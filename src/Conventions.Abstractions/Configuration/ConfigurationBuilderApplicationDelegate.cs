using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     Delegate for defining application configuration
/// </summary>
/// <param name="builder"></param>
public delegate IEnumerable<ConfigurationBuilderDelegateResult> ConfigurationBuilderApplicationDelegate(IConfigurationBuilder builder);

/// <summary>
///     The result from a given application
/// </summary>
/// <param name="Path"></param>
/// <param name="Factory"></param>
public record ConfigurationBuilderDelegateResult(string Path, Func<Stream?, IConfigurationSource> Factory);
