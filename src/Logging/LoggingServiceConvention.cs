using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Logging;

[assembly:Convention(typeof(LoggingServiceConvention))]

namespace Rocket.Surgery.Extensions.Logging
{
    /// <summary>
    /// LoggingServiceConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// Implements the <see cref="ILoggingConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="ILoggingConvention" />
    public class LoggingServiceConvention : IServiceConvention, ILoggingConvention
    {
        private readonly RocketLoggingOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingServiceConvention"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public LoggingServiceConvention(RocketLoggingOptions? options = null)
        {
            _options = options ?? new RocketLoggingOptions();
        }

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(IServiceConventionContext context)
        {
            var loggingBuilder = new LoggingBuilder(
                context.Get<IConventionScanner>(),
                context.AssemblyProvider,
                context.AssemblyCandidateFinder,
                context.Services,
                context.Environment,
                context.Configuration,
                context.Logger,
                context.Properties
            );
            context.Services.AddLogging();

            loggingBuilder.Build();
        }

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(ILoggingConventionContext context)
        {
            context.AddConfiguration(context.Configuration.GetSection("Logging"));
            context.SetMinimumLevel(_options.GetLogLevel(context));
        }
    }
}
