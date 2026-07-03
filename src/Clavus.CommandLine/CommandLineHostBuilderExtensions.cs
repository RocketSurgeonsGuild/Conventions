using Clavus.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

// ReSharper disable once CheckNamespace
namespace Clavus;

/// <summary>
///     Helper method for working with <see cref="ClavusContextBuilder" />
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
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandLine(
        this ClavusContextBuilder container,
        CommandLineConvention @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandLine(
        this ClavusContextBuilder container,
        CommandLineAsyncConvention @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandLine(
        this ClavusContextBuilder container,
        Action<IConfigurator> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandLineConvention((_, context) => @delegate(context)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandLine(
        this ClavusContextBuilder container,
        Func<IConfigurator, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandLineAsyncConvention((_, context, _) => @delegate(context)), priority, category);
        return container;
    }


    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandLine(
        this ClavusContextBuilder container,
        Func<IConfigurator, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandLineAsyncConvention((_, context, ct) => @delegate(context, ct)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandApp(
        this ClavusContextBuilder container,
        CommandAppConvention @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandApp(
        this ClavusContextBuilder container,
        CommandAppAsyncConvention @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandApp(
        this ClavusContextBuilder container,
        Action<CommandApp> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandAppConvention((_, context) => @delegate(context)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandApp(
        this ClavusContextBuilder container,
        Func<CommandApp, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandAppAsyncConvention((_, context, _) => @delegate(context)), priority, category);
        return container;
    }


    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureCommandApp(
        this ClavusContextBuilder container,
        Func<CommandApp, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandAppAsyncConvention((_, context, ct) => @delegate(context, ct)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the default command
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder SetDefaultCommand<TDefaultCommand>(
        this ClavusContextBuilder container,
        int priority = 0,
        ClavusCategory? category = null
    )
        where TDefaultCommand : class, ICommand
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(new CommandAppConvention((_, context) => context.SetDefaultCommand<TDefaultCommand>()), priority, category);
        return container;
    }

    /// <summary>
    ///     Run the host as a commandline application and return the result
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> RunConsoleAppAsync(this ValueTask<IHost> host, CancellationToken cancellationToken = default) => await RunConsoleAppAsync(await host, cancellationToken);

    /// <summary>
    ///     Run the host as a commandline application and return the result
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> RunConsoleAppAsync(this Task<IHost> host, CancellationToken cancellationToken = default) => await RunConsoleAppAsync(await host, cancellationToken);

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
        return result.ExitCode ?? Environment.ExitCode;
    }

    [LoggerMessage(
        Message = "No commands have been configured, are you trying to run a console app? Try adding some commands for it to work correctly.",
        Level = LogLevel.Warning
    )]
    static partial void LogWarning(ILogger logger);
}
