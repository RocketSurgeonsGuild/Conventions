

namespace Clavus.Analyzers.Tests;

public class ExportedMsBuildConventionsTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Pull_Through_A_Convention_With_Custom_Namespace()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    [ClavusExport]
    internal class Contrib : IClavusPart { }
}
"
                           )
                          .AddClavusConfiguration(importNamespace: "ExportedMsBuildConventions", exportNamespace: "Source.Space", exportClassName: "SourceClass")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Pull_Through_A_Convention_With_No_Namespace()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    internal class Contrib : IClavusPart { }
}
"
                           )
                          .AddGlobalOption("build_property.ClavusExportNamespace", "")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }


    [Test]
    public async Task Should_Pull_Through_A_Convention_With_Custom_MethodName()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    [ClavusExport]
    internal class Contrib : IClavusPart { }
}
"
                           )
                          .AddGlobalOption("build_property.ClavusExportMethodName", "SourceMethod")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Before(Test)]
    public Task InitializeAsync()
    {

        Configure(
            b => b
                .IgnoreOutputFile("Imported_Assembly_Conventions.cs")
                .IgnoreOutputFile("Compiled_AssemblyProvider.cs")
        );
        return Task.CompletedTask;
    }
}
