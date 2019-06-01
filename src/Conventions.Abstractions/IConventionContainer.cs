using System;
using System.Collections.Generic;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{ 
    public interface IConventionContainer<out TBuilder, in TConvention, in TDelegate>
        where TBuilder : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        IConventionScanner Scanner { get; }
        IRocketEnvironment Environment { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        TBuilder AppendConvention(params TConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>The scanner</returns>
        TBuilder AppendConvention(IEnumerable<TConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        TBuilder PrependConvention(params TConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>The scanner</returns>
        TBuilder PrependConvention(IEnumerable<TConvention> conventions);


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        TBuilder PrependDelegate(params TDelegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        TBuilder PrependDelegate(IEnumerable<TDelegate> delegates);


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        TBuilder AppendDelegate(params TDelegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        TBuilder AppendDelegate(IEnumerable<TDelegate> delegates);
    }
}
