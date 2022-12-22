using System.Collections;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     Constants for use with the commandline interface
/// </summary>
public static class CommandLineConstants
{
    /// <summary>
    ///     The code that tells the hosted service to not stop the application.
    /// </summary>
    public const int WaitCode = -1567;
}

internal class ConsoleResult
{
    public int? ExitCode { get; set; }
}

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

internal class DefaultCommand : Command<AppSettings>
{
    private readonly IAnsiConsole _console;

    public DefaultCommand(IAnsiConsole console)
    {
        _console = console;
    }

    public override int Execute(CommandContext context, AppSettings settings)
    {
        // Got at least one argument?
        var versionArgument = context.Remaining.Parsed.Contains("version") || context.Remaining.Parsed.Contains("v");
        if (versionArgument)
        {
            _console.WriteLine(
                Assembly.GetEntryAssembly()?
                   .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                   .InformationalVersion ?? "?"
            );
            return 0;
        }

        return CommandLineConstants.WaitCode;
    }
}

[ExportConvention]
public class ConsoleConvention : IServiceConvention, IHostingConvention
{
    private bool _isHostBuilder;

    public void Register(IConventionContext context, IHostBuilder builder)
    {
        _isHostBuilder = true;
        builder.ConfigureAppConfiguration(
            (builderContext, configurationBuilder) =>
            {
                var sourcesToRemove = configurationBuilder.Sources.OfType<CommandLineConfigurationSource>().ToList();

                var commandLineConfigurationSource = sourcesToRemove.FirstOrDefault();
                var appSettings = new AppSettingsConfigurationSource(sourcesToRemove.FirstOrDefault());
                context.Set(appSettings);
                var index = configurationBuilder.Sources.IndexOf(commandLineConfigurationSource);
                if (index > -1)
                {
                    configurationBuilder.Sources.Insert(index, appSettings);
                }

                foreach (var source in sourcesToRemove)
                {
                    configurationBuilder.Sources.Remove(source);
                }
            }
        );
    }

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        // We just bail out, the environment is not correct!
        if (!_isHostBuilder)
        {
            return;
        }

        var registry = new ConventionTypeRegistrar();
        var command = new CommandApp(registry);
        var consoleResult = new ConsoleResult();

        services.AddSingleton<IAnsiConsole>(_ => (IAnsiConsole)registry.GetService(typeof(IAnsiConsole))!);
        services.AddSingleton<IRemainingArguments>(_ => (IRemainingArguments)registry.GetService(typeof(IRemainingArguments))!);

        foreach (var item in context.Conventions.Get<ICommandAppConvention, CommandAppConvention>())
        {
            if (item is ICommandAppConvention convention)
            {
                convention.Register(context, command);
            }
            else if (item is CommandAppConvention @delegate)
            {
                @delegate(context, command);
            }
        }

        command.Configure(
            configurator =>
            {
                var interceptor = new ConsoleInterceptor(
                    context.Get<AppSettingsConfigurationSource>()!,
                    consoleResult,
                    configurator.Settings.Interceptor
                );
                configurator.SetInterceptor(interceptor);

                foreach (var item in context.Conventions.Get<ICommandLineConvention, CommandLineConvention>())
                {
                    if (item is ICommandLineConvention convention)
                    {
                        convention.Register(context, configurator);
                    }
                    else if (item is CommandLineConvention @delegate)
                    {
                        @delegate(context, configurator);
                    }
                }

                var defaultCommandProperty = configurator.GetType().GetProperty("DefaultCommand");
                if (defaultCommandProperty?.GetValue(configurator) == null)
                {
                    command.SetDefaultCommand<DefaultCommand>();
                }
            }
        );
        services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

        services.AddSingleton(consoleResult);
        services.AddHostedService<ConsoleWorker>();
        services.AddSingleton(context.Get<AppSettingsConfigurationSource>()!);
        services.AddSingleton<ICommandApp>(
            provider =>
            {
                registry.SetServiceProvider(provider);
                return command;
            }
        );
    }
}

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

/// <summary>
///     In-memory implementation of <see cref="IConfigurationProvider" />
/// </summary>
internal class AppSettingsConfigurationProvider : CommandLineConfigurationProvider, IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    ///     Initialize a new instance from the source.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="switchMappings"></param>
    public AppSettingsConfigurationProvider(
        IEnumerable<string> args,
        IDictionary<string, string>? switchMappings = null
    ) : base(args, switchMappings)
    {
    }

    public void Update(CommandContext commandContext, AppSettings appSettings)
    {
        Data = new Dictionary<string, string>
               {
                   [nameof(AppSettings.Trace)] = appSettings.Trace.ToString(CultureInfo.InvariantCulture),
                   [nameof(AppSettings.Verbose)] = appSettings.Verbose.ToString(CultureInfo.InvariantCulture),
                   [nameof(AppSettings.LogLevel)] = appSettings.LogLevel.HasValue ? appSettings.LogLevel.Value.ToString() : ""
               }
              .Select(z => new KeyValuePair<string, string>($"{nameof(AppSettings)}:{z.Key}", z.Value))
              .Concat(commandContext.Remaining.Parsed.Select(z => new KeyValuePair<string, string>(z.Key, z.Last()!)))
              .ToDictionary(z => z.Key, z => z.Value);
        if (appSettings.LogLevel.HasValue)
        {
            Data.Add("Logging:LogLevel:Default", appSettings.LogLevel.Value.ToString());
            Data.Add("Logging:Debug:LogLevel:Default", appSettings.LogLevel.Value.ToString());
            Data.Add("Logging:Console:LogLevel:Default", appSettings.LogLevel.Value.ToString());
        }

        OnReload();
    }

    public void Default()
    {
        base.Load();
        OnReload();
    }

    public override void Load()
    {
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return Data.GetEnumerator();
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class ConsoleWorker : IHostedService
{
    private readonly ICommandApp _commandApp;
    private readonly IHostApplicationLifetime _hostLifetime;
    private readonly ConsoleResult _consoleResult;
    private readonly AppSettingsConfigurationSource _appSettingsConfigurationSource;
    private readonly IOptions<ConsoleLifetimeOptions> _options;
    private readonly ILogger<ConsoleWorker> _logger;

    public ConsoleWorker(
        ILogger<ConsoleWorker> logger,
        ICommandApp commandApp,
        IHostApplicationLifetime hostLifetime,
        ConsoleResult consoleResult,
        AppSettingsConfigurationSource appSettingsConfigurationSource,
        IOptions<ConsoleLifetimeOptions> options
    )
    {
        _logger = logger;
        _commandApp = commandApp;
        _hostLifetime = hostLifetime;
        _consoleResult = consoleResult;
        _appSettingsConfigurationSource = appSettingsConfigurationSource;
        _options = options;
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
