using Microsoft.Build.Utilities.ProjectCreation;

namespace Clavus.Sdk.Tests;

/// <summary>
/// Covers <c>ClavusPackConfiguration</c> (see <c>Clavus.HostDetection.targets</c>): whether
/// <c>Clavus.PackConfiguration.targets</c> - the appsettings rename/pack/copy + codegen surface
/// exercised by <see cref="ConfigurationTests"/> - is imported at all for a given project.
/// </summary>
public class PackingTests
{
    [Test]
    public async Task PlainLibrary_DefaultsClavusPackConfigurationTrue()
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
           .AddFile("lib/appsettings.yaml", "{}");

        await project.VerifyProjects();
    }

    [Test]
    public async Task ExplicitlyDisabled_SkipsRenameAndCodegenEvenWithAppSettingsFile()
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
                   .Property("ClavusPackConfiguration", "false")
                   .ItemInclude(
                        "PackageReference",
                        include: "Clavus.Configuration.Yaml",
                        metadata: new Dictionary<string, string?> { ["Version"] = Config.ClavusPackageVersion("Clavus.Configuration.Yaml") }
                    )
            )
           .AddFile("lib/appsettings.yaml", "{}");

        await project.VerifyProjects();
    }

    [Test]
    public async Task WebApplication_ExplicitlyEnabled_StillAppliesRenameAndCodegen()
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
                "web/web.csproj",
                ProjectCreator
                   .Templates.SdkCsproj(targetFramework: "net10.0", sdk: "Microsoft.NET.Sdk.Web")
                   .Property("ClavusPackConfiguration", "true")
                   .ItemInclude(
                        "PackageReference",
                        include: "Clavus.Configuration.Yaml",
                        metadata: new Dictionary<string, string?> { ["Version"] = Config.ClavusPackageVersion("Clavus.Configuration.Yaml") }
                    )
            )
           .AddFile("web/Program.cs", "System.Console.WriteLine(\"hello\");")
           .AddFile("web/appsettings.yaml", "{}");

        await project.VerifyProjects();
    }
}
