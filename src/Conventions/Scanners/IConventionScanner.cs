using System;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Interface IConventionScanner
    /// </summary>
    /// TODO Edit XML Comment Template for IConventionScanner
    public interface IConventionScanner
    {
        /// <summary>
        /// Adds a delgate to be part of the scan
        /// </summary>
        /// <param name="delegate"></param>
        void AddDelegate(Delegate @delegate);

        /// <summary>
        /// Adds the convention.
        /// </summary>
        /// <param name="convention">The convention.</param>
        /// <returns>IEnumerable&lt;IServiceConvention&gt;.</returns>
        /// TODO Edit XML Comment Template for AddConvention
        void AddConvention(IConvention convention);

        /// <summary>
        /// Excepts the convention.
        /// </summary>
        /// <param name="conventionType">Type of the convention.</param>
        /// TODO Edit XML Comment Template for ExceptConvention
        void ExceptConvention(Type conventionType);

        /// <summary>
        /// To the provider.
        /// </summary>
        /// <returns>IConventionProvider.</returns>
        /// TODO Edit XML Comment Template for ToProvider
        IConventionProvider BuildProvider();
    }
}
