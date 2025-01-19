using System.Collections.Immutable;

using Aspire.Hosting;
using Aspire.Hosting.Testing;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Hosting;

using Rocket.Surgery.Aspire.Hosting;
using Rocket.Surgery.Aspire.Hosting.Testing;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.WebAssembly.Hosting;

using Serilog;

using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ImportConventionsGenericTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var result = await WithGenericSharedDeps()
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
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               """

using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = "Test.My.Namespace", ClassName = "MyImports")]

"""
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_No_Namespace()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               """

using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = "", ClassName = "MyImports")]

"""
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_MethodName()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               """

using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = "Test.My.Namespace", ClassName = "MyImports", MethodName = "ImportConventions")]

"""
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var result = await WithGenericSharedDeps()
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

[assembly: ImportConventions]

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
[assembly: ImportConventions]

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

    [Theory]
    [MemberData(nameof(Should_Generate_Static_Assembly_Methods_For_Runnable_Projects_Data))]
    public async Task Should_Generate_Static_Assembly_Methods_For_Runnable_Projects(ImmutableArray<Type> referencedTypes)
    {
        var result = await WithGenericSharedDeps()
                          .AddReferences(referencedTypes.ToArray())
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result).UseParameters(string.Join("_", referencedTypes.Select(z => z.Name))).HashParameters();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Generate_Static_Assembly_Initializer_When_xunit_is_referenced(bool isTestProject)
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
"
                           )
                          .AddGlobalOption("build_property.IsTestProject", isTestProject ? "true" : "false")
                          .Build()
                          .GenerateAsync();

        await Verify(result).UseParameters(isTestProject);
    }

    public static IEnumerable<object[]> Should_Generate_Static_Assembly_Methods_For_Runnable_Projects_Data()
    {
        yield return [ImmutableArray.CreateRange([typeof(RocketDistributedApplicationExtensions), typeof(IDistributedApplicationBuilder)])];
        yield return [ImmutableArray.CreateRange([typeof(RocketDistributedApplicationTestingExtensions), typeof(IDistributedApplicationTestingBuilder)])];
        yield return [ImmutableArray.CreateRange([typeof(RocketWebAssemblyExtensions), typeof(WebAssemblyHostBuilder)])];
        yield return [ImmutableArray.CreateRange([typeof(RocketHostApplicationExtensions), typeof(HostApplicationBuilder)])];
        yield return [ImmutableArray.CreateRange([typeof(RocketHostApplicationExtensions), typeof(WebApplicationBuilder)])];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(RocketDistributedApplicationExtensions), typeof(IDistributedApplicationBuilder),
                    typeof(ILogger),
                    typeof(ConventionSerilogExtensions),
                ]
            ),
        ];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(RocketDistributedApplicationTestingExtensions), typeof(IDistributedApplicationTestingBuilder),
                    typeof(ILogger),
                    typeof(ConventionSerilogExtensions),
                ]
            ),
        ];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(RocketWebAssemblyExtensions), typeof(WebAssemblyHostBuilder),
                    typeof(ILogger),
                    typeof(ConventionSerilogExtensions),
                ]
            ),
        ];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(RocketHostApplicationExtensions), typeof(HostApplicationBuilder),
                    typeof(ILogger),
                    typeof(ConventionSerilogExtensions),
                ]
            ),
        ];
        yield return
        [
            ImmutableArray.CreateRange(
                [
                    typeof(RocketHostApplicationExtensions), typeof(WebApplicationBuilder),
                    typeof(ILogger),
                    typeof(ConventionSerilogExtensions),
                ]
            ),
        ];
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Configure(b => b.IgnoreOutputFile("Exported_Conventions.cs"));
    }
}
