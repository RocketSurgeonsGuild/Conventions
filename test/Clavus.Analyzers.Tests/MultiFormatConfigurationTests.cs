using Clavus.Configuration.Toml;
using Clavus.Configuration.Yaml;

using Microsoft.Extensions.Configuration;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Task 5.6: Verify/integration tests confirming JSON, YAML, and TOML configuration files
///     produce equivalent generated classes and bind equivalent values, per design.md Decision 6 -
///     the generator-side parser only needs a flat key -> raw-string-value view (same shape it
///     already needs from JSON) to run type inference, decoupled from the runtime provider.
///
///     Two layers of equivalence are asserted:
///     1. Generator-level: three otherwise-identical config files (one per format, same logical
///        key/value shape) must produce byte-identical generated configuration classes modulo the
///        source file name/extension - asserted via Verify snapshot comparison.
///     2. Runtime-level: binding each format's file through its `IConfigurationSource`
///        (`AddJsonFile`/`AddYamlFile`/`AddTomlFile`) into `IConfiguration` must yield identical
///        bound values.
///
///     ASSUMPTION: a `AddTomlFile` extension exists on `IConfigurationBuilder` in
///     `Clavus.Configuration.Toml`, mirroring `Clavus.Configuration.Yaml`'s `AddYamlFile` shape
///     (task 5.4). This does not exist in this worktree yet - Dallas owns runtime providers.
/// </summary>
public class MultiFormatConfigurationTests() : ConfigGeneratorTest()
{
    private const string JsonBody =
        /*lang=json,strict*/
        """
        {
          "Sample": {
            "Name": "shared-value",
            "Timeout": "00:00:30",
            "StartDate": "2024-01-01"
          }
        }
        """;

    private const string YamlBody =
        """
        Sample:
          Name: shared-value
          Timeout: "00:00:30"
          StartDate: "2024-01-01"
        """;

    private const string TomlBody =
        """
        [Sample]
        Name = "shared-value"
        Timeout = "00:00:30"
        StartDate = "2024-01-01"
        """;

    [Test]
    public async Task Should_Generate_Equivalent_Classes_For_Json_Yaml_And_Toml_With_The_Same_Shape()
    {
        var jsonResult = await WithSharedDeps()
                              .WithProjectName("Sample.JsonFormat")
                              .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                              .AddAdditionalText("appsettings.json", JsonBody)
                              .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                              .Build()
                              .GenerateAsync(TestContext.CancellationToken);

        var yamlResult = await WithSharedDeps()
                              .WithProjectName("Sample.YamlFormat")
                              .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                              .AddAdditionalText("appsettings.yaml", YamlBody)
                              .AddOption("appsettings.yaml", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Yaml")
                              .Build()
                              .GenerateAsync(TestContext.CancellationToken);

        var tomlResult = await WithSharedDeps()
                              .WithProjectName("Sample.TomlFormat")
                              .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                              .AddAdditionalText("appsettings.toml", TomlBody)
                              .AddOption("appsettings.toml", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Toml")
                              .Build()
                              .GenerateAsync(TestContext.CancellationToken);

        // A single combined snapshot makes format-drift visible at a glance: the generated
        // `SampleConfiguration`-shaped class body should be identical across all three (modulo
        // the IConfigurationSource registration line, which is naturally format-specific).
        await Verify(new { Json = jsonResult, Yaml = yamlResult, Toml = tomlResult, });
    }

    [Test]
    public void Should_Bind_Equivalent_Values_From_Json_Yaml_And_Toml_At_Runtime()
    {
        // Runtime-level equivalence check: independent of generated-class shape, the three
        // IConfigurationSource providers must produce the same bound values for the same
        // logical shape. This does not depend on the generator at all - it exercises
        // Clavus.Configuration.Json (existing), Clavus.Configuration.Yaml (existing,
        // pre-rename Rocket.Surgery.Conventions.Configuration.Yaml), and the new
        // Clavus.Configuration.Toml provider Dallas is adding (task 5.4).
        var jsonPath = WriteTempFile("appsettings.json", JsonBody);
        var yamlPath = WriteTempFile("appsettings.yaml", YamlBody);
        var tomlPath = WriteTempFile("appsettings.toml", TomlBody);

        var jsonConfig = new ConfigurationBuilder().AddJsonFile(jsonPath).Build();
        var yamlConfig = new ConfigurationBuilder().AddYamlFile(yamlPath).Build();
        var tomlConfig = new ConfigurationBuilder().AddTomlFile(tomlPath).Build();

        var jsonBound = jsonConfig.GetSection("Sample").Get<SampleSection>();
        var yamlBound = yamlConfig.GetSection("Sample").Get<SampleSection>();
        var tomlBound = tomlConfig.GetSection("Sample").Get<SampleSection>();

        yamlBound.ShouldBeEquivalentTo(jsonBound);
        tomlBound.ShouldBeEquivalentTo(jsonBound);
    }

    private static string WriteTempFile(string name, string contents)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}-{name}");
        File.WriteAllText(path, contents);
        return path;
    }

    private sealed class SampleSection
    {
        public string? Name { get; set; }
        public string? Timeout { get; set; }
        public string? StartDate { get; set; }
    }
}
