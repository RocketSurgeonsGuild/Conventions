using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ConventionHostBuilder.
    /// Implements the <see cref="IConventionHostBuilder{TSelf}" />
    /// </summary>
    /// <typeparam name="TSelf">The type of the t self.</typeparam>
    /// <seealso cref="IConventionHostBuilder{TSelf}" />
    public abstract class ConventionHostBuilder<TSelf> : IConventionHostBuilder
        where TSelf : IConventionHostBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionHostBuilder{TSelf}" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="serviceProperties">The properties.</param>
        public ConventionHostBuilder(
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IAssemblyProvider assemblyProvider,
            DiagnosticSource diagnosticSource,
            IServiceProviderDictionary serviceProperties
        )
        {
            Scanner = scanner;
            AssemblyCandidateFinder = assemblyCandidateFinder;
            AssemblyProvider = assemblyProvider;
            DiagnosticSource = diagnosticSource;
            ServiceProperties = serviceProperties ?? new ServiceProviderDictionary();

            if (!Properties.TryGetValue(typeof(IConventionScanner), out var _))
                Properties[typeof(IConventionScanner)] = scanner;

            if (!Properties.TryGetValue(typeof(IAssemblyProvider), out var _))
                Properties[typeof(IAssemblyProvider)] = assemblyProvider;

            if (!Properties.TryGetValue(typeof(IAssemblyCandidateFinder), out var _))
                Properties[typeof(IAssemblyCandidateFinder)] = assemblyCandidateFinder;

            if (!Properties.TryGetValue(typeof(DiagnosticSource), out var _))
                Properties[typeof(DiagnosticSource)] = diagnosticSource;

            if (!Properties.TryGetValue(typeof(ILogger), out var _))
                Properties[typeof(ILogger)] = new DiagnosticLogger(diagnosticSource);
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Object.</returns>
        public virtual object this[object item]
        {
            get => Properties.TryGetValue(item, out var value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IServiceProviderDictionary ServiceProperties { get; }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IDictionary<object, object> Properties => ServiceProperties;
        /// <summary>
        /// Gets the scanner.
        /// </summary>
        /// <value>The scanner.</value>
        public IConventionScanner Scanner { get; }
        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        public IAssemblyProvider AssemblyProvider { get; }
        /// <summary>
        /// Gets the diagnostic source.
        /// </summary>
        /// <value>The diagnostic source.</value>
        public DiagnosticSource DiagnosticSource { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf AppendConvention(params IConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf AppendConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf AppendConvention(params Type[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf AppendConvention(IEnumerable<Type> conventions)
        {
            Scanner.AppendConvention(conventions);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TSelf.</returns>

        public TSelf AppendConvention<T>() where T : IConvention
        {
            Scanner.AppendConvention<T>();
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf PrependConvention(params IConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf PrependConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf PrependConvention(params Type[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf PrependConvention(IEnumerable<Type> conventions)
        {
            Scanner.PrependConvention(conventions);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TSelf.</returns>

        public TSelf PrependConvention<T>() where T : IConvention
        {
            Scanner.PrependConvention<T>();
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TSelf.</returns>

        public TSelf PrependDelegate(params Delegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TSelf.</returns>

        public TSelf PrependDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return (TSelf)(object)this;
        }


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TSelf.</returns>

        public TSelf AppendDelegate(params Delegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.AppendDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.AppendConvention(params IConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.AppendConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.AppendConvention(params Type[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.AppendConvention(IEnumerable<Type> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.AppendConvention<T>() where T : IConvention
        {
            Scanner.AppendConvention<T>();
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.PrependConvention(params IConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.PrependConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.PrependConvention(params Type[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.PrependConvention(IEnumerable<Type> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.PrependConvention<T>() where T : IConvention
        {
            Scanner.PrependConvention<T>();
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.PrependDelegate(params Delegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.PrependDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.AppendDelegate(params Delegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TSelf.</returns>

        IConventionHostBuilder IConventionHostBuilder.AppendDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }
    }
}
