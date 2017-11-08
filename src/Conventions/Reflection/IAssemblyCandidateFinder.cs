using System.Collections.Generic;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Interface IAssemblyCandidateFinder
    /// </summary>
    /// TODO Edit XML Comment Template for IAssemblyCandidateFinder
    public interface IAssemblyCandidateFinder
    {
        /// <summary>
        /// Gets the candidate assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        /// TODO Edit XML Comment Template for GetCandidateAssemblies
        IEnumerable<Assembly> GetCandidateAssemblies(params string[] candidates);

        /// <summary>
        /// Gets the candidate assemblies.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        /// TODO Edit XML Comment Template for GetCandidateAssemblies
        IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates);
    }
}
