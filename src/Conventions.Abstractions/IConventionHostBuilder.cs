using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    ///  IConventionHostBuilder
    /// </summary>
    public interface IConventionHostBuilder
    {
        /// <summary>
        /// Gets the scanner.
        /// </summary>
        /// <value>The scanner.</value>
        IConventionScanner Scanner { get; }
        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        IServiceProviderDictionary ServiceProperties { get; }
        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        IAssemblyProvider AssemblyProvider { get; }
        /// <summary>
        /// Gets the diagnostic source.
        /// </summary>
        /// <value>The diagnostic source.</value>
        DiagnosticSource DiagnosticSource { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>

        IConventionHostBuilder AppendConvention(IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>

        IConventionHostBuilder AppendConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder AppendConvention(IEnumerable<Type> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder AppendConvention(params Type[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder AppendConvention<T>() where T : IConvention;

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>

        IConventionHostBuilder PrependConvention(IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>

        IConventionHostBuilder PrependConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder PrependConvention(IEnumerable<Type> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>

        IConventionHostBuilder PrependConvention(params Type[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder PrependConvention<T>() where T : IConvention;

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder AppendDelegate(IEnumerable<Delegate> delegates);


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder AppendDelegate(params Delegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder PrependDelegate(IEnumerable<Delegate> delegates);

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        IConventionHostBuilder PrependDelegate(params Delegate[] delegates);
    }
}
