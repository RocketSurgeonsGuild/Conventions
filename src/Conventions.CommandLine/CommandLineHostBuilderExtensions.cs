using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.CommandLine;
using Spectre.Console.Cli;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
public static partial class CommandAppHostBuilderExtensions
{
    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, CommandLineConvention @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        EnsureShouldRun(container);
        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, Action<IConfigurator> @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        EnsureShouldRun(container);
        container.AppendDelegate(new CommandLineConvention((_, context) => @delegate(context)));
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandApp(this ConventionContextBuilder container, CommandAppConvention @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        EnsureShouldRun(container);
        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandApp(this ConventionContextBuilder container, Action<CommandApp> @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        EnsureShouldRun(container);
        container.AppendDelegate(new CommandAppConvention((_, context) => @delegate(context)));
        return container;
    }

    /// <summary>
    ///     Configure the default command
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder SetDefaultCommand<TDefaultCommand>(this ConventionContextBuilder container)
        where TDefaultCommand : class, ICommand
    {
        ArgumentNullException.ThrowIfNull(container);

        EnsureShouldRun(container);
        container.AppendDelegate(new CommandAppConvention((_, context) => context.SetDefaultCommand<TDefaultCommand>()));
        return container;
    }

    /// <summary>
    ///     Run the host as a commandline application and return the result
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static int RunConsoleApp(this IHost host)
    {
        return RunConsoleAppAsync(host).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Run the host as a commandline application and return the result
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> RunConsoleAppAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        var result = host.Services.GetService<ConsoleResult>();
        if (result == null) LogWarning(host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(CommandAppHostBuilderExtensions)));

        await host.StartAsync(cancellationToken);
        await host.WaitForShutdownAsync(cancellationToken);
        return result?.ExitCode ?? Environment.ExitCode;
    }

    [LoggerMessage(
        Message = "No commands have been configured, are you trying to run a console app? Try adding some commands for it to work correctly.",
        Level = LogLevel.Warning
    )]
    static partial void LogWarning(ILogger logger);

    private static void EnsureShouldRun(ConventionContextBuilder container)
    {
        if (!container.Properties.TryGetValue(typeof(ConsoleConvention), out _)) container.Properties.Add(typeof(ConsoleConvention), true);
    }
}
