#if !ROSLYN4_0
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ImportConventionsGenericTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly()
    {
        var result = await Builder
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
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
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }
}
#endif
