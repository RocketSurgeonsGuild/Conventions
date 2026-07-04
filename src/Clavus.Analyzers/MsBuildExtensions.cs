using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Clavus;

internal static class MsBuildExtensions
{
    extension(AnalyzerConfigOptions options)
    {
        public T? GetBuildProperty<T>(string propertyName, Func<string, T?> transform) => options.TryGetValue($"build_property.{propertyName}", out var value) ? transform(value) : default;
    }
    extension(IAssemblySymbol assembly)
    {
        public ImmutableDictionary<string, TypedConstant> GetAssemblyMetadataAttributes(Func<string, bool> predicate)
            => assembly
              .GetAttributes()
              .Select(z => z is { AttributeClass.MetadataName: "AssemblyMetadataAttribute", ConstructorArguments: [{ Value: string { Length: > 0, } key, }, var value,], } ? (key, value) : default)
              .Where(z => z.key is { } && predicate(z.key))
              .ToImmutableDictionary(z => z.key, z => z.value);

    }

    extension(Compilation compilation)
    {
        public ImmutableList<ClavusConfigurationData> GetClavusReferences() => [.. compilation
                                                                               .References
                                                                               .Select(compilation.GetAssemblyOrModuleSymbol)
                                                                               .OfType<IAssemblySymbol>()
                                                                               .Select(symbol => ClavusConfigurationData.FromAssemblyAttributes(symbol, "Clavus.Exports."))
                                                                              .OfType<ClavusConfigurationData>()
                                                                               .OrderBy(z => z)];
    }
}
