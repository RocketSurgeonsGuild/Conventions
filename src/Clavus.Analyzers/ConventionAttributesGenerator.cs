using Clavus.Support;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable UnusedVariable
namespace Clavus;
// TODO: analyzers
//

/// <summary>
///     Generator to handle materializing conventions as code instead of loading them at runtime
/// </summary>
[Generator]
public class ClavusAttributesGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var exportConfiguration = ClavusConfigurationData.Create(context, "ExportClavus", "ExportClavusParts", ClavusConfigurationData.ExportsDefaults);

        var exportedConventions = context
                                 .SyntaxProvider
                                 .ForAttributeWithMetadataName(
                                      "Clavus.ExportClavusPartAttribute",
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
                    [.. tuple.Right.OrderBy(z => z.MetadataName)]
                )
            )
        );

        var importConfiguration = ClavusConfigurationData
                                 .Create(context, "ImportClavus", "ImportClavusParts", ClavusConfigurationData.ImportsDefaults)
                                 .Select((z, _) => z with { Assembly = z is not { WasConfigured: false, Assembly: true } && z.Assembly });

        var hasAssemblyLoadContext = context.CompilationProvider
                                            .Select((compilation, _) => compilation.GetTypeByMetadataName("System.Runtime.Loader.AssemblyLoadContext") is { });

        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(exportedConventions.Collect())
               .Combine(importConfiguration)
               .Combine(exportConfiguration)
               .Combine(hasAssemblyLoadContext)
               .Select(
                    (z, _) => (
                        compilation: z.Left.Left.Left.Left,
                        hasExports: z.Left.Left.Left.Right.Any(),
                        exportedCandidates: z.Left.Left.Left.Right,
                        importConfiguration: z.Left.Left.Right,
                        exportConfiguration: z.Left.Right,
                        hasAssemblyLoadContext: z.Right,
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
                        tuple.importConfiguration,
                        tuple.exportConfiguration
                    )
                );
            }
        );
    }
}
