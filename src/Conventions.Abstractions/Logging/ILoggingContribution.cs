using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Logging
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="IConvention" />
    public interface ILoggingConvention : IConvention
    {
        /// <summary>
        /// Register additional logging providers with the logging builder
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configuration"></param>
        /// <param name="builder"></param>
        void Register([NotNull] IConventionContext context, [NotNull] IConfiguration configuration, [NotNull] ILoggingBuilder builder);
    }
}