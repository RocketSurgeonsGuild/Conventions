using System;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A convention provider that is bootstrapped with a set of conventions that allow for manual inalization of the initial conventions.
    /// Implements the <see cref="Rocket.Surgery.Conventions.Scanners.ConventionScannerBase" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.Scanners.ConventionScannerBase" />
    public class AggregateConventionScanner : ConventionScannerBase
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder</param>
        /// <param name="serviceProvider">The service provider for creating instances of conventions (usually a <see cref="IServiceProviderDictionary" />.</param>
        /// <param name="conventions">The additional conventions to start with</param>
        public AggregateConventionScanner(IAssemblyCandidateFinder assemblyCandidateFinder, IServiceProvider serviceProvider, params IConvention[] conventions)
            : base(assemblyCandidateFinder, serviceProvider)
        {
            PrependConvention(conventions);
        }
    }
}
