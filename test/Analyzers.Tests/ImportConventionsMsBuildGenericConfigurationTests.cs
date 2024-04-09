using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ImportConventionsMsBuildGenericConfigurationTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var result = await WithGenericSharedDeps()
                          .AddGlobalOption("build_property.ImportConventionsAssembly", "true")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Not_Generate_Static_Assembly_Level_Method_By_Default()
    {
        var result = await WithGenericSharedDeps()
                          .AddGlobalOption("build_property.ImportConventionsAssembly", "false")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var result = await WithGenericSharedDeps()
                          .AddGlobalOption("build_property.ImportConventionsNamespace", "Test.My.Namespace")
                          .AddGlobalOption("build_property.ImportConventionsClassName", "MyImports")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }


    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_No_Namespace()
    {
        var result = await WithGenericSharedDeps()
                          .AddGlobalOption("build_property.ImportConventionsNamespace", "")
                          .AddGlobalOption("build_property.ImportConventionsClassName", "MyImports")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_MethodName()
    {
        var result = await WithGenericSharedDeps()
                          .AddGlobalOption("build_property.ImportConventionsNamespace", "Test.My.Namespace")
                          .AddGlobalOption("build_property.ImportConventionsClassName", "MyImports")
                          .AddGlobalOption("build_property.ImportConventionsMethodName", "ImportConventions")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Use_Assembly_Configuration_If_Defined()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = ""Test.My.Namespace"", ClassName = ""MyImports"", MethodName = ""ImportConventions"")]
"
                           )
                          .AddGlobalOption("build_property.ImportConventionsNamespace", "Test.Other.Namespace")
                          .AddGlobalOption("build_property.ImportConventionsMethodName", "ImportsConventions")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var result = await WithGenericSharedDeps()
                          .AddGlobalOption("build_property.ImportConventionsAssembly", "true")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Support_No_Exported_Convention_Assemblies()
    {
        var result = await Builder
                          .AddGlobalOption("build_property.ImportConventionsAssembly", "true")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }
}
