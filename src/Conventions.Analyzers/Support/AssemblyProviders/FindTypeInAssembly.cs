using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal class FindTypeInAssembly(Compilation compilation, ICompiledTypeFilter<IAssemblySymbol> assemblyFilter) : TypeSymbolVisitorBase(
    compilation,
    assemblyFilter,
    new CompiledTypeFilter(ClassFilter.PublicOnly, ImmutableArray<ITypeFilterDescriptor>.Empty)
)
{
    public static INamedTypeSymbol? FindType(Compilation compilation, IAssemblySymbol assemblySymbol)
    {
        var visitor = new FindTypeInAssembly(
            compilation,
            new CompiledAssemblyFilter(ImmutableArray.Create<IAssemblyDescriptor>(new AssemblyDescriptor(assemblySymbol)))
        );
        visitor.Visit(assemblySymbol);
        return visitor._type;
    }

    private INamedTypeSymbol? _type;

    protected override bool FoundNamedType(INamedTypeSymbol symbol)
    {
        _type = symbol;
        return true;
    }
}