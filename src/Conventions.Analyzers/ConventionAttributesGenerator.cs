using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Support;

// ReSharper disable UnusedVariable
namespace Rocket.Surgery.Conventions;
// TODO: analyzers
//

/// <summary>
///     Generator to handle materializing conventions as code instead of loading them at runtime
/// </summary>
[Generator]
public class ConventionAttributesGenerator : IIncrementalGenerator
{
    private static IEnumerable<INamedTypeSymbol> GetExportedConventions(GeneratorAttributeSyntaxContext context)
    {
        foreach (var attribute in context.Attributes)
        {
            if (attribute is { AttributeClass.TypeArguments: [INamedTypeSymbol ta] })
                yield return ta;
            if (attribute is { ConstructorArguments: [{ Value: INamedTypeSymbol sv }] })
                yield return sv;
        }
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var exportConfiguration = ConventionConfigurationData.Create(context, "ExportConventions", ConventionConfigurationData.ExportsDefaults);

        var exportedConventions = context
                                 .SyntaxProvider
                                 .ForAttributeWithMetadataName(
                                      "Rocket.Surgery.Conventions.ExportConventionAttribute",
                                      (node, _) => node is TypeDeclarationSyntax,
                                      (syntaxContext, _) => (INamedTypeSymbol)syntaxContext.TargetSymbol
                                  )
                                 .WithComparer(SymbolEqualityComparer.Default);

        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(exportConfiguration)
               .Select((z, _) => ConventionAttributeData.Create(z.Right, z.Left))
               .Combine(exportedConventions.Collect()),
            static (productionContext, tuple) => ExportConventions.HandleConventionExports(
                productionContext,
                new(
                    tuple.Left,
                    tuple.Right.OrderBy(z => z.MetadataName).ToImmutableArray()
                )
            )
        );

        var importConfiguration = ConventionConfigurationData
                                 .Create(context, "ImportConventions", ConventionConfigurationData.ImportsDefaults)
                                 .Select((z, _) => z with { Assembly = z is not { WasConfigured: false, Assembly: true } && z.Assembly });

        var hasAssemblyLoadContext = context.CompilationProvider
                                            .Select((compilation, _) => compilation.GetTypeByMetadataName("System.Runtime.Loader.AssemblyLoadContext") is { });
        var msBuildConfig = context.AnalyzerConfigOptionsProvider
                                   .Select(
                                        (provider, _) => ( isTestProject: provider.GlobalOptions.TryGetValue(
                                                               "build_property.IsTestProject",
                                                               out var isTestProjectString
                                                           )
                                                        && bool.TryParse(isTestProjectString, out var isTestProject)
                                                        && isTestProject,
                                                           rootNamespace: provider.GlobalOptions.TryGetValue(
                                                               "build_property.RootNamespace",
                                                               out var rootNamespace
                                                           )
                                                               ? rootNamespace
                                                               : null )
                                    );
        var rootNamespace = context.AnalyzerConfigOptionsProvider
                                   .Select(
                                        (provider, _) => provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var value) ? value : ""
                                    );

        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(exportedConventions.Collect())
               .Combine(importConfiguration)
               .Combine(exportConfiguration)
               .Combine(hasAssemblyLoadContext)
               .Combine(msBuildConfig)
               .Select(
                    (z, _) => (
                        compilation: z.Left.Left.Left.Left.Left,
                        hasExports: z.Left.Left.Left.Left.Right.Any(),
                        exportedCandidates: z.Left.Left.Left.Left.Right,
                        importConfiguration: z.Left.Left.Left.Right, exportConfiguration: z.Left.Left.Right, hasAssemblyLoadContext: z.Left.Right,
                        msBuildConfig: z.Right
                    )
                ),
            static (productionContext, tuple) =>
            {
                if (!tuple.hasAssemblyLoadContext) return;
                ImportConventions.HandleConventionImports(
                    productionContext,
                    new(
                        tuple.compilation,
                        tuple.hasExports,
                        tuple.msBuildConfig,
                        tuple.importConfiguration,
                        tuple.exportConfiguration
                    )
                );
            }
        );
    }
}
