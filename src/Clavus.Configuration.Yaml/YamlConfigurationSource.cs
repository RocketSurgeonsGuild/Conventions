using Microsoft.Extensions.Configuration;

namespace Clavus.Configuration.Yaml;

/// <summary>
///     A YAML file based <see cref="FileConfigurationSource" />.
/// </summary>
public class YamlConfigurationSource : FileConfigurationSource
{
    /// <inheritdoc />
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new YamlConfigurationProvider(this);
    }
}
