using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Rocket.Surgery.Extensions.CommandLine
{
    /// <summary>
    /// ICommandLine
    /// </summary>
    public interface ICommandLine
    {
        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>The application.</value>
        CommandLineApplication Application { get; }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>ICommandLineExecutor.</returns>
        ICommandLineExecutor Parse(params string[] args);

        /// <summary>
        /// Executes the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        int Execute(IServiceProvider serviceProvider, params string[] args);

        /// <summary>
        /// Executes the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        Task<int> ExecuteAsync(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken,
            params string[] args
        );
    }
}