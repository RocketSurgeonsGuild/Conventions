using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     Represents in-memory data as an <see cref="IConfigurationSource" />.
/// </summary>
internal class AppSettingsConfigurationSource : IConfigurationSource
{
    public IEnumerable<string> Args { get; }
    private readonly AppSettingsConfigurationProvider _provider;

    public AppSettingsConfigurationSource(IEnumerable<string> args)
    {
        Args = args;
        _provider = new AppSettingsConfigurationProvider();
    }

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
