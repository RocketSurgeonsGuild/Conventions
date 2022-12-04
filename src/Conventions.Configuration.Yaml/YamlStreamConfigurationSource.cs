using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

/// <summary>
///     A YAML file based <see cref="FileConfigurationSource" />.
/// </summary>
public class YamlStreamConfigurationSource : StreamConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new YamlStreamConfigurationProvider(this);
    }
}
