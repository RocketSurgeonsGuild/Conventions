using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal class FindTypeVisitor(Compilation compilation, ICompiledTypeFilter<IAssemblySymbol> assemblyFilter, string typeName) : TypeSymbolVisitorBase(
    compilation,
    assemblyFilter,
    new CompiledTypeFilter(ClassFilter.PublicOnly, ImmutableArray<ITypeFilterDescriptor>.Empty)
)
{
    private INamedTypeSymbol? _type;

    public static INamedTypeSymbol? FindType(Compilation compilation, IAssemblySymbol assemblySymbol, string typeName)
    {
        var visitor = new FindTypeVisitor(
            compilation,
            new CompiledAssemblyFilter(ImmutableArray.Create<IAssemblyDescriptor>(new AssemblyDescriptor(assemblySymbol))),
            typeName
        );
        visitor.Visit(assemblySymbol);
        return visitor._type;
    }

    protected override bool FoundNamedType(INamedTypeSymbol symbol)
    {
        if (typeName != Helpers.GetFullMetadataName(symbol)) return false;
        _type = symbol;
        return true;
    }
}