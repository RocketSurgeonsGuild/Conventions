//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/TopLevel.cs
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
        Func<ConventionContextBuilder, CancellationToken, ValueTask> sourceAction = (z, c) => ValueTask.CompletedTask;
        var builder = Host.CreateApplicationBuilder(args);
        var host = await builder.ConfigureRocketSurgery(factory ?? Imports.Instance, async (builder, token) =>
        {
            await sourceAction(builder, token);
            await action(builder, token);
        });
        await host.RunAsync();
    }
}