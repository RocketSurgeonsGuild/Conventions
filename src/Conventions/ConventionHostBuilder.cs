using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

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
            ServiceProperties = serviceProperties ?? new ServiceProviderDictionary();

            Properties[typeof(IConventionScanner)] = scanner;
            Properties[typeof(IAssemblyProvider)] = assemblyProvider;
            Properties[typeof(IAssemblyCandidateFinder)] = assemblyCandidateFinder;
            Properties[typeof(IServiceProviderDictionary)] = serviceProperties;
            Properties[typeof(ILogger)] = new DiagnosticLogger(diagnosticSource);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionHostBuilder{TSelf}" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="serviceProperties">The properties.</param>
        protected ConventionHostBuilder(
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IAssemblyProvider assemblyProvider,
            ILogger diagnosticLogger,
            IServiceProviderDictionary serviceProperties
        )
        {
            ServiceProperties = serviceProperties ?? new ServiceProviderDictionary();
            Properties[typeof(IConventionScanner)] = scanner;
            Properties[typeof(IAssemblyProvider)] = assemblyProvider;
            Properties[typeof(IAssemblyCandidateFinder)] = assemblyCandidateFinder;
            Properties[typeof(IServiceProviderDictionary)] = serviceProperties;
            Properties[typeof(ILogger)] = diagnosticLogger;
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
        public IConventionScanner Scanner => ServiceProperties.Get<IConventionScanner>();

        /// <inheritdoc />
        public ILogger DiagnosticLogger => ServiceProperties.Get<ILogger>();

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        public IAssemblyCandidateFinder AssemblyCandidateFinder => ServiceProperties.Get<IAssemblyCandidateFinder>();

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        public IAssemblyProvider AssemblyProvider => ServiceProperties.Get<IAssemblyProvider>();

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