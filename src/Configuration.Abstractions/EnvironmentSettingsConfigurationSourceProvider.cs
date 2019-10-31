using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// Returns a <see cref="IConfigurationSource"/> that may relate to the given file provider.
    /// This is inserted after appsettings.{env}.json
    /// </summary>
    public delegate IConfigurationSource EnvironmentSettingsConfigurationSourceProvider(IFileProvider fileProvider, string environmentName);
}
