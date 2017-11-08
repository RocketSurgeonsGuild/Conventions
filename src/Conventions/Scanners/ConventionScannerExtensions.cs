using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Class ConventionScannerExtensions.
    /// </summary>
    /// TODO Edit XML Comment Template for ConventionScannerExtensions
    public static class ConventionScannerExtensions
    {
        /// <summary>
        /// Excepts the convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="conventions">The conventions.</param>
        /// <returns>T.</returns>
        /// TODO Edit XML Comment Template for ExceptConvention`1
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
        /// Excepts the convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>T.</returns>
        /// TODO Edit XML Comment Template for ExceptConvention`1
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
        /// Excepts the convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>T.</returns>
        /// TODO Edit XML Comment Template for ExceptConvention`1
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
        /// Excepts the convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>T.</returns>
        /// TODO Edit XML Comment Template for ExceptConvention`1
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
        /// Excepts the convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>T.</returns>
        /// TODO Edit XML Comment Template for ExceptConvention`1
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
        /// Excepts the convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>T.</returns>
        /// TODO Edit XML Comment Template for ExceptConvention`1
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
