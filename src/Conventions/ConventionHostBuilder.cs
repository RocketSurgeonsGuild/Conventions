using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionHostBuilder<TSelf> : IConventionHostBuilder<TSelf>
        where TSelf : ConventionHostBuilder<TSelf>, IConventionHostBuilder<TSelf>
    {
        public ConventionHostBuilder(
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IAssemblyProvider assemblyProvider,
            DiagnosticSource diagnosticSource,
            IDictionary<object, object> properties
        )
        {
            Scanner = scanner;
            AssemblyCandidateFinder = assemblyCandidateFinder;
            AssemblyProvider = assemblyProvider;
            DiagnosticSource = diagnosticSource;
            Properties = properties ?? new Dictionary<object, object>();
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual object this[object item]
        {
            get => Properties.TryGetValue(item, out object value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        public IDictionary<object, object> Properties { get; }

        public virtual IConventionScanner Scanner { get; }
        public virtual IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        public virtual IAssemblyProvider AssemblyProvider { get; }
        public virtual DiagnosticSource DiagnosticSource { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public TSelf AppendConvention(params IConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return (TSelf)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="conventions">The conventions.</param>
        /// <returns>The scanner</returns>
        public TSelf AppendConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return (TSelf)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>The scanner</returns>
        public TSelf PrependConvention(params IConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return (TSelf)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="conventions">The conventions.</param>
        /// <returns>The scanner</returns>
        public TSelf PrependConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return (TSelf)this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        public TSelf PrependDelegate(params Delegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return (TSelf)this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        public TSelf PrependDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return (TSelf)this;
        }


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>The scanner</returns>
        public TSelf AppendDelegate(params Delegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return (TSelf)this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <typeparam name="T">The scanner</typeparam>
        /// <param name="builder">The scanner.</param>
        /// <param name="delegates">The conventions.</param>
        /// <returns>The scanner</returns>
        public TSelf AppendDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return (TSelf)this;
        }
    }
}
