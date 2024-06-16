//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/TopLevel.g.cs
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject.Conventions;
public partial class TopLevelProgram
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static async Task RunAsync(string[] args, IConventionFactory? factory = null, Func<ConventionContextBuilder, CancellationToken, ValueTask>? action = null)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var host = await builder.ConfigureRocketSurgery(Imports.Instance, action);
        await host.RunAsync();
    }
}
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618

#pragma warning restore CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
#nullable restore
