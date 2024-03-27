using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

internal class StaticConfigurationProvider : ConfigurationProvider
{
    public StaticConfigurationProvider(IDictionary<string, string?> data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
}
