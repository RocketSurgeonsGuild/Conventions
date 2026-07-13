using Microsoft.Build.Utilities.ProjectCreation;

namespace Clavus.Sdk.Tests;

public class HostTypeTests
{
    [Test]
    public async Task WebApplication_DetectsHostAndInjectsHostingWeb()
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
            )
           .AddFile("web/Program.cs", "System.Console.WriteLine(\"hello\");");

        await project.VerifyProjects();
    }

    [Test]
    public async Task HostApplication_OutputTypeExe_DetectsHostAndInjectsHosting()
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
            )
           .AddFile("worker/Program.cs", "System.Console.WriteLine(\"hello\");");

        await project.VerifyProjects();
    }

    [Test]
    public async Task WebAssembly_DetectsHostAndInjectsHostingWebAssembly()
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
                "wasm/wasm.csproj",
                ProjectCreator
                   .Templates.SdkCsproj(targetFramework: "net10.0", sdk: "Microsoft.NET.Sdk.BlazorWebAssembly")
            )
           .AddFile("wasm/Program.cs", "System.Console.WriteLine(\"hello\");");

        await project.VerifyProjects();
    }

    [Test]
    public async Task Library_NoOutputType_NoHostDetected()
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
            );

        await project.VerifyProjects();
    }
}
