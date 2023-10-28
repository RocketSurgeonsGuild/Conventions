using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class ConsoleWorker : IHostedService
{
    private readonly ICommandApp _commandApp;
    private readonly IHostApplicationLifetime _hostLifetime;
    private readonly ConsoleResult _consoleResult;
    private readonly AppSettingsConfigurationSource _appSettingsConfigurationSource;
    private readonly ILogger<ConsoleWorker> _logger;

    public ConsoleWorker(
        ILogger<ConsoleWorker> logger,
        ICommandApp commandApp,
        IHostApplicationLifetime hostLifetime,
        ConsoleResult consoleResult,
        AppSettingsConfigurationSource appSettingsConfigurationSource
    )
    {
        _logger = logger;
        _commandApp = commandApp;
        _hostLifetime = hostLifetime;
        _consoleResult = consoleResult;
        _appSettingsConfigurationSource = appSettingsConfigurationSource;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_consoleResult.ExitCode.HasValue)
        {
            _hostLifetime.ApplicationStarted.Register(OnStarted);
        }

        return Task.CompletedTask;
    }

    private async void OnStarted()
    {
        try
        {
            _consoleResult.ExitCode = await _commandApp.RunAsync(_appSettingsConfigurationSource.Args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            _consoleResult.ExitCode = 1;
        }
        finally
        {
            if (_consoleResult.ExitCode != CommandLineConstants.WaitCode)
            {
                _hostLifetime.StopApplication();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Environment.ExitCode = _consoleResult.ExitCode switch
        {
            CommandLineConstants.WaitCode => 0,
            { } i                         => i,
            null                          => 0,
        };
        return Task.CompletedTask;
    }
}
