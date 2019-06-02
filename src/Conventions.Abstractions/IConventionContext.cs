using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// The base context marker interface to define this as a context
    /// </summary>
    public interface IConventionContext
    {
        /// <summary>
        /// Allows a context to hold additional information for conventions to consume such as configuration objects
        /// </summary>
        object this[object item] { get; set; }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        IDictionary<object, object> Properties { get; }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        ILogger Logger { get; }
    }
}
