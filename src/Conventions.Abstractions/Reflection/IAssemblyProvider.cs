using System.Collections.Generic;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Interface IAssemblyProvider
    /// </summary>
    /// TODO Edit XML Comment Template for IAssemblyProvider
    public interface IAssemblyProvider
    {
        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        /// TODO Edit XML Comment Template for GetAssemblies
        IEnumerable<Assembly> GetAssemblies();
    }
}
