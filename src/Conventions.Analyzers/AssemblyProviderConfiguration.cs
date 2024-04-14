using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;
using Rocket.Surgery.Conventions.Support;

namespace Rocket.Surgery.Conventions;

internal static partial class AssemblyProviderConfiguration
{
    public static (ImmutableList<AssemblyCollection.Item> AssemblyRequests, ImmutableList<TypeCollection.Item> TypeRequests) FromAssemblyAttributes(
        Compilation compilation
    )
    {
        var assemblySymbols = compilation
                             .References.Select(compilation.GetAssemblyOrModuleSymbol)
                             .Concat([compilation.Assembly,])
                             .Select(
                                  symbol =>
                                  {
                                      if (symbol is IAssemblySymbol assemblySymbol)
                                          return assemblySymbol;
                                      if (symbol is IModuleSymbol moduleSymbol) return moduleSymbol.ContainingAssembly;
                                      // ReSharper disable once NullableWarningSuppressionIsUsed
                                      return null!;
                                  }
                              )
                             .Where(z => z is { })
                             .ToImmutableDictionary(z => z.MetadataName);

        var assemblyRequests = ImmutableList.CreateBuilder<AssemblyCollection.Item>();
        var typeRequests = ImmutableList.CreateBuilder<TypeCollection.Item>();

        foreach (var assembly in assemblySymbols.Values)
        {
            var attributes = assembly.GetAttributes();
            foreach (var attribute in attributes)
            {
                if (attribute is not { AttributeClass.MetadataName : "AssemblyMetadataAttribute", }) continue;
                try
                {
                    switch (attribute)
                    {
                        case { ConstructorArguments: [{ Value: getAssembliesKey, }, { Value: string getAssembliesData, },], }:
                            assemblyRequests.Add(GetAssembliesFromString(assemblySymbols, getAssembliesData));
                            break;
                        case { ConstructorArguments: [{ Value: getTypesKey, }, { Value: string getTypesData, },], }:
                            typeRequests.Add(GetTypesFromString(compilation, assemblySymbols, getTypesData));
                            break;
                    }
                }
                catch (InvalidOperationException)
                {
                    //?
                }
                catch (JsonException)
                {
                    //?
                }
            }
        }

        return ( assemblyRequests.ToImmutable(), typeRequests.ToImmutable() );
    }

    public static IEnumerable<AttributeListSyntax> FromAssemblyAttributes(
        ImmutableArray<AssemblyCollection.Item> assemblyRequests,
        ImmutableArray<TypeCollection.Item> typeRequests
    )
    {
        foreach (var request in assemblyRequests.OrderBy(z => z.Location.FilePath).ThenBy(z => z.Location.LineNumber).ThenBy(z => z.Location.MemberName))
        {
            // disallow list?
            if (request.Location.MemberName == "GetAssemblyConventions" && request.Location.FilePath.EndsWith("ConventionFactoryBase.cs"))
            {
                continue;
            }

            yield return Helpers.AddAssemblyAttribute(getAssembliesKey, GetAssembliesToString(request));
        }

        foreach (var request in typeRequests.OrderBy(z => z.Location.FilePath).ThenBy(z => z.Location.LineNumber).ThenBy(z => z.Location.MemberName))
        {
            yield return Helpers.AddAssemblyAttribute(getTypesKey, GetTypesToString(request));
        }
    }

    private const string getAssembliesKey = "Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies";
    private const string getTypesKey = "Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes";

    private static AssemblyCollection.Item GetAssembliesFromString(ImmutableDictionary<string, IAssemblySymbol> assemblySymbols, string value)
    {
        var result = DecompressString(value);
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var data = JsonSerializer.Deserialize(result, SourceGenerationContext.Default.AssemblyCollectionData)!;
        var assemblyFilter = LoadAssemblyFilter(data.Assembly, assemblySymbols);
        return new(data.Location, assemblyFilter);
    }

    private static string GetAssembliesToString(AssemblyCollection.Item item)
    {
        var data = new AssemblyCollectionData(item.Location, LoadAssemblyFilterData(item.AssemblyFilter));
        var result = JsonSerializer.SerializeToUtf8Bytes(data, SourceGenerationContext.Default.AssemblyCollectionData);
        return CompressString(result);
    }

    private static TypeCollection.Item GetTypesFromString(Compilation compilation, ImmutableDictionary<string, IAssemblySymbol> assemblySymbols, string value)
    {
        var result = DecompressString(value);
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var data = JsonSerializer.Deserialize(result, SourceGenerationContext.Default.TypeCollectionData)!;
        var assemblyFilter = LoadAssemblyFilter(data.Assembly, assemblySymbols);
        var typeFilter = LoadTypeFilter(compilation, data.Type, assemblySymbols);
        return new(data.Location, assemblyFilter, typeFilter);
    }

    private static string GetTypesToString(TypeCollection.Item item)
    {
        var data = new TypeCollectionData(item.Location, LoadAssemblyFilterData(item.AssemblyFilter), LoadTypeFilterData(item.AssemblyFilter, item.TypeFilter));
        var result = JsonSerializer.SerializeToUtf8Bytes(data, SourceGenerationContext.Default.TypeCollectionData);
        return CompressString(result);
    }

    private static byte[] DecompressString(string base64String)
    {
        return Convert.FromBase64String(base64String);
//        using var memoryStream = new MemoryStream(compressedBytes);
//        using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
//        using var decompressedMemoryStream = new MemoryStream();
//        gZipStream.CopyTo(decompressedMemoryStream);
//
//        var decompressedBytes = decompressedMemoryStream.ToArray();
//        return decompressedBytes;
    }

    private static string CompressString(byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
//        using var memory = new MemoryStream();
//        using var gZipStream = new GZipStream(memory, CompressionMode.Compress);
//        gZipStream.Write(bytes, 0, bytes.Length);
//        var compressedBytes = memory.ToArray();
//        return Convert.ToBase64String(compressedBytes);
    }

    private static GetAssembliesFilterData LoadAssemblyFilterData(CompiledAssemblyFilter filter)
    {
        return new(
            filter.AssemblyDescriptors.OfType<AllAssemblyDescriptor>().Any(),
            filter.AssemblyDescriptors.OfType<IncludeSystemAssembliesDescriptor>().Any(),
            filter.AssemblyDescriptors.OfType<AssemblyDescriptor>().Select(z => z.Assembly.MetadataName).OrderBy(z => z).ToImmutableArray(),
            filter.AssemblyDescriptors.OfType<NotAssemblyDescriptor>().Select(z => z.Assembly.MetadataName).OrderBy(z => z).ToImmutableArray(),
            filter.AssemblyDescriptors.OfType<AssemblyDependenciesDescriptor>().Select(z => z.Assembly.MetadataName).OrderBy(z => z).ToImmutableArray()
        );
    }

    private static GetTypesFilterData LoadTypeFilterData(CompiledAssemblyFilter assemblyFilter, CompiledTypeFilter typeFilter)
    {
        var assemblyData = LoadAssemblyFilterData(assemblyFilter);
        return new(
            assemblyData.AllAssembly,
            assemblyData.IncludeSystem,
            assemblyData.Assembly,
            assemblyData.NotAssembly,
            assemblyData.AssemblyDependencies,
            typeFilter.ClassFilter,
            typeFilter
               .TypeFilterDescriptors
               .OfType<NamespaceFilterDescriptor>()
               .Select(z => new NamespaceFilterData(z.Filter, z.Namespaces.OrderBy(z => z).ToImmutableArray()))
               .OrderBy(z => string.Join(",", z.Namespaces.OrderBy(static z => z)))
               .ThenBy(z => z.Filter)
               .Select(z => z with { Namespaces = z.Namespaces.OrderBy(z => z).ToImmutableArray(), })
               .ToImmutableArray(),
            typeFilter
               .TypeFilterDescriptors.OfType<NameFilterDescriptor>()
               .Select(z => new NameFilterData(z.Filter, z.Names.OrderBy(z => z).ToImmutableArray()))
               .OrderBy(z => string.Join(",", z.Names.OrderBy(static z => z)))
               .ThenBy(z => z.Filter)
               .ToImmutableArray(),
            typeFilter
               .TypeFilterDescriptors.OfType<TypeKindFilterDescriptor>()
               .Select(z => new TypeKindFilterData(z.Include, z.TypeKinds.OrderBy(z => z).ToImmutableArray()))
               .OrderBy(z => string.Join(",", z.TypeKinds.OrderBy(static z => z)))
               .ThenBy(z => z.Include)
               .ToImmutableArray(),
            typeFilter
               .TypeFilterDescriptors.OfType<TypeInfoFilterDescriptor>()
               .Select(z => new TypeInfoFilterData(z.Include, z.TypeInfos.OrderBy(z => z).ToImmutableArray()))
               .OrderBy(z => string.Join(",", z.TypeInfos.OrderBy(static z => z)))
               .ThenBy(z => z.Include)
               .ToImmutableArray(),
            typeFilter
               .TypeFilterDescriptors
               .Select(
                    f => f switch
                         {
                             WithAttributeFilterDescriptor descriptor => new(
                                 true,
                                 descriptor.Attribute.ContainingAssembly.MetadataName,
                                 Helpers.GetFullMetadataName(descriptor.Attribute)
                             ),
                             WithoutAttributeFilterDescriptor descriptor => new WithAttributeData(
                                 false,
                                 descriptor.Attribute.ContainingAssembly.MetadataName,
                                 Helpers.GetFullMetadataName(descriptor.Attribute)
                             ),
                             _ => null!,
                         }
                )
               .Where(z => z is { })
               .OrderBy(z => z.Assembly)
               .ThenBy(z => z.Attribute)
               .ThenBy(z => z.Include)
               .ToImmutableArray(),
            typeFilter
               .TypeFilterDescriptors
               .Select(
                    f => f switch
                         {
                             WithAttributeStringFilterDescriptor descriptor    => new(true, descriptor.AttributeClassName),
                             WithoutAttributeStringFilterDescriptor descriptor => new WithAttributeStringData(false, descriptor.AttributeClassName),
                             _                                                 => null!,
                         }
                )
               .Where(z => z is { })
               .OrderBy(z => z.Attribute)
               .ThenBy(z => z.Include)
               .ToImmutableArray(),
            typeFilter
               .TypeFilterDescriptors
               .Select(
                    f => f switch
                         {
                             AssignableToTypeFilterDescriptor descriptor => new(
                                 true,
                                 descriptor.Type.ContainingAssembly.MetadataName,
                                 Helpers.GetFullMetadataName(descriptor.Type)
                             ),
                             NotAssignableToTypeFilterDescriptor descriptor => new AssignableToTypeData(
                                 false,
                                 descriptor.Type.ContainingAssembly.MetadataName,
                                 Helpers.GetFullMetadataName(descriptor.Type)
                             ),
                             _ => null!,
                         }
                )
               .Where(z => z is { })
               .OrderBy(z => z.Assembly)
               .ThenBy(z => z.Type)
               .ThenBy(z => z.Include)
               .ToImmutableArray(),
            typeFilter
               .TypeFilterDescriptors
               .Select(
                    f => f switch
                         {
                             AssignableToAnyTypeFilterDescriptor descriptor => new(
                                 true,
                                 descriptor
                                    .Types.Select(z => new AnyTypeData(z.ContainingAssembly.MetadataName, Helpers.GetFullMetadataName(z)))
                                    .OrderBy(z => z.Assembly)
                                    .ThenBy(z => z.Type)
                                    .ToImmutableArray()
                             ),
                             NotAssignableToAnyTypeFilterDescriptor descriptor => new AssignableToAnyTypeData(
                                 false,
                                 descriptor
                                    .Types
                                    .Select(z => new AnyTypeData(z.ContainingAssembly.MetadataName, Helpers.GetFullMetadataName(z)))
                                    .OrderBy(z => z.Assembly)
                                    .ThenBy(z => z.Type)
                                    .ToImmutableArray()
                             ),
                             _ => null!,
                         }
                )
               .Where(z => z is { })
               .OrderBy(z => string.Join(",", z.Types))
               .ThenBy(z => z.Include)
               .ToImmutableArray()
        );
    }

    private static CompiledAssemblyFilter LoadAssemblyFilter(GetAssembliesFilterData data, ImmutableDictionary<string, IAssemblySymbol> assemblySymbols)
    {
        var descriptors = ImmutableArray.CreateBuilder<IAssemblyDescriptor>();
        if (data.AllAssembly)
        {
            descriptors.Add(new AllAssemblyDescriptor());
        }

        if (data.IncludeSystem)
        {
            descriptors.Add(new IncludeSystemAssembliesDescriptor());
        }

        foreach (var item in data.Assembly)
        {
            if (assemblySymbols.TryGetValue(item, out var assembly)) descriptors.Add(new AssemblyDescriptor(assembly));
        }

        foreach (var item in data.NotAssembly)
        {
            if (assemblySymbols.TryGetValue(item, out var assembly)) descriptors.Add(new NotAssemblyDescriptor(assembly));
        }

        foreach (var item in data.AssemblyDependencies)
        {
            if (assemblySymbols.TryGetValue(item, out var assembly)) descriptors.Add(new AssemblyDependenciesDescriptor(assembly));
        }

        return new(descriptors.ToImmutable());
    }

    private static CompiledTypeFilter LoadTypeFilter(
        Compilation compilation,
        GetTypesFilterData data,
        ImmutableDictionary<string, IAssemblySymbol> assemblySymbols
    )
    {
        var descriptors = ImmutableArray.CreateBuilder<ITypeFilterDescriptor>();
        foreach (var item in data.NamespaceFilters) descriptors.Add(new NamespaceFilterDescriptor(item.Filter, item.Namespaces.ToImmutableHashSet()));
        foreach (var item in data.NameFilters) descriptors.Add(new NameFilterDescriptor(item.Filter, item.Names.ToImmutableHashSet()));
        foreach (var item in data.TypeKindFilters) descriptors.Add(new TypeKindFilterDescriptor(item.Include, item.TypeKinds.ToImmutableHashSet()));
        foreach (var item in data.TypeInfoFilters) descriptors.Add(new TypeInfoFilterDescriptor(item.Include, item.TypeInfos.ToImmutableHashSet()));

        foreach (var item in data.WithAttributeFilters)
        {
            if (!assemblySymbols.TryGetValue(item.Assembly, out var assemblySymbol)) continue;
            if (FindTypeVisitor.FindType(compilation, assemblySymbol, item.Attribute) is not { } type) continue;
            descriptors.Add(item.Include ? new WithAttributeFilterDescriptor(type) : new WithoutAttributeFilterDescriptor(type));
        }

        foreach (var item in data.WithAttributeStringFilters)
        {
            descriptors.Add(
                item.Include ? new WithAttributeStringFilterDescriptor(item.Attribute) : new WithoutAttributeStringFilterDescriptor(item.Attribute)
            );
        }

        foreach (var item in data.AssignableToTypeFilters)
        {
            if (!assemblySymbols.TryGetValue(item.Assembly, out var assemblySymbol)) continue;
            if (FindTypeVisitor.FindType(compilation, assemblySymbol, item.Type) is not { } type) continue;
            descriptors.Add(item.Include ? new AssignableToTypeFilterDescriptor(type) : new NotAssignableToTypeFilterDescriptor(type));
        }

        foreach (var item in data.AssignableToAnyTypeFilters)
        {
            var filters = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            foreach (var type in item.Types)
            {
                if (!assemblySymbols.TryGetValue(type.Assembly, out var assemblySymbol)) continue;
                if (FindTypeVisitor.FindType(compilation, assemblySymbol, type.Type) is not { } t) continue;
                filters.Add(t);
            }

            descriptors.Add(
                item.Include
                    ? new AssignableToAnyTypeFilterDescriptor(filters.ToImmutable())
                    : new NotAssignableToAnyTypeFilterDescriptor(filters.ToImmutable())
            );
        }

        return new(data.Filter, descriptors.ToImmutable());
    }

    [JsonSourceGenerationOptions]
    [JsonSerializable(typeof(AssemblyCollectionData))]
    [JsonSerializable(typeof(TypeCollectionData))]
    [JsonSerializable(typeof(GetAssembliesFilterData))]
    [JsonSerializable(typeof(GetTypesFilterData))]
    [JsonSerializable(typeof(NamespaceFilterData))]
    [JsonSerializable(typeof(NameFilterData))]
    [JsonSerializable(typeof(TypeKindFilterData))]
    [JsonSerializable(typeof(TypeInfoFilterData))]
    [JsonSerializable(typeof(WithAttributeData))]
    [JsonSerializable(typeof(WithAttributeStringData))]
    [JsonSerializable(typeof(AssignableToTypeData))]
    [JsonSerializable(typeof(AssignableToAnyTypeData))]
    private partial class SourceGenerationContext : JsonSerializerContext;

    private record AssemblyCollectionData
    (
        [property: JsonPropertyName("l")]
        SourceLocation Location,
        [property: JsonPropertyName("a")]
        GetAssembliesFilterData Assembly
    );

    private record TypeCollectionData
    (
        [property: JsonPropertyName("l")]
        SourceLocation Location,
        [property: JsonPropertyName("a")]
        GetAssembliesFilterData Assembly,
        [property: JsonPropertyName("t")]
        GetTypesFilterData Type
    );

    private record GetAssembliesFilterData
    (
        [property: JsonPropertyName("a")]
        bool AllAssembly,
        [property: JsonPropertyName("i")]
        bool IncludeSystem,
        [property: JsonPropertyName("m")]
        ImmutableArray<string> Assembly,
        [property: JsonPropertyName("na")]
        ImmutableArray<string> NotAssembly,
        [property: JsonPropertyName("d")]
        ImmutableArray<string> AssemblyDependencies
    );

    private record GetTypesFilterData
    (
        bool AllAssembly,
        bool IncludeSystem,
        ImmutableArray<string> Assembly,
        ImmutableArray<string> NotAssembly,
        ImmutableArray<string> AssemblyDependencies,
        [property: JsonPropertyName("f")]
        ClassFilter Filter,
        [property: JsonPropertyName("nsf")]
        ImmutableArray<NamespaceFilterData> NamespaceFilters,
        [property: JsonPropertyName("nf")]
        ImmutableArray<NameFilterData> NameFilters,
        [property: JsonPropertyName("tk")]
        ImmutableArray<TypeKindFilterData> TypeKindFilters,
        [property: JsonPropertyName("ti")]
        ImmutableArray<TypeInfoFilterData> TypeInfoFilters,
        [property: JsonPropertyName("w")]
        ImmutableArray<WithAttributeData> WithAttributeFilters,
        [property: JsonPropertyName("s")]
        ImmutableArray<WithAttributeStringData> WithAttributeStringFilters,
        [property: JsonPropertyName("at")]
        ImmutableArray<AssignableToTypeData> AssignableToTypeFilters,
        [property: JsonPropertyName("ta")]
        ImmutableArray<AssignableToAnyTypeData> AssignableToAnyTypeFilters
    ) : GetAssembliesFilterData(AllAssembly, IncludeSystem, Assembly, NotAssembly, AssemblyDependencies);

    internal record WithAttributeData
    (
        [property: JsonPropertyName("i")]
        bool Include,
        [property: JsonPropertyName("a")]
        string Assembly,
        [property: JsonPropertyName("b")]
        string Attribute);

    internal record WithAttributeStringData
    (
        [property: JsonPropertyName("i")]
        bool Include,
        [property: JsonPropertyName("b")]
        string Attribute);

    internal record AssignableToTypeData
    (
        [property: JsonPropertyName("i")]
        bool Include,
        [property: JsonPropertyName("a")]
        string Assembly,
        [property: JsonPropertyName("t")]
        string Type);

    internal record AssignableToAnyTypeData
    (
        [property: JsonPropertyName("i")]
        bool Include,
        [property: JsonPropertyName("t")]
        ImmutableArray<AnyTypeData> Types);

    internal record AnyTypeData
    (
        [property: JsonPropertyName("a")]
        string Assembly,
        [property: JsonPropertyName("t")]
        string Type);


    internal record NamespaceFilterData
    (
        [property: JsonPropertyName("f")]
        NamespaceFilter Filter,
        [property: JsonPropertyName("n")]
        ImmutableArray<string> Namespaces);

    internal record NameFilterData
    (
        [property: JsonPropertyName("f")]
        TextDirectionFilter Filter,
        [property: JsonPropertyName("n")]
        ImmutableArray<string> Names);

    internal record TypeKindFilterData
    (
        [property: JsonPropertyName("f")]
        bool Include,
        [property: JsonPropertyName("t")]
        ImmutableArray<TypeKind> TypeKinds);

    internal record TypeInfoFilterData
    (
        [property: JsonPropertyName("f")]
        bool Include,
        [property: JsonPropertyName("t")]
        ImmutableArray<TypeInfoFilter> TypeInfos);
}