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

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblies">The additional types to exclude.</param>
        /// <returns>The scanner</returns>
        public static T ExceptConvention<T>(this T scanner, params Assembly[] assemblies)
            where T : IConventionScanner
        {
            foreach (var type in assemblies)
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
        /// <param name="assemblies">The convention types to exclude.</param>
        /// <returns>The scanner</returns>
        public static T ExceptConvention<T>(this T scanner, IEnumerable<Assembly> assemblies)
            where T : IConventionScanner
        {
            foreach (var type in assemblies)
            {
                scanner.ExceptConvention(type);
            }
            return scanner;
        }
    }
}
