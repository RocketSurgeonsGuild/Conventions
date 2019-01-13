using System;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using System.Collections.Generic;
using System.Diagnostics;
using Rocket.Surgery.Builders;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionHostBuilder : Builder, IConventionHostBuilder
    {
        public ConventionHostBuilder(IConventionScanner scanner,
        IAssemblyCandidateFinder assemblyCandidateFinder,
        IAssemblyProvider assemblyProvider,
        DiagnosticSource diagnosticSource,
        IDictionary<object, object> properties
        ) : base(properties)
        {
            Scanner = scanner;
            AssemblyCandidateFinder = assemblyCandidateFinder;
            AssemblyProvider = assemblyProvider;
            DiagnosticSource = diagnosticSource;
        }

        public virtual IConventionScanner Scanner { get; }
        public virtual IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        public virtual IAssemblyProvider AssemblyProvider { get; }
        public virtual DiagnosticSource DiagnosticSource { get; }

        public void AppendConvention(IConvention convention)
        {
            this.Scanner.AppendConvention(convention);
        }
        public void AppendDelegate(Delegate @delegate)
        {
            this.Scanner.AppendDelegate(@delegate);
        }
        public void PrependConvention(IConvention convention)
        {
            this.Scanner.PrependConvention(convention);
        }
        public void PrependDelegate(Delegate @delegate)
        {
            this.Scanner.PrependDelegate(@delegate);
        }
    }
}
