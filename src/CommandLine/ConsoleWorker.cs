using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

internal class ConsoleWorker
(
    ILogger<ConsoleWorker> logger,
    ICommandApp commandApp,
    IHostApplicationLifetime hostLifetime,
    ConsoleResult consoleResult,
    AppSettingsConfigurationSource appSettingsConfigurationSource)
    : IHostedLifecycleService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        try
        {
            consoleResult.ExitCode = await commandApp.RunAsync(appSettingsConfigurationSource.Args, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
            consoleResult.ExitCode = 1;
        }
        finally
        {
            if (consoleResult.ExitCode != CommandLineConstants.WaitCode) _ = Task.Run(hostLifetime.StopApplication, cancellationToken);
        }
    }

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        Environment.ExitCode = consoleResult switch { { ExitCode: CommandLineConstants.WaitCode, } => 0, { ExitCode: { } i, } => i, { ExitCode: null, } => 0, _ => throw new NotImplementedException(), };
        return Task.CompletedTask;
    }
}
