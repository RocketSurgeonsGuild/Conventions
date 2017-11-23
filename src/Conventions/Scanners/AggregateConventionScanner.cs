using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A convention provider that is bootstrapped with a set of conventions that allow for manual inalization of the initial conventions. 
    /// </summary>
    public class AggregateConventionScanner : ConventionScannerBase
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder</param>
        /// <param name="conventions">The additional conventions to start with</param>
        public AggregateConventionScanner(IAssemblyCandidateFinder assemblyCandidateFinder, params IConvention[] conventions)
            : base(assemblyCandidateFinder)
        {
            IncludeConventions.AddRange(conventions);
        }
    }
}
