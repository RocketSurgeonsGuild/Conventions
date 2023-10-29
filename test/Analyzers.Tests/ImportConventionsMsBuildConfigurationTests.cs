namespace Rocket.Surgery.Conventions.Analyzers.Tests;

[UsesVerify]
public class ImportConventionsMsBuildConfigurationTests
{
    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var source = @"";

        await Verify(
            await GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ImportConventionsAssembly"] = "true",
                }
            )
        );
    }

    [Fact]
    public async Task Should_Not_Generate_Static_Assembly_Level_Method_By_Default()
    {
        var source = @"";

        await Verify(
            await GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ImportConventionsAssembly"] = "false",
                }
            )
        );
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var source = @"";

        await Verify(
            await GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ImportConventionsNamespace"] = "Test.My.Namespace",
                    ["ImportConventionsClassName"] = "MyImports",
                }
            )
        );
    }


    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_No_Namespace()
    {
        var source = @"";

        await Verify(
            await GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ImportConventionsNamespace"] = "",
                    ["ImportConventionsClassName"] = "MyImports",
                }
            )
        );
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_MethodName()
    {
        var source = @"";

        await Verify(
            await GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ImportConventionsNamespace"] = "Test.My.Namespace",
                    ["ImportConventionsClassName"] = "MyImports",
                    ["ImportConventionsMethodName"] = "ImportConventions",
                }
            )
        );
    }

    [Fact]
    public async Task Should_Use_Assembly_Configuration_If_Defined()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = ""Test.My.Namespace"", ClassName = ""MyImports"", MethodName = ""ImportConventions"")]
";

        await Verify(
            await GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ImportConventionsNamespace"] = "Test.Other.Namespace",
                    ["ImportConventionsMethodName"] = "ImportsConventions",
                }
            )
        );
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var source = @"";

        await Verify(
            await GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ImportConventionsAssembly"] = "true",
                }
            )
        );
    }

    [Fact]
    public async Task Should_Support_No_Exported_Convention_Assemblies()
    {
        var source = @"";
        await Verify(
            await GenerationHelpers.GenerateAll(
                source,
                properties: new Dictionary<string, string?>
                {
                    ["ImportConventionsAssembly"] = "true"
                }
            )
        );
    }
}
