using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

interface ICompiledTypeFilter<TSymbol>
{
    bool IsMatch(Compilation compilation, TSymbol targetType);
}

class AlwaysMatchTypeFilter<TSymbol> : ICompiledTypeFilter<TSymbol>
{
    public bool IsMatch(Compilation compilation, TSymbol targetType) => true;
}

class CompiledAssemblyFilter
    (ImmutableArray<IAssemblyDescriptor> assemblyDescriptors) : ICompiledTypeFilter<IAssemblySymbol>
{
    private static readonly HashSet<string> _coreAssemblies =
    [
        "mscorlib",
        "netstandard",
        "System",
        "System.Core",
        "System.Runtime"
    ];

    public bool IsMatch(Compilation compilation, IAssemblySymbol targetType)
    {
        if (assemblyDescriptors.OfType<AllAssemblyDescriptor>().Any()) return true;
        if (_coreAssemblies.Contains(targetType.Name) && !assemblyDescriptors.OfType<IncludeSystemAssembliesDescriptor>().Any()) return false;

        return assemblyDescriptors
           .Any(
                filter => filter switch
                          {
                              AssemblyDescriptor { Assembly: var assembly } => SymbolEqualityComparer.Default.Equals(assembly, targetType),
                              AssemblyDependenciesDescriptor { Assembly: var assembly } => targetType
                                                                                          .Modules.SelectMany(z => z.ReferencedAssemblySymbols)
                                                                                          .Any(
                                                                                               reference => SymbolEqualityComparer.Default.Equals(
                                                                                                   assembly,
                                                                                                   reference
                                                                                               )
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

    public bool IsMatch(Compilation compilation, INamedTypeSymbol targetType)
    {
        if (Aborted || ( classFilter == ClassFilter.PublicOnly && targetType.DeclaredAccessibility != Accessibility.Public ))
            return false;

        if (typeFilterDescriptors.Length == 0) return true;

        return typeFilterDescriptors.All(GetFilterDescriptor);

        bool GetFilterDescriptor(ITypeFilterDescriptor filterDescriptor) =>
            filterDescriptor switch
            {
                AssignableToTypeFilterDescriptor { Type: var assignableToType } =>
                    Helpers.HasImplicitGenericConversion(compilation, assignableToType, targetType),
                NotAssignableToTypeFilterDescriptor { Type: var notAssignableToType } =>
                    !Helpers.HasImplicitGenericConversion(compilation, notAssignableToType, targetType),
                AssignableToAnyTypeFilterDescriptor { Types: var assignableToAnyTypes } =>
                    assignableToAnyTypes.Any(z => Helpers.HasImplicitGenericConversion(compilation, z, targetType)),
                NotAssignableToAnyTypeFilterDescriptor { Types: var notAssignableToAnyTypes } =>
                    notAssignableToAnyTypes.All(z => !Helpers.HasImplicitGenericConversion(compilation, z, targetType)),
                WithAttributeFilterDescriptor { Attribute: var attribute } =>
                    targetType.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, attribute)),
                WithoutAttributeFilterDescriptor { Attribute: var attribute } =>
                    targetType.GetAttributes().All(z => !SymbolEqualityComparer.Default.Equals(z.AttributeClass, attribute)),
                WithAttributeStringFilterDescriptor { AttributeClassName: var attribute } =>
                    targetType.GetAttributes().Any(z => Helpers.GetFullMetadataName(z.AttributeClass) == attribute),
                WithoutAttributeStringFilterDescriptor { AttributeClassName: var attribute } =>
                    targetType.GetAttributes().All(z => Helpers.GetFullMetadataName(z.AttributeClass) != attribute),
                NamespaceFilterDescriptor { Filter: var filterName, Namespaces: var filterNamespaces } =>
                    handleNamespaceFilter(filterName, filterNamespaces, targetType),
                NameFilterDescriptor { Filter: var filterName, Names: var filterNames } =>
                    handleNameFilter(filterName, filterNames, targetType),
                TypeKindFilterDescriptor { Include: var include, TypeKinds: var typeKinds } =>
                    handleKindFilter(include, typeKinds, targetType),
                _ => throw new NotSupportedException()
            };

        static bool handleNamespaceFilter(NamespaceFilter filterName, ImmutableHashSet<string> filterNamespaces, INamedTypeSymbol namedTypeSymbol1)
        {
            var ns = namedTypeSymbol1.ContainingNamespace.ToDisplayString();
            return filterName switch
                   {
                       NamespaceFilter.Exact => filterNamespaces.Contains(ns),
                       NamespaceFilter.In    => filterNamespaces.Any(n => ns.StartsWith(n)),
                       NamespaceFilter.NotIn => filterNamespaces.All(n => !ns.StartsWith(n)),
                       _                     => throw new NotImplementedException(),
                   };
        }

        static bool handleNameFilter(TextDirectionFilter filterName, ImmutableHashSet<string> filterNames, INamedTypeSymbol namedTypeSymbol2) =>
            filterName switch
            {
                TextDirectionFilter.Contains   => filterNames.Any(name => namedTypeSymbol2.Name.Contains(name)),
                TextDirectionFilter.StartsWith => filterNames.Any(name => namedTypeSymbol2.Name.StartsWith(name)),
                TextDirectionFilter.EndsWith   => filterNames.Any(name => namedTypeSymbol2.Name.EndsWith(name)),
                _                              => throw new NotImplementedException(),
            };

        static bool handleKindFilter(bool include, ImmutableHashSet<TypeKind> typeKinds, INamedTypeSymbol namedTypeSymbol2) =>
            include switch
            {
                true => typeKinds.Any(kind => namedTypeSymbol2.TypeKind == kind), false => typeKinds.All(kind => namedTypeSymbol2.TypeKind != kind),
            };
    }
}
