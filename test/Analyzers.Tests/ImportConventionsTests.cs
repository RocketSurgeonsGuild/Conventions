using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ImportConventionsTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Not_Generate_Static_Assembly_Level_Method_By_Default()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = ""Test.My.Namespace"", ClassName = ""MyImports"")]
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }


    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_No_Namespace()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = """", ClassName = ""MyImports"")]
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }


    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_MethodName()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = ""Test.My.Namespace"", ClassName = ""MyImports"", MethodName = ""ImportConventions"")]
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventionsAttribute]
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Support_No_Exported_Convention_Assemblies()
    {
        var result = await Builder
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly()
    {
        var result = await Builder
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
",
                               @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ExportConvention]
    [ImportConventions]
    public partial class Program
    {
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly_If_Not_Exported()
    {
        var result = await Builder
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

namespace Rocket.Surgery.Conventions.Tests
{
    [ExportConvention]
    internal class Contrib : IConvention { }
}
",
                               @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventions]
    public partial class Program
    {
    }
}
"
                           )
                          .AddGlobalOption("build_property.ExportConventionsAssembly", "false")
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Configure(b => b.IgnoreOutputFile("Exported_Conventions.cs"));
    }
}
