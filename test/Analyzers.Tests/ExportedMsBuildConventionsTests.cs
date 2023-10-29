namespace Rocket.Surgery.Conventions.Analyzers.Tests;

[UsesVerify]
public class ExportedMsBuildConventionsTests
{
    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";

        await Verify(
            GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ExportConventionsNamespace"] = "Source.Space",
                    ["ExportConventionsClassName"] = "SourceClass",
                }
            )
        );
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_No_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";

        await Verify(
            GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ExportConventionsNamespace"] = "",
                }
            )
        );
    }


    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_MethodName()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";

        await Verify(
            GenerationHelpers.GenerateAll(
                source, compilationReferences: await GenerationHelpers.CreateDeps(), properties: new Dictionary<string, string?>
                {
                    ["ExportConventionsMethodName"] = "SourceMethod",
                }
            )
        );
    }
}
