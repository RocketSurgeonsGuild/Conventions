using System;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Shell;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class CommandLineHostedService.
    /// Implements the <see cref="IHostedService" />
    /// </summary>
    /// <seealso cref="IHostedService" />
    internal class ShellHostedService : IHostedService
    {
        private readonly IHost _host;
        private readonly InvocationContext _context;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly CommandLineResult _result;
        private readonly ILogger<ShellHostedService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellHostedService" /> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="context"></param>
        /// <param name="lifetime">The lifetime.</param>
        /// <param name="commandLineResult">The command line result.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">
        /// executor
        /// or
        /// serviceProvider
        /// or
        /// lifetime
        /// or
        /// commandLineResult
        /// </exception>
        public ShellHostedService(
            IHost host,
            InvocationContext context,
            IHostApplicationLifetime lifetime,
            CommandLineResult commandLineResult,
            ILogger<ShellHostedService> logger
        )
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            _result = commandLineResult ?? throw new ArgumentNullException(nameof(commandLineResult));
            _logger = logger;
            _context.BindingContext.AddService(typeof(IHost), _ => _host);
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>Task.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(
                async () =>
                {
                    try
                    {
                        _result.Value = await new ShellInvocationPipeline(_context).InvokeAsync().ConfigureAwait(false);
                    }
#pragma warning disable CA1031
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Command failed to execute");
                        _result.Value = -1;
                    }
#pragma warning restore CA1031
                    finally
                    {
                        _lifetime.StopApplication();
                    }
                }
            );
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}