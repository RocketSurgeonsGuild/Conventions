using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Builders
{
    /// <summary>
    /// Abstract base class for implementing a builder
    /// Implements the <see cref="IBuilder" />
    /// </summary>
    /// <seealso cref="IBuilder" />
    public abstract class Builder : IBuilder
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected Builder(IDictionary<object, object> properties)
        {
            Properties = properties ?? new Dictionary<object, object>();
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Object.</returns>

        public virtual object this[object item]
        {
            get => Properties.TryGetValue(item, out var value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IDictionary<object, object> Properties { get; }
    }

    /// <summary>
    /// Abstract base class for creating builders that are attached to some parent builder
    /// Useful for creating sub builds that live for a short period to augment the parent.
    /// Implements the <see cref="Builder" />
    /// </summary>
    /// <typeparam name="TBuilder">The type of the t builder.</typeparam>
    /// <seealso cref="Builder" />
    public abstract class Builder<TBuilder> : Builder
        where TBuilder : class
    {
        /// <summary>
        /// Constructs a Builder{TBuilder} with the parent instance
        /// </summary>
        /// <param name="parent">The parent builder TBuilder</param>
        /// <param name="properties">The properties</param>
        protected Builder(TBuilder parent, IDictionary<object, object> properties) : base(properties)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// Get the parent value from this builder
        /// </summary>
        /// <value>The parent.</value>
        public TBuilder Parent { get; }

        /// <summary>
        /// Exit the current builder and return the parent
        /// </summary>
        /// <returns>TBuilder.</returns>
        public virtual TBuilder Exit()
        {
            return Parent;
        }
    }
}
