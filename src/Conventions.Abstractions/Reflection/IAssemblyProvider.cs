using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

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
        /// <returns>IEnumerable{Assembly}.</returns>
        [NotNull] IEnumerable<Assembly> GetAssemblies();
    }
}