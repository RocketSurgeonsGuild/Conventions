using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Class ContributionScannerExtensions.
    /// </summary>
    /// TODO Edit XML Comment Template for ContributionScannerExtensions
    public static class ContributionScannerExtensions
    {
        /// <summary>
        /// Excepts the convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="conventions">The conventions.</param>
        /// <returns>T.</returns>
        /// TODO Edit XML Comment Template for ExceptConvention`1
        public static T AddContribution<T>(this T scanner, params IConvention[] conventions)
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
        public static T AddContribution<T>(this T scanner, IEnumerable<IConvention> types)
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
        /// <param name="contributions">The conventions.</param>
        /// <returns>T.</returns>
        /// TODO Edit XML Comment Template for ExceptConvention`1
        public static T AddContribution<T>(this T scanner, params Delegate[] contributions)
            where T : IConventionScanner
        {
            foreach (var type in contributions)
            {
                scanner.AddContribution(type);
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
        public static T AddContribution<T>(this T scanner, IEnumerable<Delegate> types)
            where T : IConventionScanner
        {
            foreach (var type in types)
            {
                scanner.AddContribution(type);
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
        public static T ExceptContribution<T>(this T scanner, params Type[] types)
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
        public static T ExceptContribution<T>(this T scanner, IEnumerable<Type> types)
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
