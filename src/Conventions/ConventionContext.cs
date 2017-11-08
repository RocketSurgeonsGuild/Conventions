using System.Collections.Generic;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention context that allos for stashing items in index keys
    /// </summary>
    public abstract class ConventionContext : IConventionContext
    {

        private readonly IDictionary<object, object> _items = new Dictionary<object, object>();

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
    }
}
