using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Class SimpleConventionScanner.
    /// </summary>
    /// <seealso cref="IConventionScanner" />
    /// TODO Edit XML Comment Template for SimpleConventionScanner
    public class SimpleConventionScanner : ConventionScannerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConventionScanner" /> class.
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly provider.</param>
        /// TODO Edit XML Comment Template for #ctor
        public SimpleConventionScanner(IAssemblyCandidateFinder assemblyCandidateFinder)
            : base(assemblyCandidateFinder)
        {
        }
    }
}
