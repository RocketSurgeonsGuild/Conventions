using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Builders
{
    /// <summary>
    /// Class Container.
    /// </summary>
    /// TODO Edit XML Comment Template for Container
    public abstract class Builder: IBuilder
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

    /// <summary>
    /// Class Container.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the t return.</typeparam>
    /// <seealso cref="Builder" />
    public abstract class Builder<TBuilder> : Builder
        where TBuilder : class, IBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Container" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <exception cref="System.ArgumentNullException">parent</exception>
        protected Builder(TBuilder parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// TODO Edit XML Comment Template for Parent
        public TBuilder Parent { get; }

        /// <summary>
        /// Exits this instance.
        /// </summary>
        /// <returns>TReturn.</returns>
        public TBuilder Exit()
        {
            return Parent;
        }
    }
}
