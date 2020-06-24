using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// CommandLine.
    /// Implements the <see cref="ICommandLine" />
    /// </summary>
    /// <seealso cref="ICommandLine" />
    internal class CommandLine : ICommandLine
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="logger">The logger.</param>
        internal CommandLine(CommandLineApplication application, ILogger logger)
        {
            _logger = logger;
            Application = application;
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>The application.</value>
        public CommandLineApplication Application { get; }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>ICommandLineExecutor.</returns>
        public ICommandLineExecutor Parse(params string[] args)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Parsing {@Args}", args.AsEnumerable());
            }

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

                if (state.OnParseDelegates == null)
                {
                    state.OnParseDelegates = new List<OnParseDelegate>();
                }

                foreach (var d in state.OnParseDelegates)
                {
                    d(state);
                }

                myState = state;
            }

            if (myState != null && _logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(
                    "Selected Command {@Command} {@State}",
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
                    }
                );
            }

            return new CommandLineExecutor(result.SelectedCommand, myState!, Application, args);
        }

        /// <summary>
        /// Executes the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        public int Execute(IServiceProvider serviceProvider, params string[] args)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Executing {@Args}", args.AsEnumerable());
            }

            return Parse(args).Execute(serviceProvider);
        }

        /// <summary>
        /// Executes the specified service provider.
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
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Executing {@Args}", args.AsEnumerable());
            }

            return Parse(args).ExecuteAsync(serviceProvider, cancellationToken);
        }
    }
}