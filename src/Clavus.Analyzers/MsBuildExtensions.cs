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
        {
            var attributes = assembly.GetAttributes();
            return attributes
                .Select(z => z is { AttributeClass.MetadataName: "AssemblyMetadataAttribute", ConstructorArguments: [{ Value: string { Length: > 0, } key, }, var value,], } ? (key, value) : default)
                .Where(z => z.key is { } && predicate(z.key))
                .ToImmutableDictionary(z => z.key, z => z.value);
        }
    }

    extension(Compilation compilation)
    {
        public IEnumerable<ClavusConfigurationData> GetClavusReferences() => compilation.References
            .Select(compilation.GetAssemblyOrModuleSymbol)
            .OfType<IAssemblySymbol>()
            .Select(symbol => ClavusConfigurationData.FromAssemblyAttributes(symbol, "Export"))
            .OfType<ClavusConfigurationData>()
            .OrderBy(z => z);

        /// <summary>
        ///     Reads <c>[assembly: Clavus.ConfigurationAssembly(...)]</c> markers off every referenced assembly,
        ///     reusing the same reference-walk mechanism as <see cref="GetClavusReferences" /> (task 2.7). The
        ///     assembly's simple name is paired with each of its declared configuration entries so the manifest can
        ///     report which assembly contributed which file.
        /// </summary>
        public ImmutableArray<(string AssemblyName, string Name, string RelativePath)> GetClavusConfigurationReferences() => [
            ..compilation
               .References
               .Select(compilation.GetAssemblyOrModuleSymbol)
               .OfType<IAssemblySymbol>()
               .SelectMany(
                    symbol => symbol
                             .GetAttributes()
                             .Where(a => a.AttributeClass?.ToDisplayString() == "Clavus.ConfigurationAssemblyAttribute")
                             .Select(a => (symbol.Name, a))
                )
               .Select(
                    pair => (
                        AssemblyName: pair.Name,
                        Name: pair.a.ConstructorArguments is [{ Value: string n, }, ..] ? n : "",
                        RelativePath: pair.a.ConstructorArguments is [_, { Value: string p, },] ? p : ""
                    )
                )
               .Where(z => z.Name.Length > 0)
               .OrderBy(z => z.AssemblyName, StringComparer.Ordinal)
               .ThenBy(z => z.Name, StringComparer.Ordinal),
        ];
    }
}
