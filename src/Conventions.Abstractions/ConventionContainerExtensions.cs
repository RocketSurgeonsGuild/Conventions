using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Class ConventionScannerExtensions.
    /// </summary>
    /// TODO Edit XML Comment Template for ConventionScannerExtensions
    public static class ConventionContainerExtensions
    {
        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public static void AppendConvention(this IConventionScanner scanner, params IConvention[] conventions)
        {
            foreach (var type in conventions)
            {
                scanner.AppendConvention(type);
            }
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>The scanner</returns>
        public static void AppendConvention(this IConventionScanner scanner, IEnumerable<IConvention> types)
        {
            foreach (var type in types)
            {
                scanner.AppendConvention(type);
            }
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public static void PrependConvention(this IConventionScanner scanner, params IConvention[] conventions)
        {
            foreach (var type in conventions)
            {
                scanner.PrependConvention(type);
            }
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>The scanner</returns>
        public static void PrependConvention(this IConventionScanner scanner, IEnumerable<IConvention> types)
        {
            foreach (var type in types)
            {
                scanner.PrependConvention(type);
            }
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        public static void PrependDelegate(this IConventionScanner scanner, params Delegate[] delegates)
        {
            foreach (var type in delegates)
            {
                scanner.PrependDelegate(type);
            }
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        public static void PrependDelegate(this IConventionScanner scanner, IEnumerable<Delegate> delegates)
        {
            foreach (var type in delegates)
            {
                scanner.PrependDelegate(type);
            }
        }


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        public static void AppendDelegate(this IConventionScanner scanner, params Delegate[] delegates)
        {
            foreach (var type in delegates)
            {
                scanner.AppendDelegate(type);
            }
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        public static void AppendDelegate(this IConventionScanner scanner, IEnumerable<Delegate> delegates)
        {
            foreach (var type in delegates)
            {
                scanner.AppendDelegate(type);
            }
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public static T AppendConvention<T>(this T scanner, params IConvention[] conventions)
            where T : IConventionHostBuilder
        {
            foreach (var type in conventions)
            {
                scanner.AppendConvention(type);
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
        public static T AppendConvention<T>(this T scanner, IEnumerable<IConvention> types)
            where T : IConventionHostBuilder
        {
            foreach (var type in types)
            {
                scanner.AppendConvention(type);
            }
            return scanner;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public static T PrependConvention<T>(this T scanner, params IConvention[] conventions)
            where T : IConventionHostBuilder
        {
            foreach (var type in conventions)
            {
                scanner.PrependConvention(type);
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
        public static T PrependConvention<T>(this T scanner, IEnumerable<IConvention> types)
            where T : IConventionHostBuilder
        {
            foreach (var type in types)
            {
                scanner.PrependConvention(type);
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
        public static T PrependDelegate<T>(this T scanner, params Delegate[] delegates)
            where T : IConventionHostBuilder
        {
            foreach (var type in delegates)
            {
                scanner.PrependDelegate(type);
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
        public static T PrependDelegate<T>(this T scanner, IEnumerable<Delegate> delegates)
            where T : IConventionHostBuilder
        {
            foreach (var type in delegates)
            {
                scanner.PrependDelegate(type);
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
        public static T AppendDelegate<T>(this T scanner, params Delegate[] delegates)
            where T : IConventionHostBuilder
        {
            foreach (var type in delegates)
            {
                scanner.AppendDelegate(type);
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
        public static T AppendDelegate<T>(this T scanner, IEnumerable<Delegate> delegates)
            where T : IConventionHostBuilder
        {
            foreach (var type in delegates)
            {
                scanner.AppendDelegate(type);
            }
            return scanner;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public static T AppendConvention<T, TBuilder, TConvention, TDelegate>(this T builder, params TConvention[] conventions)
            where T : IConventionContainer<TBuilder, TConvention, TDelegate>
            where TBuilder : IBuilder
            where TConvention : IConvention
            where TDelegate : Delegate
        {
            foreach (var type in conventions)
            {
                builder.AppendConvention(type);
            }
            return builder;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>The scanner</returns>
        public static T AppendConvention<T, TBuilder, TConvention, TDelegate>(this T scanner, IEnumerable<TConvention> types)
            where T : IConventionContainer<TBuilder, TConvention, TDelegate>
            where TBuilder : IBuilder
            where TConvention : IConvention
            where TDelegate : Delegate
        {
            foreach (var type in types)
            {
                scanner.AppendConvention(type);
            }
            return scanner;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public static T PrependConvention<T, TBuilder, TConvention, TDelegate>(this T scanner, params TConvention[] conventions)
           where T : IConventionContainer<TBuilder, TConvention, TDelegate>
            where TBuilder : IBuilder
            where TConvention : IConvention
            where TDelegate : Delegate
        {
            foreach (var type in conventions)
            {
                scanner.PrependConvention(type);
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
        public static T PrependConvention<T, TBuilder, TConvention, TDelegate>(this T scanner, IEnumerable<TConvention> types)
            where T : IConventionContainer<TBuilder, TConvention, TDelegate>
            where TBuilder : IBuilder
            where TConvention : IConvention
            where TDelegate : Delegate
        {
            foreach (var type in types)
            {
                scanner.PrependConvention(type);
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
        public static T PrependDelegate<T, TBuilder, TConvention, TDelegate>(this T scanner, params TDelegate[] delegates)
            where T : IConventionContainer<TBuilder, TConvention, TDelegate>
            where TBuilder : IBuilder
            where TConvention : IConvention
            where TDelegate : Delegate
        {
            foreach (var type in delegates)
            {
                scanner.PrependDelegate(type);
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
        public static T PrependDelegate<T, TBuilder, TConvention, TDelegate>(this T scanner, IEnumerable<TDelegate> delegates)
            where T : IConventionContainer<TBuilder, TConvention, TDelegate>
            where TBuilder : IBuilder
            where TConvention : IConvention
            where TDelegate : Delegate
        {
            foreach (var type in delegates)
            {
                scanner.PrependDelegate(type);
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
        public static T AppendDelegate<T, TBuilder, TConvention, TDelegate>(this T scanner, params TDelegate[] delegates)
            where T : IConventionContainer<TBuilder, TConvention, TDelegate>
            where TBuilder : IBuilder
            where TConvention : IConvention
            where TDelegate : Delegate
        {
            foreach (var type in delegates)
            {
                scanner.AppendDelegate(type);
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
        public static T AppendDelegate<T, TBuilder, TConvention, TDelegate>(this T scanner, IEnumerable<TDelegate> delegates)
            where T : IConventionContainer<TBuilder, TConvention, TDelegate>
            where TBuilder : IBuilder
            where TConvention : IConvention
            where TDelegate : Delegate
        {
            foreach (var type in delegates)
            {
                scanner.AppendDelegate(type);
            }
            return scanner;
        }
    }
}
