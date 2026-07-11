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
        context.RegisterPostInitializationOutput(z => z.AddEmbeddedAttributeDefinition());
        var exportConfiguration = ClavusConfigurationData
                                 .Read(context, "Export")
                                 .WithTrackingName("clavus:export_configuration");

        var importConfiguration = ClavusConfigurationData
                                 .Read(context, "Import")
                                 .WithTrackingName("clavus:import_configuration");

        var msBuildConfig = context
                           .AnalyzerConfigOptionsProvider
                           .Combine(exportConfiguration)
                           .Combine(importConfiguration)
                           .Select((provider, _) => new MsBuildConfig(
                                       provider.Left.Left.GlobalOptions.GetBuildProperty("ClavusMetadata", x => bool.TryParse(x, out var v) && v),
                                       provider.Left.Left.GlobalOptions.GetBuildProperty("ClavusAssignExternal", x => bool.TryParse(x, out var v) && v),
                                       provider.Left.Left.GlobalOptions.GetBuildProperty("IsTestProject", x => bool.TryParse(x, out var v) && v),
                                       provider.Left.Left.GlobalOptions.GetBuildProperty("RootNamespace", s => s) ?? "",
                                       provider.Left.Left.GlobalOptions.GetBuildProperty("ClavusHostType", s => s) ?? "Undefined",
                                       provider.Left.Left.GlobalOptions.GetBuildProperty("ClavusCategory", s => s) ?? "Unknown",
                                       provider.Left.Right,
                                       provider.Right
                                   )
                            )
                           .WithTrackingName("clavus:msbuild");

        var exportedConventions = context
                                 .SyntaxProvider
                                 .ForAttributeWithMetadataName(
                                      "Clavus.ClavusExportAttribute",
                                      (node, _) => node is TypeDeclarationSyntax,
                                      (syntaxContext, _) => (INamedTypeSymbol)syntaxContext.TargetSymbol
                                  )
                                 .Collect()
                                 .Select((z, _) => z.Sort(Comparer<INamedTypeSymbol>.Create((x, y) => string.Compare(x.MetadataName, y.MetadataName, StringComparison.Ordinal))))
                                 .WithTrackingName("clavus_:self_exports");

        context.RegisterSourceOutput(
            msBuildConfig
               .Combine(context.CompilationProvider)
               .Combine(exportedConventions),
            static (productionContext, tuple) => ExportConventions.HandleConventionExports(
                productionContext,
                new(tuple.Left.Left, tuple.Right)
            )
        );


        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(exportedConventions)
               .Combine(msBuildConfig),
            static (productionContext, tuple) => ImportConventions.HandleConventionImports(productionContext, new(tuple.Left.Left, tuple.Right, tuple.Left.Right))
        );
    }
}
