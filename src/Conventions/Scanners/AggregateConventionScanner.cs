using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Class AggregateConventionScanner.
    /// </summary>
    /// <seealso cref="IConventionScanner" />
    /// TODO Edit XML Comment Template for AggregateConventionScanner
    public class AggregateConventionScanner : ConventionScannerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateConventionScanner"/> class.
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly provider.</param>
        /// <param name="conventions">The conventions.</param>
        /// TODO Edit XML Comment Template for #ctor
        public AggregateConventionScanner(IAssemblyCandidateFinder assemblyCandidateFinder, params IConvention[] conventions)
            : base(assemblyCandidateFinder)
        {
            IncludeConventions.AddRange(conventions);
        }
    }
}
