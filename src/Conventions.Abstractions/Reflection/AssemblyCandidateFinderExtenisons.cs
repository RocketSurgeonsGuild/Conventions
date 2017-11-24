using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Extension methods for <see cref="IAssemblyCandidateFinder"/>
    /// </summary>
    public static class AssemblyCandidateFinderExtenisons
    {
        /// <summary>
        /// Get the candidates for a given set
        /// </summary>
        /// <param name="finder">The <see cref="IAssemblyCandidateFinder"/></param>
        /// <param name="candidate">The first candidate to find</param>
        /// <param name="candidates">The candidates as an array</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetCandidateAssemblies(this IAssemblyCandidateFinder finder, string candidate, params string[] candidates)
        {
            return finder.GetCandidateAssemblies(new[] {candidate}.Concat(candidates));
        }
    }
}