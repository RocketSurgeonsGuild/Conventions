using System;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionBuilder<TBuilder, TConvention, TDelegate> : ConventionContainerBuilder<TBuilder, TConvention, TDelegate>, IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TBuilder : IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        protected ConventionBuilder(
            IRocketEnvironment environment,
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IDictionary<object, object> properties
        ) : base(environment, scanner, properties)
        {
            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(assemblyCandidateFinder));
        }

        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
    }
}
