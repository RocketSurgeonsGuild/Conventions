using System.Collections.Generic;

namespace Rocket.Surgery.Builders
{
    /// <summary>
    /// A common interface for creating builder classes
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        IDictionary<object, object> Properties { get; }
    }
}
