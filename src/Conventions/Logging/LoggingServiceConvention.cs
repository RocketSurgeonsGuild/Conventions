using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;

[assembly: Convention(typeof(LoggingServiceConvention))]

namespace Rocket.Surgery.Conventions.Logging
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
        /// Initializes a new instance of the <see cref="LoggingServiceConvention" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public LoggingServiceConvention(RocketLoggingOptions? options = null)
            => _options = options ?? new RocketLoggingOptions();

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(ILoggingConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.AddConfiguration(context.Configuration.GetSection("Logging"));
            var logLevel = _options.GetLogLevel(context);
            if (logLevel.HasValue)
            {
                context.SetMinimumLevel(logLevel.Value);
            }
        }

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(IServiceConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var loggingBuilder = new LoggingBuilder(
                context.Get<IConventionScanner>()!,
                context.AssemblyProvider,
                context.AssemblyCandidateFinder,
                context.Services,
                context.Configuration,
                context.Logger,
                context.Properties
            );
            context.Services.AddLogging();

            loggingBuilder.Build();
        }
    }
}