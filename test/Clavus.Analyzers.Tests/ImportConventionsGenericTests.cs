using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;



namespace Clavus.Analyzers.Tests;

public class ImportConventionsGenericTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Clavus;

[assembly: ImportClavusParts]
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               """

                               using Clavus;

                               [assembly: ImportClavusParts(Namespace = "Test.My.Namespace", ClassName = "MyImports")]

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method_No_Namespace()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               """

                               using Clavus;

                               [assembly: ImportClavusParts(Namespace = "", ClassName = "MyImports")]

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_MethodName()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               """

                               using Clavus;

                               [assembly: ImportClavusParts(Namespace = "Test.My.Namespace", ClassName = "MyImports", MethodName = "ImportConventions")]

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Clavus;

[assembly: ImportClavusPartsAttribute]
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Support_No_Exported_Convention_Assemblies()
    {
        var result = await Builder
                          .AddSources(
                               @"
using Clavus;

[assembly: ImportClavusParts]
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly()
    {
        var result = await Builder
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

[assembly: ImportClavusParts]

namespace Clavus.Tests
{
    [ExportClavusPart]
    internal class Contrib : IClavusPart { }
}
",
                               @"
using Clavus;

namespace TestProject
{
    public partial class Program
    {
    }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly_If_Not_Exported()
    {
        var result = await Builder
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;
[assembly: ImportClavusParts]

namespace Clavus.Tests
{
    [ExportClavusPart]
    internal class Contrib : IClavusPart { }
}
",
                               @"
using Clavus;

namespace TestProject
{
    public partial class Program
    {
    }
}
"
                           )
                          .AddGlobalOption("build_property.ExportClavusAssembly", "false")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    [MethodDataSource(nameof(Should_Generate_Static_Assembly_Methods_For_Runnable_Projects_Data))]
    public async Task Should_Generate_Static_Assembly_Methods_For_Runnable_Projects(ImmutableArray<Type> referencedTypes)
    {
        var result = await WithGenericSharedDeps()
                          .AddReferences(referencedTypes.ToArray())
                          .AddSources(
                               @"
using Clavus;

[assembly: ImportClavusParts]
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(Regex.Replace(Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("_", referencedTypes.Select(z => z.Name))))), "[^\\d|\\w]", "").ToLowerInvariant());
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Should_Generate_Static_Assembly_Initializer_When_xunit_is_referenced(bool isTestProject)
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Clavus;

[assembly: ImportClavusParts]
"
                           )
                          .AddGlobalOption("build_property.IsTestProject", isTestProject ? "true" : "false")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(isTestProject);
    }

    public static IEnumerable<object[]> Should_Generate_Static_Assembly_Methods_For_Runnable_Projects_Data()
    {
        yield return [ImmutableArray.CreateRange([typeof(ClavusDistributedApplicationHelpers), typeof(IDistributedApplicationBuilder)])];
        yield return [ImmutableArray.CreateRange([typeof(ClavusDistributedApplicationTestingHelpers), typeof(IDistributedApplicationTestingBuilder)])];
        yield return [ImmutableArray.CreateRange([typeof(ClavusWebAssemblyHelpers), typeof(WebAssemblyHostBuilder)])];
        yield return [ImmutableArray.CreateRange([typeof(ClavusHostApplicationHelpers), typeof(HostApplicationBuilder)])];
        yield return [ImmutableArray.CreateRange([typeof(ClavusHostApplicationHelpers), typeof(WebApplicationBuilder)])];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(ClavusDistributedApplicationHelpers), typeof(IDistributedApplicationBuilder),
                    typeof(ILogger),
                    typeof(ClavusContext),
                ]
            ),
        ];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(ClavusDistributedApplicationTestingHelpers), typeof(IDistributedApplicationTestingBuilder),
                    typeof(ILogger),
                    typeof(ClavusContext),
                ]
            ),
        ];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(ClavusWebAssemblyHelpers), typeof(WebAssemblyHostBuilder),
                    typeof(ILogger),
                    typeof(ClavusContext),
                ]
            ),
        ];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(ClavusHostApplicationHelpers), typeof(HostApplicationBuilder),
                    typeof(ILogger),
                    typeof(ClavusContext),
                ]
            ),
        ];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(ClavusHostApplicationHelpers), typeof(WebApplicationBuilder),
                    typeof(ILogger),
                    typeof(ClavusContext),
                ]
            ),
        ];
    }

    [Before(Test)]
    public Task InitializeAsync()
    {
        Configure(b => b.IgnoreOutputFile("Exported_Conventions.cs"));
        return Task.CompletedTask;
    }
}
