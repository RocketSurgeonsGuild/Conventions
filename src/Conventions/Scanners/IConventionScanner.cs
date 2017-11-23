using System;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// The convention scanner interface is used to find conventions
    ///     and return those conventions in order they are added.
    /// </summary>
    public interface IConventionScanner
    {
        /// <summary>
        /// Add a delegate to the scanner
        /// </summary>
        /// <param name="delegate">The delegate</param>
        void AddDelegate(Delegate @delegate);

        /// <summary>
        /// Adds a convention to the scanner.
        /// </summary>
        /// <param name="convention">The convention</param>
        void AddConvention(IConvention convention);

        /// <summary>
        /// Exclude certian conventions by their implemented type.
        /// </summary>
        /// <param name="type">The type to exclude</param>
        void ExceptConvention(Type type);

        /// <summary>
        /// Creates a provider that returns a set of convetions.
        /// </summary>
        /// <returns></returns>
        IConventionProvider BuildProvider();
    }
}
