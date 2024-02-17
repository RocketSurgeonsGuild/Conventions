using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ExportedMsBuildConventionsTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_Namespace()
    {
        var result = await WithSharedDeps()
                    .AddSources(
                         @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
"
                     )
                    .AddGlobalOption("build_property.ExportConventionsNamespace", "Source.Space")
                    .AddGlobalOption("build_property.ExportConventionsClassName", "SourceClass")
                    .Build()
                    .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_No_Namespace()
    {
        var result = await WithSharedDeps()
                    .AddSources(
                         @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
"
                     )
                    .AddGlobalOption("build_property.ExportConventionsNamespace", "")
                    .Build()
                    .GenerateAsync();

        await Verify(result);
    }


    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_MethodName()
    {
        var result = await WithSharedDeps()
                    .AddSources(
                         @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
"
                     )
                    .AddGlobalOption("build_property.ExportConventionsMethodName", "SourceMethod")
                    .Build()
                    .GenerateAsync();

        await Verify(result);
    }
}
