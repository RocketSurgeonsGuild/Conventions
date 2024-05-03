using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.CommandLine;
using Spectre.Console.Cli;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
[PublicAPI]
public static partial class CommandAppHostBuilderExtensions
{
    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, CommandLineConvention @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, CommandLineAsyncConvention @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, Action<IConfigurator> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandLineConvention((_, context) => @delegate(context)), priority);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, Func<IConfigurator, ValueTask> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandLineAsyncConvention((_, context, _) => @delegate(context)), priority);
        return container;
    }


    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(
        this ConventionContextBuilder container,
        Func<IConfigurator, CancellationToken, ValueTask> @delegate, int priority = 0
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandLineAsyncConvention((_, context, ct) => @delegate(context, ct)), priority);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandApp(this ConventionContextBuilder container, CommandAppConvention @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandApp(this ConventionContextBuilder container, CommandAppAsyncConvention @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandApp(this ConventionContextBuilder container, Action<CommandApp> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandAppConvention((_, context) => @delegate(context)), priority);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandApp(this ConventionContextBuilder container, Func<CommandApp, ValueTask> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandAppAsyncConvention((_, context, _) => @delegate(context)), priority);
        return container;
    }


    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandApp(
        this ConventionContextBuilder container,
        Func<CommandApp, CancellationToken, ValueTask> @delegate, int priority = 0
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandAppAsyncConvention((_, context, ct) => @delegate(context, ct)), priority);
        return container;
    }

    /// <summary>
    ///     Configure the default command
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder SetDefaultCommand<TDefaultCommand>(this ConventionContextBuilder container, int priority = 0)
        where TDefaultCommand : class, ICommand
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandAppConvention((_, context) => context.SetDefaultCommand<TDefaultCommand>()), priority);
        return container;
    }

    /// <summary>
    ///     Run the host as a commandline application and return the result
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> RunConsoleAppAsync(this ValueTask<IHost> host, CancellationToken cancellationToken = default)
    {
        return await RunConsoleAppAsync(await host, cancellationToken);
    }

    /// <summary>
    ///     Run the host as a commandline application and return the result
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> RunConsoleAppAsync(this Task<IHost> host, CancellationToken cancellationToken = default)
    {
        return await RunConsoleAppAsync(await host, cancellationToken);
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
}
