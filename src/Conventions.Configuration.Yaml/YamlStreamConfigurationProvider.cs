using Microsoft.Extensions.Configuration;
using YamlDotNet.Core;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

/// <summary>
///     A YAML file based <see cref="FileConfigurationProvider" />.
/// </summary>
public class YamlStreamConfigurationProvider : StreamConfigurationProvider
{
    /// <inheritdoc />
    public YamlStreamConfigurationProvider(YamlStreamConfigurationSource source) : base(source)
    {
    }

    /// <inheritdoc />
    public override void Load(Stream stream)
    {
        var parser = new YamlConfigurationFileParser();
        try
        {
            Data = parser.Parse(stream);
        }
        catch (YamlException e)
        {
            throw new FormatException("Error_YamlParseError - " + e.Message, e);
        }
    }
}
