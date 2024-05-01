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
        var host = await builder.UseRocketBooster(RocketBooster.For(Imports.Instance), action);
        await host.RunAsync();
    }
}