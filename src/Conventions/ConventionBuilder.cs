using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ConventionBuilder.
    /// Implements the <see cref="ConventionContainerBuilder{TBuilder, TConvention, TDelegate}" />
    /// Implements the <see cref="IConventionBuilder{TBuilder, TConvention, TDelegate}" />
    /// </summary>
    /// <typeparam name="TBuilder">The type of the t builder.</typeparam>
    /// <typeparam name="TConvention">The type of the t convention.</typeparam>
    /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
    /// <seealso cref="ConventionContainerBuilder{TBuilder, TConvention, TDelegate}" />
    /// <seealso cref="IConventionBuilder{TBuilder, TConvention, TDelegate}" />
    public abstract class ConventionBuilder<TBuilder, TConvention, TDelegate> :
        ConventionContainerBuilder<TBuilder, TConvention, TDelegate>,
        IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TBuilder : IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionBuilder{TBuilder, TConvention, TDelegate}" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">
        /// assemblyProvider
        /// or
        /// assemblyCandidateFinder
        /// </exception>
        protected ConventionBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IDictionary<object, object?> properties
        ) : base(scanner, properties)
        {
            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ??
                throw new ArgumentNullException(nameof(assemblyCandidateFinder));

            if (!Properties.TryGetValue(typeof(IAssemblyProvider), out var _))
            {
                Properties[typeof(IAssemblyProvider)] = AssemblyProvider;
            }

            if (!Properties.TryGetValue(typeof(IAssemblyCandidateFinder), out var _))
            {
                Properties[typeof(IAssemblyCandidateFinder)] = AssemblyCandidateFinder;
            }
        }

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        public IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
    }
}