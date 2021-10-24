using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;

#pragma warning disable CA1001

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     Command Line Builder
///     Implements the <see cref="ICommandLineContext" />
/// </summary>
/// <seealso cref="ICommandLineContext" />
internal class CommandLineContext : ICommandLineContext
{
    private readonly CommandLineApplication<ApplicationState> _application;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandLineContext" /> class.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <exception cref="ArgumentNullException">diagnosticSource</exception>
    public CommandLineContext(CommandLineApplication<ApplicationState> application)
    {
        _application = application;
    }

    /// <summary>
    ///     Called when [run].
    /// </summary>
    /// <param name="onRunDelegate">The delegate.</param>
    /// <returns>ICommandLineBuilder.</returns>
    public ICommandLineContext OnRun(OnRunDelegate onRunDelegate)
    {
        _application.Model.OnRunDelegate = onRunDelegate;
        _application.Model.OnRunAsyncDelegate = null;
        _application.Model.OnRunAsyncCancellableDelegate = null;
        _application.Model.OnRunType = null;
        return this;
    }

    /// <summary>
    ///     Called when [run].
    /// </summary>
    /// <param name="onRunAsyncDelegate">The delegate.</param>
    /// <returns>ICommandLineContext.</returns>
    public ICommandLineContext OnRun(OnRunAsyncDelegate onRunAsyncDelegate)
    {
        _application.Model.OnRunDelegate = null;
        _application.Model.OnRunAsyncDelegate = onRunAsyncDelegate;
        _application.Model.OnRunAsyncCancellableDelegate = null;
        _application.Model.OnRunType = null;
        return this;
    }

    /// <summary>
    ///     Called when [run].
    /// </summary>
    /// <param name="onRunAsyncCancellableDelegate">The delegate.</param>
    /// <returns>ICommandLineContext.</returns>
    public ICommandLineContext OnRun(OnRunAsyncCancellableDelegate onRunAsyncCancellableDelegate)
    {
        _application.Model.OnRunDelegate = null;
        _application.Model.OnRunAsyncDelegate = null;
        _application.Model.OnRunAsyncCancellableDelegate = onRunAsyncCancellableDelegate;
        _application.Model.OnRunType = null;
        return this;
    }

    /// <summary>
    ///     Called when [run].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>ICommandLineContext.</returns>
    public ICommandLineContext OnRun<T>()
        where T : IDefaultCommand
    {
        _application.Model.OnRunDelegate = null;
        _application.Model.OnRunAsyncDelegate = null;
        _application.Model.OnRunAsyncCancellableDelegate = null;
        _application.Model.OnRunType = typeof(T);
        return this;
    }

    /// <summary>
    ///     Called when [run].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>ICommandLineContext.</returns>
    public ICommandLineContext OnRunAsync<T>()
        where T : IDefaultCommandAsync
    {
        _application.Model.OnRunDelegate = null;
        _application.Model.OnRunAsyncDelegate = null;
        _application.Model.OnRunAsyncCancellableDelegate = null;
        _application.Model.OnRunType = typeof(T);
        return this;
    }

    /// <summary>
    ///     Adds the command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action">The action.</param>
    /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
    /// <returns>ICommandLineContext.</returns>
    public CommandLineApplication<T> AddCommand<T>(
        Action<CommandLineApplication<T>>? action = null,
        bool throwOnUnexpectedArg = true
    )
        where T : class
    {
        if (action == null)
        {
            action = _ => { };
        }

        var commandAttribute = typeof(T).GetCustomAttribute<CommandAttribute>();

        if (commandAttribute == null)
        {
            throw new ArgumentException(
                $"You must give the command a name using {typeof(CommandAttribute).FullName} to add a command without a name."
            );
        }

        if (!( _application.Commands.Find(z => z.Name == commandAttribute.Name) is CommandLineApplication<T> command
            ))
        {
            command = _application.Command(commandAttribute.Name!, action);
        }
        else
        {
            action(command);
        }

        return command;
    }

    /// <summary>
    ///     Adds the command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <param name="action">The action.</param>
    /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
    /// <returns>ICommandLineContext.</returns>
    public CommandLineApplication<T> AddCommand<T>(
        string name,
        Action<CommandLineApplication<T>>? action = null,
        bool throwOnUnexpectedArg = true
    )
        where T : class
    {
        if (action == null)
        {
            action = _ => { };
        }

        if (!( _application.Commands.Find(z => z.Name == name) is CommandLineApplication<T> command ))
        {
            command = _application.Command(name, action);
        }
        else
        {
            action(command);
        }

        return command;
    }

    /// <summary>
    ///     Adds the command.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="action">The action.</param>
    /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
    /// <returns>ICommandLineContext.</returns>
    public CommandLineApplication AddCommand(
        string name,
        Action<CommandLineApplication>? action = null,
        bool throwOnUnexpectedArg = true
    )
    {
        if (action == null)
        {
            action = _ => { };
        }

        if (!( _application.Commands.Find(z => z.Name == name) is { } command ))
        {
            command = _application.Command(name, action);
        }
        else
        {
            action(command);
        }

        return command;
    }

    /// <summary>
    ///     Called when [parse].
    /// </summary>
    /// <param name="onParseDelegate">The delegate.</param>
    /// <returns>ICommandLineContext.</returns>
    public ICommandLineContext OnParse(OnParseDelegate onParseDelegate)
    {
        _application.Model.OnParseDelegates.Add(onParseDelegate);
        return this;
    }

    /// <summary>
    ///     Gets the command line application conventions.
    /// </summary>
    /// <value>The command line application conventions.</value>
    public IConventionBuilder CommandLineApplicationConventions => _application.Conventions;
}
