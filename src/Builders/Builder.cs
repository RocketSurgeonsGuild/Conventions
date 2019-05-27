using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Builders
{
    /// <summary>
    /// Abstract base class for implementing a builder
    /// </summary>
    public abstract class Builder : IBuilder
    {
        private readonly IDictionary<object, object> _items;

        protected Builder(IDictionary<object, object> properties)
        {
            _items = properties ?? new Dictionary<object, object>();
        }

        public virtual object this[object item]
        {
            get => _items.TryGetValue(item, out var value) ? value : null;
            set => _items[item] = value;
        }

        public IDictionary<object, object> Properties => _items;
    }

    /// <summary>
    /// Abstract base class for creating builders that are attached to some parent builder
    /// Useful for creating sub builds that live for a short period to augment the parent.
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    public abstract class Builder<TBuilder> : Builder
        where TBuilder : class
    {
        /// <summary>
        /// Constructs a Builder&lt;TBuilder&gt; with the parent instance
        /// </summary>
        /// <param name="parent">The parent builder TBuilder</param>
        protected Builder(TBuilder parent, IDictionary<object, object> properties) : base(properties)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// Get the parent value from this builder
        /// </summary>
        public TBuilder Parent { get; }

        /// <summary>
        /// Exit the current builder and return the parent
        /// </summary>
        /// <returns></returns>
        public virtual TBuilder Exit()
        {
            return Parent;
        }
    }
}
