using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal class TypeSymbolVisitor(Compilation compilation, ICompiledTypeFilter<INamedTypeSymbol> filter) : SymbolVisitor
{
    public static ImmutableArray<INamedTypeSymbol> GetTypes(Compilation compilation)
    {
        return GetTypes(compilation, new AlwaysMatchTypeFilter<INamedTypeSymbol>(), new AlwaysMatchTypeFilter<IAssemblySymbol>());
    }
    public static ImmutableArray<INamedTypeSymbol> GetTypes(Compilation compilation, ICompiledTypeFilter<IAssemblySymbol> assemblyFilter)
    {
        return GetTypes(compilation, new AlwaysMatchTypeFilter<INamedTypeSymbol>(), assemblyFilter);
    }

    public static ImmutableArray<INamedTypeSymbol> GetTypes(Compilation compilation, ICompiledTypeFilter<INamedTypeSymbol> filter)
    {
        return GetTypes(compilation, filter, new CompiledAssemblyFilter(ImmutableArray.Create<IAssemblyDescriptor>(new AllAssemblyDescriptor())));
    }

    public static ImmutableArray<INamedTypeSymbol> GetTypes(Compilation compilation, ICompiledTypeFilter<INamedTypeSymbol> filter, ICompiledTypeFilter<IAssemblySymbol> assemblyFilter)
    {
        var visitor = new TypeSymbolVisitor(compilation, filter);
        visitor.VisitNamespace(compilation.GlobalNamespace);
        foreach (var symbol in compilation.References.Select(compilation.GetAssemblyOrModuleSymbol))
        {
            switch (symbol)
            {
                case IAssemblySymbol assemblySymbol when assemblyFilter.IsMatch(compilation, assemblySymbol):
                    symbol.Accept(visitor);
                    break;
                case IModuleSymbol moduleSymbol when assemblyFilter.IsMatch(compilation, moduleSymbol.ContainingAssembly):
                    symbol.Accept(visitor);
                    break;
            }
        }

        return visitor.GetTypes();
    }

    private readonly HashSet<INamedTypeSymbol> _types = new(SymbolEqualityComparer.Default);

    private void Accept<T>(IEnumerable<T> members)
        where T : ISymbol?
    {
        foreach (var member in members)
        {
            member?.Accept(this);
        }
    }

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        Accept(symbol.GetMembers());
    }

    public override void VisitAssembly(IAssemblySymbol symbol)
    {
        symbol.GlobalNamespace.Accept(this);
    }

    public override void VisitNamedType(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind is TypeKind.Class or TypeKind.Delegate or TypeKind.Struct)
        {
            if (!symbol.CanBeReferencedByName) return;
            if (Helpers.GetBaseTypes(compilation, symbol).Contains(compilation.GetTypeByMetadataName("System.Attribute"), SymbolEqualityComparer.Default)) return;
            if (filter.IsMatch(compilation, symbol))
            {
                // symbol.IsAbstract ||
                _types.Add(symbol);
            }
        }

        Accept(symbol.GetMembers());
    }

    public ImmutableArray<INamedTypeSymbol> GetTypes() => _types.ToImmutableArray();
}
