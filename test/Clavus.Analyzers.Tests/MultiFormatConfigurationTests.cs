using Clavus.Configuration.Toml;
using Clavus.Configuration.Yaml;

using Microsoft.Extensions.Configuration;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Runtime-level equivalence test confirming JSON, YAML, and TOML configuration files with the
///     same logical shape bind to identical values through their respective
///     `IConfigurationSource`s (`AddJsonFile`/`AddYamlFile`/`AddTomlFile`).
///
///     NOTE: this previously also contained a generator-level equivalence test
///     (`Should_Generate_Equivalent_Classes_For_Json_Yaml_And_Toml_With_The_Same_Shape`) covering
///     task 5.6 / design.md Decision 6 of the `clavus-managed-configuration` change. That test
///     exercised the generator-side config-class-emission pipeline
///     (`ConfigurationClassEmitter`/`ConfigurationDiscovery`/etc.), which was removed wholesale in
///     `1bd74928` ("simplify configuration"). Deleted rather than resurrected — see
///     `.squad/decisions/inbox/ash-analyzers-managed-configuration-tests-removed.md`.
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
