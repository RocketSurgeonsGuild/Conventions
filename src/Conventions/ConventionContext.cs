using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention context that allos for stashing items in index keys
    /// </summary>
    public abstract class ConventionContext : IConventionContext
    {
        private readonly IDictionary<object, object> _items = new Dictionary<object, object>();

        /// <summary>
        /// Creates a base context
        /// </summary>
        /// <param name="logger"></param>
        protected ConventionContext(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// The indexer that contains the items
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual object this[object item]
        {
            get => _items.TryGetValue(item, out object value) ? value : null;
            set => _items[item] = value;
        }

        /// <inheritdoc />
        public ILogger Logger { get; }
    }
}
