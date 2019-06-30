using System.Collections.Generic;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// A provider that gets a list of assemblies for a given context
    /// </summary>
    public interface IAssemblyProvider
    {
        /// <summary>
        /// Get the full list of assemblies
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        IEnumerable<Assembly> GetAssemblies();
    }
}
