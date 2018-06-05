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
        private readonly IDictionary<object, object> _items;

        /// <summary>
        /// Creates a base context
        /// </summary>
        /// <param name="logger"></param>
        protected ConventionContext(ILogger logger, IDictionary<object, object> properties)
        {
            Logger = logger;
            _items = properties ?? new Dictionary<object, object>();
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual object this[object item]
        {
            get => _items.TryGetValue(item, out object value) ? value : null;
            set => _items[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        public IDictionary<object, object> Properties => _items;

        /// <inheritdoc />
        public ILogger Logger { get; }
    }
}
