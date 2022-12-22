using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     Represents in-memory data as an <see cref="IConfigurationSource" />.
/// </summary>
internal class AppSettingsConfigurationSource : IConfigurationSource
{
    private readonly AppSettingsConfigurationProvider _provider;
    private readonly CommandLineConfigurationSource? _commandLineConfigurationSource;

    public AppSettingsConfigurationSource(CommandLineConfigurationSource? commandLineConfigurationSource)
    {
        _commandLineConfigurationSource = commandLineConfigurationSource;
        _provider = new AppSettingsConfigurationProvider(
            commandLineConfigurationSource?.Args ?? Array.Empty<string>(),
            commandLineConfigurationSource?.SwitchMappings ?? new Dictionary<string, string>()
        );
    }

    public IEnumerable<string> Args => _commandLineConfigurationSource?.Args ?? Array.Empty<string>();

    public void Update(CommandContext commandContext, AppSettings appSettings)
    {
        _provider.Update(commandContext, appSettings);
    }

    public void Default()
    {
        _provider.Default();
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
