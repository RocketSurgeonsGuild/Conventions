using System;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Builders;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionBuilder<TBuilder, TConvention, TDelegate> : Builder, IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TBuilder : IBuilder
        where TConvention : IConvention
    {
        protected readonly IConventionScanner Scanner;

        protected ConventionBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder)
        {
            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(assemblyCandidateFinder));
            Scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        }

        protected abstract TBuilder GetBuilder();

        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        public TBuilder PrependDelegate(TDelegate @delegate)
        {
            Scanner.PrependDelegate(@delegate as Delegate);
            return GetBuilder();
        }

        public TBuilder PrependConvention(TConvention convention)
        {
            Scanner.PrependConvention(convention);
            return GetBuilder();
        }

        public TBuilder AppendDelegate(TDelegate @delegate)
        {
            Scanner.AppendDelegate(@delegate as Delegate);
            return GetBuilder();
        }

        public TBuilder AppendConvention(TConvention convention)
        {
            Scanner.AppendConvention(convention);
            return GetBuilder();
        }
    }
}
