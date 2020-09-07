using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Container for conventions
    /// </summary>
    public sealed class ConventionWithDependencies : IConventionWithDependencies
    {
        private readonly List<ConventionDependency> _dependencies;

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="hostType"></param>
        public ConventionWithDependencies(IConvention convention, HostType hostType)
        {
            Convention = convention;
            HostType = hostType;
            _dependencies = new List<ConventionDependency>();
        }

        /// <inheritdoc />
        public IConvention Convention { get; }

        /// <summary>
        /// The dependencies
        /// </summary>
        public IEnumerable<IConventionDependency> Dependencies => _dependencies.OfType<IConventionDependency>();

        internal IEnumerable<ConventionDependency> InnerDependencies => _dependencies;

        /// <inheritdoc />
        public HostType HostType { get; }

        /// <summary>
        /// Adds a new dependency to the list
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ConventionWithDependencies WithDependency(DependencyDirection direction, Type type)
        {
            _dependencies.Add(new ConventionDependency(direction, type));
            return this;
        }
    }
}