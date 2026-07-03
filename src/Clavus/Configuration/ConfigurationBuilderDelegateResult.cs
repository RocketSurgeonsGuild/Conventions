using Microsoft.Extensions.Configuration;

namespace Clavus.Configuration;

/// <summary>
///     The result from a given application
/// </summary>
/// <param name="Path"></param>
/// <param name="Factory"></param>
[PublicAPI]
public record ConfigurationBuilderDelegateResult(string Path, Func<Stream?, IConfigurationSource> Factory);
