using System.Collections.Generic;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Declares a convention result with it's dependencies pre-computed
    /// </summary>
    public interface IConventionWithDependencies
    {
        /// <summary>
        /// The convention
        /// </summary>
        IConvention Convention { get; }

        /// <summary>
        /// The dependencies
        /// </summary>
        IEnumerable<IConventionDependency> Dependencies { get; }

        /// <summary>
        /// The host type of the convention
        /// </summary>
        HostType HostType { get; }
    }
}