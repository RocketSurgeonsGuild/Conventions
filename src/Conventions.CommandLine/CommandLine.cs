using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     CommandLine.
///     Implements the <see cref="ICommandLine" />
/// </summary>
/// <seealso cref="ICommandLine" />
internal class CommandLine : ICommandLine
{
    private readonly ILogger _logger;

    private readonly Action<ILogger, IEnumerable<string>, Exception?> _logParsingStart =
        LoggerMessage.Define<IEnumerable<string>>(LogLevel.Trace, new EventId(1337), "Parsing {@Args}");

    private readonly Action<ILogger, IEnumerable<string>, Exception?> _logExecutingStart =
        LoggerMessage.Define<IEnumerable<string>>(LogLevel.Trace, new EventId(1337), "Executing {@Args}");

    private readonly Action<ILogger, object, object, Exception?> _logSelectedCommand =
        LoggerMessage.Define<object, object>(LogLevel.Trace, new EventId(1337), "Selected Command {@Command} {@State}");

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandLine" /> class.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="logger">The logger.</param>
    internal CommandLine(CommandLineApplication application, ILogger logger)
    {
        _logger = logger;
        Application = application;
    }

    /// <summary>
    ///     Gets the application.
    /// </summary>
    /// <value>The application.</value>
    public CommandLineApplication Application { get; }

    /// <summary>
    ///     Parses the specified arguments.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>ICommandLineExecutor.</returns>
    public ICommandLineExecutor Parse(params string[] args)
    {
        _logParsingStart(_logger, args.AsEnumerable(), null);

        var result = Application.Parse(args);

        var parent = result.SelectedCommand;
        while (parent.Parent != null)
        {
            parent = parent.Parent;
        }

        ApplicationState? myState = null;
        if (parent is IModelAccessor ma && ma.GetModel() is ApplicationState state)
        {
            state.IsDefaultCommand = result.SelectedCommand is IModelAccessor m &&
                                     m.GetModelType() == typeof(ApplicationState) && !result.SelectedCommand.IsShowingInformation;

            foreach (var d in state.OnParseDelegates)
            {
                d(state);
            }

            myState = state;
        }

        if (myState != null)
        {
            _logSelectedCommand(
                _logger,
                new
                {
                    result.SelectedCommand.FullName,
                    result.SelectedCommand.Name,
                    result.SelectedCommand.Description
                },
                new
                {
                    myState.RemainingArguments,
                    myState.Trace,
                    myState.Verbose,
                    myState.Log.Level
                },
                null
            );
        }

        return new CommandLineExecutor(result.SelectedCommand, myState!, Application, args);
    }

    /// <summary>
    ///     Executes the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Int32.</returns>
    public int Execute(IServiceProvider serviceProvider, params string[] args)
    {
        _logExecutingStart(_logger, args.AsEnumerable(), null);
        return Parse(args).Execute(serviceProvider);
    }

    /// <summary>
    ///     Executes the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Int32.</returns>
    public Task<int> ExecuteAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken,
        params string[] args
    )
    {
        _logExecutingStart(_logger, args.AsEnumerable(), null);
        return Parse(args).ExecuteAsync(serviceProvider, cancellationToken);
    }
}
