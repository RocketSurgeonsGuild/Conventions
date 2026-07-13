using Microsoft.Build.Utilities.ProjectCreation;

namespace Clavus.Sdk.Tests;

public class SdkTests
{
    [Test]
    public async Task BaseSdk_AppliesSharedDefaults_AndInjectsAnalyzers()
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
                   .ItemInclude(
                        "ClavusPart",
                        include: "Logging",
                        metadata: new Dictionary<string, string?>
                        {
                            ["ParameterType"] = "global::Microsoft.Extensions.Logging.ILoggingBuilder",
                            ["ParameterName"] = "builder"
                        }
                    )
            );

        await project.VerifyProjects();
    }
}
