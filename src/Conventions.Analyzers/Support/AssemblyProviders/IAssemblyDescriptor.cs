using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal interface IAssemblyDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct AssemblyDescriptor(IAssemblySymbol AssemblySymbol) : IAssemblyDescriptor
{
    public override string ToString()
    {
        return "Assembly: " + Helpers.GetFullMetadataName(AssemblySymbol);
    }
}

[DebuggerDisplay("{ToString()}")]
internal readonly record struct AllAssemblyDescriptor : IAssemblyDescriptor
{
    public override string ToString()
    {
        return "All";
    }
}

[DebuggerDisplay("{ToString()}")]
internal readonly record struct CompiledAssemblyDescriptor(IAssemblySymbol Assembly) : IAssemblyDescriptor
{
    public override string ToString()
    {
        return "CompiledAssembly of " + Helpers.GetFullMetadataName(Assembly);
    }
}

[DebuggerDisplay("{ToString()}")]
internal readonly record struct CompiledAssemblyDependenciesDescriptor(IAssemblySymbol Assembly) : IAssemblyDescriptor
{
    public override string ToString()
    {
        return "CompiledAssemblyDependencies of " + Helpers.GetFullMetadataName(Assembly);
    }
}