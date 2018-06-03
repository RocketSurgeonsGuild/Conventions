using System;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Builders;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionBuilder<TBuilder, TConvention, TDelegate> : ConventionContainerBuilder<TBuilder, TConvention, TDelegate>, IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TBuilder : IBuilder
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        protected ConventionBuilder(IConventionScanner scanner, IAssemblyProvider assemblyProvider, IAssemblyCandidateFinder assemblyCandidateFinder) : base(scanner)
        {
            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(assemblyCandidateFinder));
        }

        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
    }
}
