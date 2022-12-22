using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class ConsoleInterceptor : ICommandInterceptor
{
    private readonly AppSettingsConfigurationSource _appSettingsConfigurationSource;
    private readonly ConsoleResult _consoleResult;
    private readonly ICommandInterceptor? _commandInterceptor;

    public ConsoleInterceptor(
        AppSettingsConfigurationSource appSettingsConfigurationSource,
        ConsoleResult consoleResult,
        ICommandInterceptor? commandInterceptor
    )
    {
        _appSettingsConfigurationSource = appSettingsConfigurationSource;
        _consoleResult = consoleResult;
        _commandInterceptor = commandInterceptor;
    }

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        _commandInterceptor?.Intercept(context, settings);
        if (settings is not AppSettings appSettings)
        {
//            _consoleResult.ExitCode = CommandLineConstants.WaitCode;
            _appSettingsConfigurationSource.Default();
            return;
        }

        _appSettingsConfigurationSource.Update(context, appSettings);
    }
}
