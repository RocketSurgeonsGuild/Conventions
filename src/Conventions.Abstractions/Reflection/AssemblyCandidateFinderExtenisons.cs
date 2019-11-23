using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Extension methods for <see cref="IAssemblyCandidateFinder" />
    /// </summary>
    public static class AssemblyCandidateFinderExtenisons
    {
        /// <summary>
        /// Get the candidates for a given set
        /// </summary>
        /// <param name="finder">The <see cref="IAssemblyCandidateFinder" /></param>
        /// <param name="candidate">The first candidate to find</param>
        /// <param name="candidates">The candidates as an array</param>
        /// <returns>IEnumerable{Assembly}.</returns>
        [NotNull]
        public static IEnumerable<Assembly> GetCandidateAssemblies(
            [NotNull] this IAssemblyCandidateFinder finder,
            string candidate,
            params string[] candidates
        )
        {
            if (finder == null)
            {
                throw new ArgumentNullException(nameof(finder));
            }

            return finder.GetCandidateAssemblies(new[] { candidate }.Concat(candidates).ToArray());
        }
    }
}