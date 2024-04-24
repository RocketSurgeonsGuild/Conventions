using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

internal class ConsoleInterceptor(AppSettingsConfigurationSource appSettingsConfigurationSource)
    : ICommandInterceptor
{
    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is not AppSettings appSettings) return;
        appSettingsConfigurationSource.Update(context, appSettings);
    }

    public void InterceptResult(CommandContext context, CommandSettings settings, ref int result) { }
}