namespace Clavus.Analyzers.Tests;

/// <summary>
///     Task 3.7: Verify snapshot tests covering BCL mode (default), NodaTime mode (opt-in MSBuild
///     property + `NodaTime` reference present), and the `CLAVUS_CFG002` mismatch diagnostic when
///     the property is enabled without the reference (design.md Decision 3 and Risks section).
///
///     The NodaTime opt-in MSBuild property is `ClavusConfigurationEnableNodaTime`, matching
///     Clavus.Sdk's Sdk.props. The diagnostic id `CLAVUS_CFG002` is taken verbatim from design.md
///     Decision 3 / Risks.
/// </summary>
public class NodaTimeModeTests() : ConfigGeneratorTest()
{
    private const string ConfigJson =
        /*lang=json,strict*/
        """
        {
          "Schedule": {
            "StartDate": "2024-01-01",
            "StartTime": "09:00:00",
            "Timestamp": "2024-01-01T09:00:00Z",
            "Timeout": "00:30:00"
          }
        }
        """;

    [Test]
    public async Task Should_Generate_BCL_Types_By_Default()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", ConfigJson)
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // Expect DateOnly/TimeOnly/DateTimeOffset/TimeSpan - the NodaTime property is unset.
        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_NodaTime_Types_When_Opted_In_And_Referenced()
    {
        var result = await WithSharedDeps()
                          .AddReferences(typeof(NodaTime.LocalDate))
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddGlobalOption("build_property.ClavusConfigurationEnableNodaTime", "true")
                          .AddAdditionalText("appsettings.json", ConfigJson)
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // Expect LocalDate/LocalTime/OffsetDateTime/Duration substituted per design.md Decision 3.
        await Verify(result);
    }

    [Test]
    public async Task Should_Report_CLAVUS_CFG002_When_Enabled_Without_NodaTime_Reference()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddGlobalOption("build_property.ClavusConfigurationEnableNodaTime", "true")
                          .AddAdditionalText("appsettings.json", ConfigJson)
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // The property is enabled but `NodaTime` is not in Compilation.ReferencedAssemblyNames -
        // must report CLAVUS_CFG002 rather than silently falling back to BCL types (design.md
        // Decision 3: "silent fallback would make a project's generated public API shape depend
        // on an easily-missed reference").
        await Verify(result);
    }
}
