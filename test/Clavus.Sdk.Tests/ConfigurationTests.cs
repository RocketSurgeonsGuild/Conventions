using Microsoft.Build.Utilities.ProjectCreation;

namespace Clavus.Sdk.Tests;

/// <summary>
/// Covers <c>src/Clavus.Sdk/Sdk/Clavus.PackConfiguration.targets</c>: for each supported
/// <c>appsettings.{ext}</c> format, an existing file is renamed/packed/copied to
/// <c>$(MSBuildProjectName).{ext}</c> and - when the matching configuration provider assembly is
/// also referenced - a generated <c>IConfigurationPart</c> source is emitted. Both effects are
/// driven purely by file presence + provider-reference detection, independent of the
/// (default-off) <c>ClavusEnable*Configuration</c>/<c>Clavus.Configuration.targets</c> opt-in path.
/// </summary>
public class ConfigurationTests
{
    [Test]
    [Arguments("json", "Microsoft.Extensions.Configuration.Json", "10.0.9", "Clavus.JsonConfiguration.g.cs")]
    [Arguments("yaml", "Clavus.Configuration.Yaml", null, "Clavus.YamlConfiguration.g.cs")]
    [Arguments("yml", "Clavus.Configuration.Yaml", null, "Clavus.YmlConfiguration.g.cs")]
    [Arguments("toml", "Clavus.Configuration.Toml", null, "Clavus.TomlConfiguration.g.cs")]
    public async Task AppSettingsFile_WithProviderReferenced_EmitsConfigurationPart(
        string extension,
        string providerPackageId,
        string? providerVersion,
        string generatedFileName
    )
    {
        var version = providerVersion ?? Config.ClavusPackageVersion(providerPackageId);

        using var project = new SdkTestProject();
        project
           .AddProject(
                "Directory.Build.props",
                ProjectCreator
                   .Create("Directory.Build.props")
                   .Sdk("Clavus.Sdk")
            )
           .AddProject(
                "lib/lib.csproj",
                ProjectCreator
                   .Templates.SdkCsproj(targetFramework: "net10.0")
                   .ItemInclude(
                        "PackageReference",
                        include: providerPackageId,
                        metadata: new Dictionary<string, string?> { ["Version"] = version }
                    )
            )
           .AddFile($"lib/appsettings.{extension}", "{}");

        await project.VerifyProjects();
    }

    /// <summary>
    /// Two independently-detected <c>appsettings.{ext}</c> providers in the same project don't
    /// interfere with each other: each still gets renamed/copied and gets its own generated
    /// <c>IConfigurationPart</c>, side by side. Split into one method per pair (rather than
    /// [Arguments] rows) because Verify derives each snapshot's file name from the full parameter
    /// list, and doubling the parameters for a provider pair overflows the filesystem's file name
    /// length limit.
    /// </summary>
    [Test]
    public Task TwoAppSettingsFiles_JsonAndYaml_EmitBothConfigurationParts() =>
        VerifyTwoProviders("json", "Microsoft.Extensions.Configuration.Json", "10.0.9", "yaml", "Clavus.Configuration.Yaml", null);

    [Test]
    public Task TwoAppSettingsFiles_JsonAndToml_EmitBothConfigurationParts() =>
        VerifyTwoProviders("json", "Microsoft.Extensions.Configuration.Json", "10.0.9", "toml", "Clavus.Configuration.Toml", null);

    [Test]
    public Task TwoAppSettingsFiles_YamlAndToml_EmitBothConfigurationParts() =>
        VerifyTwoProviders("yaml", "Clavus.Configuration.Yaml", null, "toml", "Clavus.Configuration.Toml", null);

    private static async Task VerifyTwoProviders(
        string extensionA,
        string providerPackageIdA,
        string? providerVersionA,
        string extensionB,
        string providerPackageIdB,
        string? providerVersionB
    )
    {
        var versionA = providerVersionA ?? Config.ClavusPackageVersion(providerPackageIdA);
        var versionB = providerVersionB ?? Config.ClavusPackageVersion(providerPackageIdB);

        using var project = new SdkTestProject();
        project
           .AddProject(
                "Directory.Build.props",
                ProjectCreator
                   .Create("Directory.Build.props")
                   .Sdk("Clavus.Sdk")
            )
           .AddProject(
                "lib/lib.csproj",
                ProjectCreator
                   .Templates.SdkCsproj(targetFramework: "net10.0")
                   .ItemInclude(
                        "PackageReference",
                        include: providerPackageIdA,
                        metadata: new Dictionary<string, string?> { ["Version"] = versionA }
                    )
                   .ItemInclude(
                        "PackageReference",
                        include: providerPackageIdB,
                        metadata: new Dictionary<string, string?> { ["Version"] = versionB }
                    )
            )
           .AddFile($"lib/appsettings.{extensionA}", "{}")
           .AddFile($"lib/appsettings.{extensionB}", "{}");

        await project.VerifyProjects();
    }

    [Test]
    [Arguments("json")]
    [Arguments("yaml")]
    [Arguments("yml")]
    [Arguments("toml")]
    public async Task AppSettingsFile_WithoutProviderReferenced_NoConfigurationPartEmitted(string extension)
    {
        using var project = new SdkTestProject();
        project
           .AddProject(
                "Directory.Build.props",
                ProjectCreator
                   .Create("Directory.Build.props")
                   .Sdk("Clavus.Sdk")
            )
           .AddProject(
                "lib/lib.csproj",
                ProjectCreator
                   .Templates.SdkCsproj(targetFramework: "net10.0")
            )
           .AddFile($"lib/appsettings.{extension}", "{}");

        await project.VerifyProjects();
    }
}
