using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    public interface IConventionHostBuilder
    {
        IConventionScanner Scanner { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IAssemblyProvider AssemblyProvider { get; }
        DiagnosticSource DiagnosticSource { get; }
    }

    public interface IConventionHostBuilder<out TSelf> : IConventionHostBuilder
        where TSelf : IConventionHostBuilder<TSelf>
    {
        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        TSelf AppendConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>The scanner</returns>
        TSelf AppendConvention(IEnumerable<IConvention> types);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        TSelf PrependConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="types">The conventions.</param>
        /// <returns>The scanner</returns>
        TSelf PrependConvention(IEnumerable<IConvention> types);

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        TSelf PrependDelegate(params Delegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        TSelf PrependDelegate(IEnumerable<Delegate> delegates);

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        TSelf AppendDelegate(params Delegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        TSelf AppendDelegate(IEnumerable<Delegate> delegates);
    }
}
