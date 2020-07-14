using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// IConventionHostBuilder
    /// </summary>
    public interface IConventionHostBuilder
    {
        /// <summary>
        /// Gets the scanner.
        /// </summary>
        /// <value>The scanner.</value>
        [NotNull]
        IConventionScanner Scanner { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        [NotNull]
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        [NotNull]
        IServiceProviderDictionary ServiceProperties { get; }

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull]
        IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the diagnostic logger.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull]
        ILogger DiagnosticLogger { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention([NotNull] IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention([NotNull] IEnumerable<Type> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention(params Type[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention<T>()
            where T : IConvention;

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention([NotNull] IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention([NotNull] IEnumerable<Type> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention(params Type[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention<T>()
            where T : IConvention;

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendDelegate([NotNull] IEnumerable<Delegate> delegates);


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendDelegate(params Delegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependDelegate([NotNull] IEnumerable<Delegate> delegates);

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependDelegate(params Delegate[] delegates);
    }
}