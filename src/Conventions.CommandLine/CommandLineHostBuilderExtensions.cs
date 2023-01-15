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
public static class CommandAppHostBuilderExtensions
{
    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, CommandLineConvention @delegate)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

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
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

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
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

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
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

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
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        EnsureShouldRun(container);
        container.AppendDelegate(new CommandAppConvention((_, context) => context.SetDefaultCommand<TDefaultCommand>()));
        return container;
    }

    /// <summary>
    ///     Run the host as a commandline application and return the result
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static int Run(this IHostBuilder builder)
    {
        return RunAsync(builder).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Run the host as a commandline application and return the result
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> RunAsync(this IHostBuilder builder, CancellationToken cancellationToken = default)
    {
        using var host = builder.Build();
        var result = host.Services.GetService<ConsoleResult>();
        if (result == null)
        {
            host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(CommandAppHostBuilderExtensions)).LogWarning(
                "No commands have been configured, are you trying to run a console app? Try adding some commands for it to work correctly."
            );
        }

        await host.StartAsync(cancellationToken);
        await host.WaitForShutdownAsync(cancellationToken);
        return result?.ExitCode ?? 0;
    }

    private static void EnsureShouldRun(ConventionContextBuilder container)
    {
        if (!container.Properties.TryGetValue(typeof(ConsoleConvention), out _))
        {
            container.Properties.Add(typeof(ConsoleConvention), true);
        }
    }
}
