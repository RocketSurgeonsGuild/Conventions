using Microsoft.Extensions.Configuration;
using YamlDotNet.Core;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

/// <summary>
///     A YAML file based <see cref="FileConfigurationProvider" />.
/// </summary>
public class YamlConfigurationProvider : FileConfigurationProvider
{
    /// <summary>
    ///     The yaml configuration provider
    /// </summary>
    /// <param name="source"></param>
    public YamlConfigurationProvider(YamlConfigurationSource source) : base(source) { }

    /// <inheritdoc />
    public override void Load(Stream stream)
    {
        var parser = new YamlConfigurationStreamParser();
        try
        {
            Data = parser.Parse(stream);
        }
        catch (YamlException e)
        {
            throw new FormatException($"Could not parse the YAML file: {e.Message}", e);
        }
    }
}
