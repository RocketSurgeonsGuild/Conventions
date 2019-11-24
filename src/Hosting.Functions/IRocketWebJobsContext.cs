using System.Collections.Generic;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting.Functions
{
    /// <summary>
    /// Interface IRocketWebJobsContext
    /// Implements the <see cref="IConventionHostBuilder" />
    /// </summary>
    /// <seealso cref="IConventionHostBuilder" />
    public interface IRocketWebJobsContext : IConventionHostBuilder
    {
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [NotNull] IDictionary<object, object?> Properties { get; }
    }
}