using System;
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
        /// <summary>
        /// Creates a base context
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="logger"></param>
        /// <param name="properties"></param>
        protected ConventionContext(IRocketEnvironment environment, ILogger logger, IDictionary<object, object> properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Properties = properties ?? new Dictionary<object, object>();
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual object this[object item]
        {
            get => Properties.TryGetValue(item, out object value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        public IDictionary<object, object> Properties { get; }

        /// <inheritdoc />
        public ILogger Logger { get; }

        /// <inheritdoc />
        public IRocketEnvironment Environment { get; }
    }
}
