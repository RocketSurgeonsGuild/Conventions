using System;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ConventionBuilder.
    /// Implements the <see cref="Rocket.Surgery.Conventions.ConventionContainerBuilder{TBuilder, TConvention, TDelegate}" />
    /// Implements the <see cref="Rocket.Surgery.Conventions.IConventionBuilder{TBuilder, TConvention, TDelegate}" />
    /// </summary>
    /// <typeparam name="TBuilder">The type of the t builder.</typeparam>
    /// <typeparam name="TConvention">The type of the t convention.</typeparam>
    /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
    /// <seealso cref="Rocket.Surgery.Conventions.ConventionContainerBuilder{TBuilder, TConvention, TDelegate}" />
    /// <seealso cref="Rocket.Surgery.Conventions.IConventionBuilder{TBuilder, TConvention, TDelegate}" />
    public abstract class ConventionBuilder<TBuilder, TConvention, TDelegate> : ConventionContainerBuilder<TBuilder, TConvention, TDelegate>, IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TBuilder : IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        protected ConventionBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IDictionary<object, object> properties
        ) : base(scanner, properties)
        {
            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(assemblyCandidateFinder));

            if (!Properties.TryGetValue(typeof(IAssemblyProvider), out var _))
                Properties[typeof(IAssemblyProvider)] = AssemblyProvider;

            if (!Properties.TryGetValue(typeof(IAssemblyCandidateFinder), out var _))
                Properties[typeof(IAssemblyCandidateFinder)] = AssemblyCandidateFinder;
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
