using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Multi-project dependency builders for the `clavus-managed-configuration` generator tests,
///     mirroring `Rocket.Surgery.Conventions.Analyzers.Tests.GenerationHelpers`. Each helper
///     compiles a small "library" project (optionally declaring a `ClavusConfiguration` file via
///     an additional text + `build_metadata.AdditionalFiles.ItemType` = "ClavusConfiguration",
///     the same surfacing mechanism `ClavusMetadata`/`ClavusHostType` already use per
///     design.md's "reuses the existing attribute-scanning infrastructure" decision), then chains
///     `AddCompilationReferences` so downstream projects see the marker attribute
///     (`[assembly: Clavus.ConfigurationAssembly(...)]`) the same way `ExportConventions` markers
///     flow through `SampleDependencyOne` -> `SampleDependencyThree` today.
/// </summary>
public static class ConfigGenerationHelpers
{
    public static async Task<GeneratorTestResults[]> CreateManifestDeps(GeneratorTestContextBuilder rootBuilder, CancellationToken cancellationToken)
    {
        var baseBuilder = rootBuilder;
        var contributor = await ConfigContributorLibrary(baseBuilder, cancellationToken);
        var nonContributor = await NonContributingLibrary(baseBuilder, cancellationToken);
        var intermediate = await IntermediateNoConfigLibrary(baseBuilder, contributor, cancellationToken);
        return [contributor, nonContributor, intermediate,];
    }

    /// <summary>A library that declares a single `appsettings.json` ClavusConfiguration item.</summary>
    public static Task<GeneratorTestResults> ConfigContributorLibrary(GeneratorTestContextBuilder builder, CancellationToken cancellationToken) =>
        builder
           .WithProjectName("Sample.ConfigContributor")
           .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
           .AddAdditionalText(
                "appsettings.json",
                """
                {
                  "Sample": {
                    "Name": "contributor",
                    "Timeout": "00:00:30"
                  }
                }
                """
            )
           .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
           .AddSources(
                """
                namespace Sample.ConfigContributor;

                public class Marker;
                """
            )
           .Build()
           .GenerateAsync(cancellationToken);

    /// <summary>A library with no ClavusConfiguration items at all — must be excluded from the manifest.</summary>
    public static Task<GeneratorTestResults> NonContributingLibrary(GeneratorTestContextBuilder builder, CancellationToken cancellationToken) =>
        builder
           .WithProjectName("Sample.NonContributor")
           .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
           .AddSources(
                """
                namespace Sample.NonContributor;

                public class Marker;
                """
            )
           .Build()
           .GenerateAsync(cancellationToken);

    /// <summary>
    ///     A library that itself declares no configuration but references <see cref="ConfigContributorLibrary" />
    ///     - exercises the "intermediate library declares no configuration of its own" transitive case (task 4.6).
    /// </summary>
    public static Task<GeneratorTestResults> IntermediateNoConfigLibrary(
        GeneratorTestContextBuilder builder,
        GeneratorTestResults contributor,
        CancellationToken cancellationToken
    )
    {
        return builder
           .WithProjectName("Sample.IntermediateNoConfig")
           .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
           .AddCompilationReferences(contributor)
           .AddSources(
                """
                using Sample.ConfigContributor;

                namespace Sample.IntermediateNoConfig;

                public class Marker
                {
                    public Marker? Self { get; set; }
                }
                """
            )
           .Build()
           .GenerateAsync(cancellationToken);
    }
}
