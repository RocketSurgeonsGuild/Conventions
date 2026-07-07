namespace Clavus.Analyzers.Tests;

/// <summary>
///     Task 2.8 (openspec/changes/clavus-managed-configuration/tasks.md): Verify snapshot tests
///     for `ClavusConfigurationManifest` generation, per design.md Decision 2 — the host-side
///     generator pass reads `[assembly: Clavus.ConfigurationAssembly(...)]` markers off
///     referenced assemblies (the same reference walk already used for convention export) and
///     emits a manifest listing contributing assemblies and their relative config paths.
///
///     ASSUMPTION (pending Ripley's generator landing): the manifest type name
///     (`ClavusConfigurationManifest`) and the marker attribute shape follow design.md verbatim;
///     exact member names/shape of the emitted manifest are asserted via Verify snapshot, so
///     once the real generator lands the first run simply establishes the baseline snapshot.
/// </summary>
public class ManifestGenerationTests() : ConfigGeneratorTest()
{
    [Test]
    public async Task Should_List_A_Single_Contributing_Assembly()
    {
        var contributor = await ConfigGenerationHelpers.ConfigContributorLibrary(Builder, TestContext.CancellationToken);

        var result = await Configure(b => b.AddCompilationReferences(contributor))
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddSources(
                               """
                               using Sample.ConfigContributor;

                               namespace Sample.Host;

                               public class Program;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Exclude_Non_Contributing_References()
    {
        var contributor = await ConfigGenerationHelpers.ConfigContributorLibrary(Builder, TestContext.CancellationToken);
        var nonContributor = await ConfigGenerationHelpers.NonContributingLibrary(Builder, TestContext.CancellationToken);

        var result = await Configure(b => b.AddCompilationReferences(contributor, nonContributor))
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddSources(
                               """
                               using Sample.ConfigContributor;
                               using Sample.NonContributor;

                               namespace Sample.Host;

                               public class Program;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // The manifest must only list Sample.ConfigContributor - Sample.NonContributor declares
        // no ClavusConfiguration items and must not appear as a contributing assembly.
        await Verify(result);
    }

    [Test]
    public async Task Should_List_Transitive_Multi_Level_Contributors()
    {
        var contributor = await ConfigGenerationHelpers.ConfigContributorLibrary(Builder, TestContext.CancellationToken);
        var intermediate = await ConfigGenerationHelpers.IntermediateNoConfigLibrary(Builder, contributor, TestContext.CancellationToken);

        var result = await Configure(b => b.AddCompilationReferences(contributor, intermediate))
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddSources(
                               """
                               using Sample.IntermediateNoConfig;

                               namespace Sample.Host;

                               public class Program
                               {
                                   public Marker? Marker { get; set; }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // Sample.IntermediateNoConfig declares no configuration of its own, but the manifest
        // must still surface Sample.ConfigContributor two levels down the reference graph -
        // this is the "intermediate library declares no configuration" transitive case.
        await Verify(result);
    }
}
