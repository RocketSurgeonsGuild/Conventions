using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Container for conventions
    /// </summary>
    public sealed class ConventionWithDependencies : IConventionWithDependencies
    {
        private readonly List<IConventionDependency> _dependencies;

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="hostType"></param>
        public ConventionWithDependencies(IConvention convention, HostType hostType)
        {
            Convention = convention;
            HostType = hostType;
            _dependencies = new List<IConventionDependency>();
        }

        /// <inheritdoc />
        public IConvention Convention { get; }

        /// <inheritdoc />
        public IEnumerable<IConventionDependency> Dependencies => _dependencies;

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

        private sealed class ConventionDependency : IConventionDependency
        {
            public ConventionDependency(DependencyDirection direction, Type type)
            {
                Type = type;
                Direction = direction;
            }
            public Type Type { get; }
            public DependencyDirection Direction { get; }
        }
    }
}