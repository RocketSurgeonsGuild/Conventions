using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// IConventionContainer
    /// </summary>
    /// <typeparam name="TBuilder">The type of the t builder.</typeparam>
    /// <typeparam name="TConvention">The type of the t convention.</typeparam>
    /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
    public interface IConventionContainer<out TBuilder, in TConvention, in TDelegate>
        where TBuilder : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        /// <summary>
        /// Gets the scanner.
        /// </summary>
        /// <value>The scanner.</value>
        [NotNull]
        IConventionScanner Scanner { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TBuilder.</returns>
        TBuilder AppendConvention(params TConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TBuilder.</returns>
        TBuilder AppendConvention([NotNull] IEnumerable<TConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TBuilder.</returns>
        TBuilder AppendConvention<T>()
            where T : TConvention;

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TBuilder.</returns>
        TBuilder PrependConvention(params TConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TBuilder.</returns>
        TBuilder PrependConvention([NotNull] IEnumerable<TConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TBuilder.</returns>
        TBuilder PrependConvention<T>()
            where T : TConvention;


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TBuilder.</returns>
        TBuilder PrependDelegate(params TDelegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TBuilder.</returns>
        TBuilder PrependDelegate([NotNull] IEnumerable<TDelegate> delegates);


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TBuilder.</returns>
        TBuilder AppendDelegate(params TDelegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TBuilder.</returns>
        TBuilder AppendDelegate([NotNull] IEnumerable<TDelegate> delegates);
    }
}