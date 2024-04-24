using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

/// <summary>
///     Represents in-memory data as an <see cref="IConfigurationSource" />.
/// </summary>
internal class AppSettingsConfigurationSource(IEnumerable<string> args) : IConfigurationSource
{
    private readonly AppSettingsConfigurationProvider _provider = new();

    public IEnumerable<string> Args { get; } = args;

    public void Update(CommandContext commandContext, AppSettings appSettings)
    {
        _provider.Update(commandContext, appSettings);
    }

    /// <summary>
    ///     Builds the <see cref="AppSettingsConfigurationSource" /> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" />.</param>
    /// <returns>A <see cref="AppSettingsConfigurationSource" /></returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _provider;
    }
}
