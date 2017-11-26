using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Default assembly provider that uses a list of assemblies
    /// </summary>
    public class DefaultAssemblyProvider : IAssemblyProvider
    {
        private readonly IEnumerable<Assembly> _assembles;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies</param>
        public DefaultAssemblyProvider(IEnumerable<Assembly> assemblies)
        {
            _assembles = assemblies.Where(x => x != null).ToArray();
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        /// TODO Edit XML Comment Template for GetAssemblies
        public IEnumerable<Assembly> GetAssemblies() => _assembles;
    }
}
