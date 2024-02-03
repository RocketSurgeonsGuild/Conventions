using static Rocket.Surgery.Conventions.Analyzers.Tests.GenerationHelpers;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ImportConventionsTests
{
    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }

    [Fact]
    public async Task Should_Not_Generate_Static_Assembly_Level_Method_By_Default()
    {
        var source = @"
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = ""Test.My.Namespace"", ClassName = ""MyImports"")]
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }


    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_No_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = """", ClassName = ""MyImports"")]
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }


    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_MethodName()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = ""Test.My.Namespace"", ClassName = ""MyImports"", MethodName = ""ImportConventions"")]
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventionsAttribute]
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }

    [Fact]
    public async Task Should_Generate_Static_Class_Member_Level_Method()
    {
        var source = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventions]
    public partial class Program
    {
    }
}
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }


    [Fact]
    public async Task Should_Generate_Static_Class_Member_Level_Method_FullName()
    {
        var source = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventionsAttribute]
    public partial class Program
    {
    }
}
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }

    [Fact]
    public async Task Should_Support_No_Exported_Convention_Assemblies()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
";
        await Verify(GenerateAll(source));
    }

    [Fact]
    public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly()
    {
        var source1 = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";
        var source2 = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventions]
    public partial class Program
    {
    }
}
";

        await Verify(GenerateAll(new[] { source1, source2, }));
    }

    [Fact]
    public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly_If_Not_Exported()
    {
        var source1 = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

namespace Rocket.Surgery.Conventions.Tests
{
    [ExportConvention]
    internal class Contrib : IConvention { }
}
";
        var source2 = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventions]
    public partial class Program
    {
    }
}
";

        await Verify(
            GenerateAll(
                new[] { source1, source2, },
                properties: new Dictionary<string, string?>
                {
                    ["ExportConventionsAssembly"] = "false",
                }
            )
        );
    }
}