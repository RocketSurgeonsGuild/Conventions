﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.CommandLine
{
    /// <summary>
    /// Logging Builder
    /// Implements the <see cref="ICommandLineBuilder" />
    /// Implements the <see cref="ICommandLineConventionContext" />
    /// </summary>
    /// <seealso cref="ICommandLineBuilder" />
    /// <seealso cref="ICommandLineConventionContext" />
    public class CommandLineBuilder : ConventionBuilder<ICommandLineBuilder, ICommandLineConvention, CommandLineConventionDelegate>, ICommandLineBuilder, ICommandLineConventionContext
    {
        private readonly CommandLineApplication<ApplicationState> _application;

        private readonly List<(Type serviceType, object serviceValue)> _services =
            new List<(Type serviceType, object serviceValue)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineBuilder"/> class.
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
            IDictionary<object, object> properties) : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            _application = new CommandLineApplication<ApplicationState>()
            {
                ThrowOnUnexpectedArgument = false
            };
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
        }

        /// <summary>
        /// Gets the command line application conventions.
        /// </summary>
        /// <value>The command line application conventions.</value>
        public IConventionBuilder CommandLineApplicationConventions => _application.Conventions;

        ICommandLineConventionContext ICommandLineConventionContext.OnParse(OnParseDelegate @delegate)
        {
            OnParse(@delegate);
            return this;
        }

        ICommandLineConventionContext ICommandLineConventionContext.OnRun(OnRunDelegate @delegate)
        {
            OnRun(@delegate);
            return this;
        }

        ICommandLineConventionContext ICommandLineConventionContext.OnRun<T>()
        {
            OnRun<T>();
            return this;
        }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnRun(OnRunDelegate @delegate)
        {
            _application.Model.OnRunDelegate = @delegate;
            _application.Model.OnRunType = null;
            return this;
        }

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnRun<T>() where T : IDefaultCommand
        {
            _application.Model.OnRunType = typeof(T);
            _application.Model.OnRunDelegate = null;
            return this;
        }

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
        /// <returns>ICommandLineBuilder.</returns>
        public CommandLineApplication<T> AddCommand<T>(Action<CommandLineApplication<T>> action = null, bool throwOnUnexpectedArg = true)
            where T : class
        {
            if (action == null)
                action = application => { };

            var commandAttribute = (typeof(T)).GetCustomAttribute<CommandAttribute>();

            if (commandAttribute == null)
            {
                throw new ArgumentException($"You must give the command a name using {typeof(CommandAttribute).FullName} to add a command without a name.");
            }

            if (!(_application.Commands.Find(z => z.Name == commandAttribute.Name) is CommandLineApplication<T> command))
            {
                command = _application.Command(commandAttribute.Name, action, throwOnUnexpectedArg);
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
        public CommandLineApplication<T> AddCommand<T>(string name, Action<CommandLineApplication<T>> action = null, bool throwOnUnexpectedArg = true)
            where T : class
        {
            if (action == null)
                action = application => { };

            if (!(_application.Commands.Find(z => z.Name == name) is CommandLineApplication<T> command))
            {
                command = _application.Command(name, action, throwOnUnexpectedArg);
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
        public CommandLineApplication AddCommand(string name, Action<CommandLineApplication> action = null, bool throwOnUnexpectedArg = true)
        {
            if (action == null)
                action = application => { };

            if (!(_application.Commands.Find(z => z.Name == name) is CommandLineApplication command))
            {
                command = _application.Command(name, action, throwOnUnexpectedArg);
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
        /// <param name="delegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        public ICommandLineBuilder OnParse(OnParseDelegate @delegate)
        {
            _application.Model.OnParseDelegates.Add(@delegate);
            return this;
        }

        /// <summary>
        /// Builds the specified entry assembly.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        /// <returns>ICommandLine.</returns>
        public ICommandLine Build(Assembly entryAssembly = null)
        {
            if (entryAssembly is null) entryAssembly = Assembly.GetEntryAssembly();

            new ConventionComposer(Scanner)
                .Register(
                    this,
                    typeof(ICommandLineConvention),
                    typeof(CommandLineConventionDelegate)
                );

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
                .AddConvention(new ActivatorUtilitiesConvention(
                    new CommandLineServiceProvider(_application)
                ));

            return new CommandLine(this, _application, Logger);
        }
    }
}
