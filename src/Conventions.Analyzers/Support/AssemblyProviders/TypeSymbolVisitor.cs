using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal abstract class TypeSymbolVisitorBase
    (Compilation compilation, ICompiledTypeFilter<IAssemblySymbol> assemblyFilter, ICompiledTypeFilter<INamedTypeSymbol> typeFilter) : SymbolVisitor
{
    private bool _abort;

    private void Accept<T>(IEnumerable<T> members)
        where T : ISymbol?
    {
        foreach (var member in members)
        {
            if (_abort) return;
            member?.Accept(this);
        }
    }

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        if (_abort) return;
        Accept(symbol.GetMembers());
    }

    public override void VisitAssembly(IAssemblySymbol symbol)
    {
        if (_abort) return;
        if (!assemblyFilter.IsMatch(compilation, symbol)) return;
        symbol.GlobalNamespace.Accept(this);
    }

    public override void VisitNamedType(INamedTypeSymbol symbol)
    {
        if (_abort) return;
        if (!symbol.CanBeReferencedByName) return;
        if (!typeFilter.IsMatch(compilation, symbol)) return;
        _abort = FoundNamedType(symbol);

        if (_abort) return;
        Accept(symbol.GetMembers());
    }

    protected abstract bool FoundNamedType(INamedTypeSymbol symbol);
}

internal class FindTypeInAssembly(Compilation compilation, ICompiledTypeFilter<IAssemblySymbol> assemblyFilter) : TypeSymbolVisitorBase(
    compilation,
    assemblyFilter,
    new CompiledTypeFilter(ClassFilter.PublicOnly, ImmutableArray<ITypeFilterDescriptor>.Empty)
)
{
    private INamedTypeSymbol? _type;

    public static INamedTypeSymbol? FindType(Compilation compilation, IAssemblySymbol assemblySymbol)
    {
        var visitor = new FindTypeInAssembly(
            compilation,
            new CompiledAssemblyFilter(ImmutableArray.Create<IAssemblyDescriptor>(new AssemblyDescriptor(assemblySymbol)))
        );
        visitor.Visit(assemblySymbol);
        return visitor._type;
    }

    protected override bool FoundNamedType(INamedTypeSymbol symbol)
    {
        _type = symbol;
        return true;
    }
}

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
        if (typeName != symbol.ToDisplayString()) return true;
        _type = symbol;
        return false;
    }
}

internal class TypeSymbolVisitor
    (Compilation compilation, ICompiledTypeFilter<IAssemblySymbol> assemblyFilter, ICompiledTypeFilter<INamedTypeSymbol> typeFilter)
    : TypeSymbolVisitorBase(compilation, assemblyFilter, typeFilter)
{
    public static ImmutableArray<INamedTypeSymbol> GetTypes(
        Compilation compilation,
        ICompiledTypeFilter<IAssemblySymbol> assemblyFilter,
        ICompiledTypeFilter<INamedTypeSymbol> typeFilter
    )
    {
        var visitor = new TypeSymbolVisitor(compilation, assemblyFilter, typeFilter);
        foreach (var symbol in compilation.References.Select(compilation.GetAssemblyOrModuleSymbol).Concat([compilation.Assembly]))
        {
            switch (symbol)
            {
                case IAssemblySymbol:
                    symbol.Accept(visitor);
                    break;
                case IModuleSymbol:
                    symbol.Accept(visitor);
                    break;
            }
        }

        return visitor.GetTypes();
    }

    private readonly HashSet<INamedTypeSymbol> _types = new(SymbolEqualityComparer.Default);

    protected override bool FoundNamedType(INamedTypeSymbol symbol)
    {
        _types.Add(symbol);
        return false;
    }

    public ImmutableArray<INamedTypeSymbol> GetTypes() => _types.ToImmutableArray();
}
