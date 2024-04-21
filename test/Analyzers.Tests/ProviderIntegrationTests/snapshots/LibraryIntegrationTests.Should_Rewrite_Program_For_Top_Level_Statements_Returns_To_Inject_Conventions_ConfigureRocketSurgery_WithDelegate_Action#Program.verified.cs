//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Program.cs
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject.Conventions;
public partial class Program
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public async Task RunAsync(string[] args, IConventionFactory? factory = null, Func<ConventionContextBuilder, CancellationToken, ValueTask>? action = null)
    {
        Func<ConventionContextBuilder, ValueTask> sourceAction = z =>
        {
            return ValueTask.CompletedTask;
        };
        var builder = Host.CreateApplicationBuilder(args);
        var host = await builder.ConfigureRocketSurgery(factory ?? Imports.Instance, async (builder, token) =>
        {
            await sourceAction(builder);
            await action(builder, token);
        });
        await host.RunAsync();
    }
}