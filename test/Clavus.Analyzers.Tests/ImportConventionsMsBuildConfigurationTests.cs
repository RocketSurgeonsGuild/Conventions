

namespace Clavus.Analyzers.Tests;

public class ImportConventionsMsBuildConfigurationTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.ClavusImportAssembly", "true")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Not_Generate_Static_Assembly_Level_Method_By_Default()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.ClavusImportAssembly", "false")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var result = await WithSharedDeps()
                          .AddClavusConfiguration(importNamespace: "Test.My.Namespace", exportNamespace: "", importClassName: "MyImports")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }


    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method_No_Namespace()
    {
        var result = await WithSharedDeps()
                          .AddClavusConfiguration(importNamespace: "", exportNamespace: "", importClassName: "MyImports")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_MethodName()
    {
        var result = await WithSharedDeps()
                          .AddClavusConfiguration(importNamespace: "Test.My.Namespace", exportNamespace: "", importClassName: "MyImports", importMethodName: "ImportConventions")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Use_Assembly_Configuration_If_Defined()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"using Clavus;

"
                           )
                          .AddClavusConfiguration(importNamespace: "Test.Other.Namespace", exportNamespace: "", importMethodName: "ImportsConventions")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.ClavusImportAssembly", "true")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Support_No_Exported_Convention_Assemblies()
    {
        var result = await Builder
                          .AddGlobalOption("build_property.ClavusImportAssembly", "true")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Before(Test)]
    public Task InitializeAsync()
    {
        Configure(b => b.IgnoreOutputFile("Exported_Conventions.cs"));
        return Task.CompletedTask;
    }
}
