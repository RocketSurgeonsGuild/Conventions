using System;
using System.Collections.Generic;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

#pragma warning disable CA1001

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// Logging Builder
    /// Implements the <see cref="ICommandLineBuilder" />
    /// Implements the <see cref="ICommandLineConventionContext" />
    /// </summary>
    /// <seealso cref="ICommandLineBuilder" />
    /// <seealso cref="ICommandLineConventionContext" />
    public class CommandLineBuilder :
        ConventionBuilder<ICommandLineBuilder, ICommandLineConvention, CommandLineConventionDelegate>,
        ICommandLineBuilder,
        ICommandLineConventionContext
    {
        private readonly CommandLineApplication<ApplicationState> _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineBuilder" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">diagnosticSource</exception>
        public CommandLineBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        ) : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            _application = new CommandLineApplication<ApplicationState>
            {
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
        }

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunDelegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnRun(OnRunDelegate onRunDelegate)
        {
            _application.Model.OnRunDelegate = onRunDelegate;
            _application.Model.OnRunAsyncDelegate = null;
            _application.Model.OnRunAsyncCancellableDelegate = null;
            _application.Model.OnRunType = null;
            return this;
        }

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunAsyncDelegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnRun(OnRunAsyncDelegate onRunAsyncDelegate)
        {
            _application.Model.OnRunDelegate = null;
            _application.Model.OnRunAsyncDelegate = onRunAsyncDelegate;
            _application.Model.OnRunAsyncCancellableDelegate = null;
            _application.Model.OnRunType = null;
            return this;
        }

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunAsyncCancellableDelegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnRun(OnRunAsyncCancellableDelegate onRunAsyncCancellableDelegate)
        {
            _application.Model.OnRunDelegate = null;
            _application.Model.OnRunAsyncDelegate = null;
            _application.Model.OnRunAsyncCancellableDelegate = onRunAsyncCancellableDelegate;
            _application.Model.OnRunType = null;
            return this;
        }

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnRun<T>()
            where T : IDefaultCommand
        {
            _application.Model.OnRunDelegate = null;
            _application.Model.OnRunAsyncDelegate = null;
            _application.Model.OnRunAsyncCancellableDelegate = null;
            _application.Model.OnRunType = typeof(T);
            return this;
        }

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnRunAsync<T>()
            where T : IDefaultCommandAsync
        {
            _application.Model.OnRunDelegate = null;
            _application.Model.OnRunAsyncDelegate = null;
            _application.Model.OnRunAsyncCancellableDelegate = null;
            _application.Model.OnRunType = typeof(T);
            return this;
        }

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
        /// <returns>ICommandLineBuilder.</returns>
        public CommandLineApplication<T> AddCommand<T>(
            Action<CommandLineApplication<T>>? action = null,
            bool throwOnUnexpectedArg = true
        )
            where T : class
        {
            if (action == null)
            {
                action = application => { };
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
        /// Adds the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
        /// <returns>ICommandLineBuilder.</returns>
        public CommandLineApplication<T> AddCommand<T>(
            string name,
            Action<CommandLineApplication<T>>? action = null,
            bool throwOnUnexpectedArg = true
        )
            where T : class
        {
            if (action == null)
            {
                action = application => { };
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
        /// Adds the command.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
        /// <returns>ICommandLineBuilder.</returns>
        public CommandLineApplication AddCommand(
            string name,
            Action<CommandLineApplication>? action = null,
            bool throwOnUnexpectedArg = true
        )
        {
            if (action == null)
            {
                action = application => { };
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
        /// Called when [parse].
        /// </summary>
        /// <param name="onParseDelegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnParse(OnParseDelegate onParseDelegate)
        {
            _application.Model.OnParseDelegates.Add(onParseDelegate);
            return this;
        }

        /// <summary>
        /// Builds the specified entry assembly.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        /// <returns>ICommandLine.</returns>
        public ICommandLine Build(Assembly? entryAssembly = null)
        {
            if (entryAssembly is null)
            {
                entryAssembly = Assembly.GetEntryAssembly()!;
            }

            Composer.Register(Scanner, this, typeof(ICommandLineConvention), typeof(CommandLineConventionDelegate));

            _application.Conventions
               .UseAttributes()
               .SetAppNameFromEntryAssembly()
               .SetRemainingArgsPropertyOnModel()
               .SetSubcommandPropertyOnModel()
               .SetParentPropertyOnModel()
                //.UseOnExecuteMethodFromModel()
               .UseOnValidateMethodFromModel()
               .UseOnValidationErrorMethodFromModel()
               .AddConvention(new DefaultHelpOptionConvention())
               .AddConvention(new VersionConvention(entryAssembly))
               .AddConvention(
                    new ActivatorUtilitiesConvention(
                        new CommandLineServiceProvider(_application)
                    )
                );

            return new CommandLine(_application, Logger);
        }

        /// <summary>
        /// Gets the command line application conventions.
        /// </summary>
        /// <value>The command line application conventions.</value>
        public IConventionBuilder CommandLineApplicationConventions => _application.Conventions;

        ICommandLineConventionContext ICommandLineConventionContext.OnParse(OnParseDelegate onParseDelegate)
        {
            OnParse(onParseDelegate);
            return this;
        }

        ICommandLineConventionContext ICommandLineConventionContext.OnRun(OnRunDelegate onRunDelegate)
        {
            OnRun(onRunDelegate);
            return this;
        }

        ICommandLineConventionContext ICommandLineConventionContext.OnRun(OnRunAsyncDelegate onRunAsyncDelegate)
        {
            OnRun(onRunAsyncDelegate);
            return this;
        }

        ICommandLineConventionContext ICommandLineConventionContext.OnRun(
            OnRunAsyncCancellableDelegate onRunAsyncCancellableDelegate
        )
        {
            OnRun(onRunAsyncCancellableDelegate);
            return this;
        }

        ICommandLineConventionContext ICommandLineConventionContext.OnRun<T>()
        {
            OnRun<T>();
            return this;
        }

        ICommandLineConventionContext ICommandLineConventionContext.OnRunAsync<T>()
        {
            OnRunAsync<T>();
            return this;
        }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }
    }
}