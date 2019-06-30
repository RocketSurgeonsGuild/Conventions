using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A simple convention scanner that scans using the provided assembly candidate finder
    /// Implements the <see cref="Rocket.Surgery.Conventions.Scanners.ConventionScannerBase" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.Scanners.ConventionScannerBase" />
    public class SimpleConventionScanner : ConventionScannerBase
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder</param>
        public SimpleConventionScanner(IAssemblyCandidateFinder assemblyCandidateFinder)
            : base(assemblyCandidateFinder)
        {
        }
    }
}
