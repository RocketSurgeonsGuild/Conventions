using System;

namespace Rocket.Surgery.Conventions.Scanners
{
    public interface IConventionContainer
    {
        /// <summary>
        /// Add a delegate to the scanner, that runs before scanning.
        /// </summary>
        /// <param name="delegate">The delegate</param>
        void PrependDelegate(Delegate @delegate);

        /// <summary>
        /// Adds a convention to the scanner, that runs before scanning.
        /// </summary>
        /// <param name="convention">The convention</param>
        void PrependConvention(IConvention convention);

        /// <summary>
        /// Add a delegate to the scanner, that runs after scanning.
        /// </summary>
        /// <param name="delegate">The delegate</param>
        void AppendDelegate(Delegate @delegate);

        /// <summary>
        /// Adds a convention to the scanner, that runs after scanning.
        /// </summary>
        /// <param name="convention">The convention</param>
        void AppendConvention(IConvention convention);
    }
}