using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Class ConventionScannerExtensions.
    /// </summary>
    /// TODO Edit XML Comment Template for ConventionScannerExtensions
    public static class ConventionScannerExtensions
    {
        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public static T AddConvention<T>(this T scanner, params IConvention[] conventions)
            where T : IConventionScanner
        {
            foreach (var type in conventions)
            {
                scanner.AddConvention(type);
            }
            return scanner;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>The scanner</returns>
        public static T AddConvention<T>(this T scanner, IEnumerable<IConvention> types)
            where T : IConventionScanner
        {
            foreach (var type in types)
            {
                scanner.AddConvention(type);
            }
            return scanner;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        public static T AddDelegate<T>(this T scanner, params Delegate[] delegates)
            where T : IConventionScanner
        {
            foreach (var type in delegates)
            {
                scanner.AddDelegate(type);
            }
            return scanner;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        public static T AddDelegate<T>(this T scanner, IEnumerable<Delegate> delegates)
            where T : IConventionScanner
        {
            foreach (var type in delegates)
            {
                scanner.AddDelegate(type);
            }
            return scanner;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The additional types to exclude.</param>
        /// <returns>The scanner</returns>
        public static T ExceptConvention<T>(this T scanner, params Type[] types)
            where T : IConventionScanner
        {
            foreach (var type in types)
            {
                scanner.ExceptConvention(type);
            }
            return scanner;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The convention types to exclude.</param>
        /// <returns>The scanner</returns>
        public static T ExceptConvention<T>(this T scanner, IEnumerable<Type> types)
            where T : IConventionScanner
        {
            foreach (var type in types)
            {
                scanner.ExceptConvention(type);
            }
            return scanner;
        }
    }
}
