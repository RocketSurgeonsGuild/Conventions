#if !NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Default assembly provider that uses <see cref="DependencyContext"/>
    /// </summary>
    public class AppDomainAssemblyProvider : IAssemblyProvider
    {
        private readonly Lazy<IEnumerable<Assembly>> _assembles;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
        /// </summary>
        /// <param name="appDomain">The application domain</param>
        public AppDomainAssemblyProvider(AppDomain appDomain = null)
        {
            _assembles = new Lazy<IEnumerable<Assembly>>(() =>
                (appDomain ?? AppDomain.CurrentDomain).GetAssemblies().Where(x => x != null));
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        /// TODO Edit XML Comment Template for GetAssemblies
        public IEnumerable<Assembly> GetAssemblies() => _assembles.Value;
    }
}
#endif
