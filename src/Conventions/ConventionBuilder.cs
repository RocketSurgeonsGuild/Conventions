using System;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Builders;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionBuilder<TBuilder, TConvention, TDelegate> : ConventionContainerBuilder<TBuilder, TConvention, TDelegate>, IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TBuilder : IBuilder
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        protected ConventionBuilder(IConventionScanner scanner, IAssemblyProvider assemblyProvider, IAssemblyCandidateFinder assemblyCandidateFinder, IDictionary<object, object> properties) : base(scanner, properties)
        {
            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(assemblyCandidateFinder));
        }

        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
    }
}
