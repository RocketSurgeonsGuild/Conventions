using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

interface ICompiledTypeFilter<TSymbol>
{
    bool IsMatch(Compilation compilation, TSymbol targetAssembly);
}

class AlwaysMatchTypeFilter<TSymbol> : ICompiledTypeFilter<TSymbol>
{
    public bool IsMatch(Compilation compilation, TSymbol targetAssembly) => true;
}

class CompiledAssemblyFilter
    (ImmutableArray<IAssemblyDescriptor> assemblyDescriptors) : ICompiledTypeFilter<IAssemblySymbol>
{
    public bool IsMatch(Compilation compilation, IAssemblySymbol targetAssembly)
    {
        if (assemblyDescriptors.OfType<AllAssemblyDescriptor>().Any()) return true;

        return assemblyDescriptors
           .Any(
                filter => filter switch
                          {
                              AssemblyDescriptor { Assembly: var assembly } => SymbolEqualityComparer.Default.Equals(assembly, targetAssembly),

                              CompiledAssemblyDescriptor { Assembly: var assembly } => SymbolEqualityComparer.Default.Equals(assembly, targetAssembly),
                              CompiledAssemblyDependenciesDescriptor { Assembly: var assembly } => targetAssembly.ContainingModule.ReferencedAssemblySymbols.Any(
                                  reference => SymbolEqualityComparer.Default.Equals(assembly, reference)
                                  ),
                              _ => false
                          }
            );
    }
}

class CompiledTypeFilter
    (ClassFilter classFilter, ImmutableArray<ITypeFilterDescriptor> typeFilterDescriptors) : ICompiledTypeFilter<INamedTypeSymbol>
{
    public bool Aborted { get; } = typeFilterDescriptors.OfType<CompiledAbortTypeFilterDescriptor>().Any();

    public bool IsMatch(Compilation compilation, INamedTypeSymbol targetAssembly)
    {
        if (Aborted || ( classFilter == ClassFilter.PublicOnly && targetAssembly.DeclaredAccessibility != Accessibility.Public ))
            return false;

        if (typeFilterDescriptors.Length == 0) return true;

        return typeFilterDescriptors
           .Any(
                filter => filter switch
                          {
                              CompiledAssignableToTypeFilterDescriptor { Type: var assignableToType } =>
                                  Helpers.HasImplicitGenericConversion(compilation, assignableToType, targetAssembly),
                              CompiledAssignableToAnyTypeFilterDescriptor { Type: var assignableToAnyType } =>
                                  Helpers.HasImplicitGenericConversion(compilation, assignableToAnyType, targetAssembly),
                              CompiledWithAttributeFilterDescriptor { Attribute: var attribute } =>
                                  targetAssembly.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, attribute)),
                              CompiledWithoutAttributeFilterDescriptor { Attribute: var attribute } =>
                                  !targetAssembly.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, attribute)),
                              NamespaceFilterDescriptor { Filter: var filterName, Namespaces: var filterNamespaces } =>
                                  handleNamespaceFilter(filterName, filterNamespaces, targetAssembly),
                              NameFilterDescriptor { Filter: var filterName, Names: var filterNames } =>
                                  handleNameFilter(filterName, filterNames, targetAssembly),
                              _ => false
                          }
            );

        static bool handleNamespaceFilter(NamespaceFilter filterName, ImmutableHashSet<string> filterNamespaces, INamedTypeSymbol namedTypeSymbol1)
        {
            var ns = namedTypeSymbol1.ContainingNamespace.ToDisplayString();
            return filterName switch
                   {
                       NamespaceFilter.Exact => filterNamespaces.Contains(ns),
                       NamespaceFilter.In    => filterNamespaces.Any(n => ns.StartsWith(n)),
                       NamespaceFilter.NotIn => !filterNamespaces.Any(n => ns.StartsWith(n)),
                       _                     => throw new NotImplementedException(),
                   };
        }

        static bool handleNameFilter(TextDirectionFilter filterName, ImmutableHashSet<string> filterNames, INamedTypeSymbol namedTypeSymbol2)
        {
            return filterName switch
                   {
                       TextDirectionFilter.Contains   => filterNames.Any(name => namedTypeSymbol2.Name.Contains(name)),
                       TextDirectionFilter.StartsWith => filterNames.Any(name => namedTypeSymbol2.Name.StartsWith(name)),
                       TextDirectionFilter.EndsWith   => filterNames.Any(name => namedTypeSymbol2.Name.EndsWith(name)),
                       _                              => throw new NotImplementedException(),
                   };
        }
    }
}
