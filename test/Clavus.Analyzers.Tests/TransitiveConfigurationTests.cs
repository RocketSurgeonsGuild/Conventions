namespace Clavus.Analyzers.Tests;

/// <summary>
///     Task 4.6: Verify snapshot tests for transitive configuration flow across two and three
///     levels of project references, per design.md Decision 5 - `IConfigurationPart`s ride the
///     same dependency-walk mechanism already required for cross-project convention export, so a
///     consumer several levels down the reference chain gets configuration parts without
///     redeclaring anything.
/// </summary>
public class TransitiveConfigurationTests() : ConfigGeneratorTest()
{
    [Test]
    public async Task Should_Flow_Configuration_Two_Levels_Direct_Reference()
    {
        var contributor = await ConfigGenerationHelpers.ConfigContributorLibrary(Builder, TestContext.CancellationToken);

        var result = await Configure(b => b.AddCompilationReferences(contributor))
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddSources(
                               """
                               using Sample.ConfigContributor;

                               namespace Sample.DirectConsumer;

                               public class Program;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // Sample.DirectConsumer directly references Sample.ConfigContributor (1 hop) and must
        // see its IConfigurationPart in its own flattened export set.
        await Verify(result);
    }

    [Test]
    public async Task Should_Flow_Configuration_Three_Levels_Through_A_Non_Contributing_Intermediate()
    {
        var contributor = await ConfigGenerationHelpers.ConfigContributorLibrary(Builder, TestContext.CancellationToken);
        var intermediate = await ConfigGenerationHelpers.IntermediateNoConfigLibrary(Builder, contributor, TestContext.CancellationToken);

        var result = await Configure(b => b.AddCompilationReferences(contributor, intermediate))
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddSources(
                               """
                               using Sample.IntermediateNoConfig;

                               namespace Sample.TransitiveConsumer;

                               public class Program
                               {
                                   public Marker? Marker { get; set; }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // Sample.TransitiveConsumer -> Sample.IntermediateNoConfig (declares no config of its
        // own) -> Sample.ConfigContributor (2 hops, 3 total projects). The consumer must still
        // see Sample.ConfigContributor's IConfigurationPart flattened into its export set,
        // without Sample.IntermediateNoConfig redeclaring anything.
        await Verify(result);
    }

    [Test]
    public async Task Should_Flow_Configuration_When_Multiple_Sibling_Libraries_Each_Contribute()
    {
        var contributorA = await ConfigGenerationHelpers.ConfigContributorLibrary(Builder, TestContext.CancellationToken);
        var contributorB = await Builder
                                .WithProjectName("Sample.ConfigContributorB")
                                .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                                .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Other": { "Flag": "true" } }""")
                                .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                                .AddSources(
                                     """
                                     namespace Sample.ConfigContributorB;

                                     public class Marker;
                                     """
                                 )
                                .Build()
                                .GenerateAsync(TestContext.CancellationToken);

        var result = await Configure(b => b.AddCompilationReferences(contributorA, contributorB))
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddSources(
                               """
                               using Sample.ConfigContributor;
                               using Sample.ConfigContributorB;

                               namespace Sample.MultiConsumer;

                               public class Program;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // Both sibling libraries' IConfigurationParts must flatten into the same export set
        // without either suppressing the other.
        await Verify(result);
    }
}
