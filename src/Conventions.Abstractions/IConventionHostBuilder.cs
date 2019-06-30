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
    }

    /// <summary>
    ///  IConventionHostBuilder
    /// Implements the <see cref="Rocket.Surgery.Conventions.IConventionHostBuilder" />
    /// </summary>
    /// <typeparam name="TSelf">The type of the t self.</typeparam>
    /// <seealso cref="Rocket.Surgery.Conventions.IConventionHostBuilder" />
    public interface IConventionHostBuilder<out TSelf> : IConventionHostBuilder
        where TSelf : IConventionHostBuilder<TSelf>
    {
        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        TSelf AppendConvention(IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        TSelf AppendConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>
        TSelf AppendConvention(IEnumerable<Type> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>
        TSelf AppendConvention(params Type[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TSelf.</returns>
        TSelf AppendConvention<T>() where T : IConvention;

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        TSelf PrependConvention(IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        TSelf PrependConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>
        TSelf PrependConvention(IEnumerable<Type> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        TSelf PrependConvention(params Type[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TSelf.</returns>
        TSelf PrependConvention<T>() where T : IConvention;

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TSelf.</returns>
        TSelf AppendDelegate(IEnumerable<Delegate> delegates);


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TSelf.</returns>
        TSelf AppendDelegate(params Delegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TSelf.</returns>
        TSelf PrependDelegate(IEnumerable<Delegate> delegates);

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TSelf.</returns>
        TSelf PrependDelegate(params Delegate[] delegates);
    }
}
