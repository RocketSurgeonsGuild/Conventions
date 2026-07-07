using System.Collections.Immutable;
using Clavus.Support;
using Clavus.Support.Configuration;
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
                                       provider.Right,
                                       provider.Left.Left.GlobalOptions.GetBuildProperty("EnableClavusConfiguration", x => bool.TryParse(x, out var v) && v),
                                       provider.Left.Left.GlobalOptions.GetBuildProperty("ClavusConfigurationNodaTime", x => bool.TryParse(x, out var v) && v)
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

        // Configuration pipeline (openspec/changes/clavus-managed-configuration): discovers ClavusConfiguration
        // AdditionalFiles, groups their base/environment/local layers, and produces the generated configuration
        // classes/IConfigurationPart implementations, the [assembly: Clavus.ConfigurationAssembly] markers, and
        // the host-visible ClavusConfigurationManifest. Reuses the same AnalyzerConfigOptionsProvider-based
        // MSBuild plumbing pattern as ClavusMetadata/ClavusHostType.
        var configurationSourceFiles = ConfigurationDiscovery
                                       .GetSourceFiles(context)
                                       .Collect()
                                       .WithTrackingName("clavus:configuration_source_files");

        var jsonConfigurationGroups = configurationSourceFiles
                                      .Select(static (files, _) => ConfigurationDiscovery.GroupJsonFiles(files))
                                      .WithTrackingName("clavus:configuration_groups_json");

        var allConfigurationGroups = configurationSourceFiles
                                     .Select(static (files, _) => ConfigurationDiscovery.GroupAllFiles(files))
                                     .WithTrackingName("clavus:configuration_groups_all");

        var configurationPartTypeNames = msBuildConfig
                                         .Combine(jsonConfigurationGroups)
                                         .Select(
                                              static (pair, _) => pair.Left.EnableClavusConfiguration
                                                  ? [.. pair.Right
                                                       .Select(
                                                            group => $"global::{( pair.Left.RootNamespace is { Length: > 0, } ? $"{pair.Left.RootNamespace}." : "" )}{ConfigurationIdentifiers.ToRootClassName(group.BaseName)}Part"
                                                        )]
                                                  : ImmutableArray<string>.Empty
                                          )
                                         .WithTrackingName("clavus:configuration_part_type_names");

        context.RegisterSourceOutput(
            msBuildConfig
               .Combine(context.CompilationProvider)
               .Combine(exportedConventions)
               .Combine(configurationPartTypeNames),
            static (productionContext, tuple) => ExportConventions.HandleConventionExports(
                productionContext,
                new(tuple.Left.Left.Left, tuple.Left.Right, tuple.Right)
            )
        );


        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(exportedConventions)
               .Combine(msBuildConfig),
            static (productionContext, tuple) => ImportConventions.HandleConventionImports(productionContext, new(tuple.Left.Left, tuple.Right, tuple.Left.Right))
        );

        context.RegisterSourceOutput(
            msBuildConfig
               .Combine(jsonConfigurationGroups)
               .Combine(allConfigurationGroups)
               .Combine(context.CompilationProvider),
            static (productionContext, tuple) =>
            {
                var config = tuple.Left.Left.Left;
                var jsonGroups = tuple.Left.Left.Right;
                var allGroups = tuple.Left.Right;
                var compilation = tuple.Right;

                if (!config.EnableClavusConfiguration) return;

                var nodaTimeReferenced = compilation.ReferencedAssemblyNames.Any(
                    id => string.Equals(id.Name, "NodaTime", StringComparison.OrdinalIgnoreCase)
                );

                if (config.ClavusConfigurationUseNodaTime && !nodaTimeReferenced)
                {
                    productionContext.ReportDiagnostic(Diagnostic.Create(Diagnostics.NodaTimeEnabledWithoutReference, Location.None));
                }

                var useNodaTime = config.ClavusConfigurationUseNodaTime && nodaTimeReferenced;

                foreach (var group in jsonGroups)
                {
                    var output = ConfigurationClassEmitter.Emit(group, config.RootNamespace, useNodaTime);
                    productionContext.AddSource(output.HintName, output.SourceText);
                }

                if (!allGroups.IsDefaultOrEmpty)
                {
                    productionContext.AddSource(ConfigurationAssemblyMarkerEmitter.HintName, ConfigurationAssemblyMarkerEmitter.Emit(allGroups));
                }

                productionContext.AddSource(
                    ConfigurationManifestEmitter.HintName,
                    ConfigurationManifestEmitter.Emit(compilation, config.RootNamespace, allGroups, compilation.AssemblyName ?? "")
                );
            }
        );
    }
}
