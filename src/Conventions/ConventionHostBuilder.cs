using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
#pragma warning disable IDE0058 // Expression value is never used

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ConventionHostBuilder.
    /// Implements the <see cref="IConventionHostBuilder" />
    /// </summary>
    /// <typeparam name="TSelf">The type of the t self.</typeparam>
    /// <seealso cref="IConventionHostBuilder" />
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
        protected ConventionHostBuilder(
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
            {
                Properties[typeof(IConventionScanner)] = scanner;
            }

            if (!Properties.TryGetValue(typeof(IAssemblyProvider), out var _))
            {
                Properties[typeof(IAssemblyProvider)] = assemblyProvider;
            }

            if (!Properties.TryGetValue(typeof(IAssemblyCandidateFinder), out var _))
            {
                Properties[typeof(IAssemblyCandidateFinder)] = assemblyCandidateFinder;
            }

            if (!Properties.TryGetValue(typeof(DiagnosticSource), out var _))
            {
                Properties[typeof(DiagnosticSource)] = diagnosticSource;
            }

            if (!Properties.TryGetValue(typeof(ILogger), out var _))
            {
                Properties[typeof(ILogger)] = new DiagnosticLogger(diagnosticSource);
            }
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Object.</returns>
        public virtual object? this[object item]
        {
            get => Properties.TryGetValue(item, out var value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IDictionary<object, object?> Properties => ServiceProperties;

        /// <summary>
        /// Apply a set of calls against a host builder without losing the context of the actual host build
        /// </summary>
        public TSelf Apply([NotNull] Action<IConventionHostBuilder> hostBuilder)
        {
            if (hostBuilder == null)
            {
                throw new ArgumentNullException(nameof(hostBuilder));
            }

            hostBuilder(this);
            return (TSelf)(object)this;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IServiceProviderDictionary ServiceProperties { get; }

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
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention(params IConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention(params Type[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention(IEnumerable<Type> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention<T>()
            where T : IConvention
        {
            Scanner.AppendConvention<T>();
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention(params IConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention(params Type[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention(IEnumerable<Type> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention<T>()
            where T : IConvention
        {
            Scanner.PrependConvention<T>();
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependDelegate(params Delegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendDelegate(params Delegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }
    }
}