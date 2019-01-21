using System.Collections.Generic;
using System.Diagnostics;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    public interface IConventionHostBuilder : IBuilder
    {
        IConventionScanner Scanner { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IAssemblyProvider AssemblyProvider { get; }
        DiagnosticSource DiagnosticSource { get; }
    }
}
