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
            if (attribute is { AttributeClass.TypeArguments: [INamedTypeSymbol ta,], })
                yield return ta;
            if (attribute is { ConstructorArguments: [{ Value: INamedTypeSymbol sv, },], })
                yield return sv;
        }
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var exportConfigurationCandidate = ConventionConfigurationData.Create(context, "ExportConventions", ConventionConfigurationData.ExportsDefaults);

        var exportCandidates = context
                              .SyntaxProvider
                              .ForAttributeWithMetadataName(
                                   "Rocket.Surgery.Conventions.ConventionAttribute",
                                   (_, _) => true,
                                   (syntaxContext, _) => GetExportedConventions(syntaxContext)
                               )
                              .SelectMany((z, _) => z)
                              .WithComparer(SymbolEqualityComparer.Default);
        var exportCandidates2 = context
                               .SyntaxProvider
                               .ForAttributeWithMetadataName(
                                    "Rocket.Surgery.Conventions.ConventionAttribute`1",
                                    (_, _) => true,
                                    (syntaxContext, _) => GetExportedConventions(syntaxContext)
                                )
                               .SelectMany((z, _) => z)
                               .WithComparer(SymbolEqualityComparer.Default);

        var exportedConventions = context
                                 .SyntaxProvider
                                 .ForAttributeWithMetadataName(
                                      "Rocket.Surgery.Conventions.ExportConventionAttribute",
                                      (node, _) => node is TypeDeclarationSyntax,
                                      (syntaxContext, _) => (INamedTypeSymbol)syntaxContext.TargetSymbol
                                  )
                                 .WithComparer(SymbolEqualityComparer.Default);

        var combinedExports = exportCandidates
                             .Collect()
                             .Combine(exportCandidates2.Collect())
                             .SelectMany((tuple, _) => tuple.Left.AddRange(tuple.Right))
                             .Collect()
                             .Combine(exportedConventions.Collect())
                             .SelectMany((tuple, _) => tuple.Left.AddRange(tuple.Right))
                             .WithComparer(SymbolEqualityComparer.Default);

        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(exportConfigurationCandidate)
               .Select((z, _) => ConventionAttributeData.Create(z.Right, z.Left))
               .Combine(combinedExports.Collect().Select(static (z, _) => z.Distinct(SymbolEqualityComparer.Default).OfType<INamedTypeSymbol>()))
               .Combine(exportedConventions.Collect()),
            static (productionContext, tuple) => ExportConventions.HandleConventionExports(
                productionContext,
                new(
                    tuple.Left.Left,
                    tuple.Left.Right.OrderBy(z => z.ToDisplayString()).ToImmutableArray(),
                    tuple.Right.OrderBy(z => z.ToDisplayString()).ToImmutableArray()
                )
            )
        );

        var importConfigurationCandidate = ConventionConfigurationData
                                          .Create(context, "ImportConventions", ConventionConfigurationData.ImportsDefaults)
                                          .Select((z, _) => !z.WasConfigured && z.Assembly ? z with { Assembly = false, } : z);

        var hasAssemblyLoadContext = context.CompilationProvider
                                            .Select((compilation, _) => compilation.GetTypeByMetadataName("System.Runtime.Loader.AssemblyLoadContext") is { });
        var isTestProject = context.AnalyzerConfigOptionsProvider
                                   .Select(
                                        (provider, _) => provider.GlobalOptions.TryGetValue($"build_property.IsTestProject", out var value)
                                         && bool.TryParse(value, out var v)
                                         && v
                                    );

        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(combinedExports.Collect())
               .Combine(importConfigurationCandidate)
               .Combine(exportConfigurationCandidate)
               .Combine(hasAssemblyLoadContext)
               .Combine(isTestProject)
               .Select(
                    (z, _) => (
                        compilation: z.Left.Left.Left.Left.Left,
                        hasExports: z.Left.Left.Left.Left.Right.Any(),
                        exportedCandidates: z.Left.Left.Left.Left.Right,
                        importConfiguration: z.Left.Left.Left.Right, exportConfiguration: z.Left.Left.Right, hasAssemblyLoadContext: z.Left.Right,
                        isTestProject: z.Right
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
                        tuple.isTestProject,
                        tuple.importConfiguration,
                        tuple.exportConfiguration
                    )
                );
            }
        );

        var getAssembliesSyntaxProvider = context
                                         .SyntaxProvider.CreateSyntaxProvider(
                                              (node, _) => AssemblyCollection.GetAssembliesMethod(node) is { method: { }, selector: { }, },
                                              (syntaxContext, _) => AssemblyCollection.GetAssembliesMethod(syntaxContext)
                                          )
                                         .Combine(hasAssemblyLoadContext)
                                         .Where(z => z is { Right: true, Left: { method: { }, selector: { }, }, })
                                         .Select((z, _) => z.Left)
                                         .Collect();
        var getTypesSyntaxProvider = context
                                    .SyntaxProvider.CreateSyntaxProvider(
                                         (node, _) => TypeCollection.GetTypesMethod(node) is { method: { }, selector: { }, },
                                         (syntaxContext, _) => TypeCollection.GetTypesMethod(syntaxContext)
                                     )
                                    .Combine(hasAssemblyLoadContext)
                                    .Where(z => z is { Right: true, Left: { method: { }, selector: { }, }, })
                                    .Select((tuple, _) => tuple.Left)
                                    .Collect();
        context.RegisterImplementationSourceOutput(
            getAssembliesSyntaxProvider
               .Combine(getTypesSyntaxProvider)
               .Combine(importConfigurationCandidate)
               .Combine(isTestProject)
               .Combine(context.CompilationProvider),
            static (context, results) =>
            {
                AssemblyCollection.Collect(
                    context,
                    new(
                        results.Right,
                        results.Left.Left.Right,
                        results.Left.Left.Left.Left,
                        results.Left.Left.Left.Right,
                        results.Left.Right
                    )
                );
            }
        );
    }
}
