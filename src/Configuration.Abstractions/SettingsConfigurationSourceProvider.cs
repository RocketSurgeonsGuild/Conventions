using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// Returns a <see cref="IConfigurationSource"/> that may relate to the given file provider.!--
    /// This is inserted after appsettings.json
    /// </summary>
    public delegate IConfigurationSource SettingsConfigurationSourceProvider(IFileProvider fileProvider);
}
