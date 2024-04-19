using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class ConsoleInterceptor
(
    AppSettingsConfigurationSource appSettingsConfigurationSource,
    ICommandInterceptor? commandInterceptor
)
    : ICommandInterceptor
{
    public void Intercept(CommandContext context, CommandSettings settings)
    {
        commandInterceptor?.Intercept(context, settings);
        if (settings is not AppSettings appSettings)
            //            _consoleResult.ExitCode = CommandLineConstants.WaitCode;
            return;

        appSettingsConfigurationSource.Update(context, appSettings);
    }
}