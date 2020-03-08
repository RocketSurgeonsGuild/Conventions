using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// CommandLineExecutor.
    /// Implements the <see cref="ICommandLineExecutor" />
    /// </summary>
    /// <seealso cref="ICommandLineExecutor" />
    internal class CommandLineExecutor : ICommandLineExecutor
    {
        private readonly CommandLineApplication _rootApplication;
        private readonly string[] _args;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineExecutor" /> class.
        /// </summary>
        /// <param name="application">The selected application.</param>
        /// <param name="applicationState">State of the application.</param>
        /// <param name="rootApplication">The root application.</param>
        /// <param name="args">The arguments.</param>
        public CommandLineExecutor(
            CommandLineApplication application,
            IApplicationState applicationState,
            CommandLineApplication rootApplication,
            string[] args
        )
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
            ApplicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
            IsDefaultCommand = Application is IModelAccessor m && m.GetModelType() == typeof(ApplicationState) &&
                !Application.IsShowingInformation;
            _rootApplication = rootApplication;
            _args = args;
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>The application.</value>
        public CommandLineApplication Application { get; }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>The state of the application.</value>
        public IApplicationState ApplicationState { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is default command.
        /// </summary>
        /// <value><c>true</c> if this instance is default command; otherwise, <c>false</c>.</value>
        public bool IsDefaultCommand { get; }

        /// <summary>
        /// Executes the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public int Execute(IServiceProvider serviceProvider)
            => ExecuteAsync(serviceProvider, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Executes the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<int> ExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            if (Application.IsShowingInformation)
            {
                return Task.FromResult(0);
            }

            var validationResult = Application.GetValidationResult();
            if (validationResult != ValidationResult.Success)
            {
                return Task.FromResult(Application.ValidationErrorHandler(validationResult));
            }

            if (Application is IModelAccessor ma && ma.GetModel() is ApplicationState state)
            {
                if (state.OnRunType != null)
                {
                    var instance = ActivatorUtilities.CreateInstance(serviceProvider, state.OnRunType);
                    if (instance is IDefaultCommand defaultCommand)
                    {
                        return Task.FromResult(defaultCommand.Run(state));
                    }

                    if (instance is IDefaultCommandAsync defaultCommandAsync)
                    {
                        return defaultCommandAsync.Run(state, cancellationToken);
                    }
                }

                if (state.OnRunDelegate != null)
                {
                    return Task.FromResult(state.OnRunDelegate(state));
                }

                if (state.OnRunAsyncDelegate != null)
                {
                    return state.OnRunAsyncDelegate(state);
                }

                if (state.OnRunAsyncCancellableDelegate != null)
                {
                    return state.OnRunAsyncCancellableDelegate(state, cancellationToken);
                }

                return Task.FromResult(int.MinValue);
            }

            ActivatorUtilitiesConvention.AdditionalServicesProperty.SetValue(Application, serviceProvider);
            return _rootApplication.ExecuteAsync(_args, cancellationToken);
        }
    }
}