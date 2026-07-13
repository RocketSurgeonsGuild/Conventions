using Microsoft.Build.Utilities.ProjectCreation;

namespace Clavus.Sdk.Tests;

/// <summary>
/// Covers the combination no single existing test exercises: a detected <c>ClavusHost</c> (via
/// Clavus.HostDetection.targets) together with a custom <c>ClavusPart</c> item, wired up by a real
/// Program.cs that calls the generated <c>ConfigureClavus</c>/<c>Configure{Part}</c> extension
/// methods. That forces Clavus.Parts.targets, Clavus.Hosts.targets and Clavus.ContextBuilder.targets
/// (the last of which only emits <c>ClavusContextBuilder.g.cs</c> when a <c>ClavusHost</c> item is
/// present - see Clavus.ContextBuilder.targets) to all run against the same project, and the
/// generated code has to actually compile against real call sites instead of just being declared.
/// </summary>
public class ClavusPartHostContextBuilderTests
{
    [Test]
    public async Task HostApplication_WithClavusPart_GeneratesHostPartAndContextBuilderTogether()
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
                "worker/worker.csproj",
                ProjectCreator
                   .Templates.SdkCsproj(targetFramework: "net10.0", outputType: "Exe")
                   .ItemInclude(
                        "PackageReference",
                        include: "Microsoft.Extensions.Logging",
                        metadata: new Dictionary<string, string?> { ["Version"] = "10.0.9" }
                    )
                   .ItemInclude(
                        "ClavusPart",
                        include: "Logging",
                        metadata: new Dictionary<string, string?>
                        {
                            ["ParameterType"] = "global::Microsoft.Extensions.Logging.ILoggingBuilder",
                            ["ParameterName"] = "builder"
                        }
                    )
            )
           .AddFile(
                "worker/Program.cs",
                """
                using Microsoft.Extensions.Hosting;
                using worker;

                var builder = Host.CreateApplicationBuilder(args);
                await builder.ConfigureClavus(static contextBuilder => contextBuilder.ConfigureLogging(static (context, logging) => { }));
                """
            );

        await project.VerifyProjects();
    }
}
