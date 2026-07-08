using Microsoft.Extensions.Configuration;

using Tomlyn;

namespace Clavus.Configuration.Toml;

/// <summary>
///     A TOML file based <see cref="FileConfigurationProvider" />.
/// </summary>
/// <remarks>
///     The toml configuration provider
/// </remarks>
/// <param name="source"></param>
public class TomlConfigurationProvider(TomlConfigurationSource source) : FileConfigurationProvider(source)
{
    /// <inheritdoc />
    public override void Load(Stream stream)
    {
        var parser = new TomlConfigurationStreamParser();
        try
        {
            Data = parser.Parse(stream);
        }
        catch (TomlException e)
        {
            throw new FormatException($"Could not parse the TOML file: {e.Message}", e);
        }
    }
}
