using Microsoft.Extensions.Configuration;

namespace Clavus.Configuration.Toml;

/// <summary>
///     A TOML file based <see cref="FileConfigurationSource" />.
/// </summary>
public class TomlConfigurationSource : FileConfigurationSource
{
    /// <inheritdoc />
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new TomlConfigurationProvider(this);
    }
}
