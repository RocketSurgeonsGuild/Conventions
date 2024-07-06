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
        Func<ConventionContextBuilder, CancellationToken, ValueTask> sourceAction = (z, c) => ValueTask.CompletedTask;
        var builder = Host.CreateApplicationBuilder(args);
        var host = await builder.LaunchWith(RocketBooster.For(factory ?? Imports.Instance), async (builder, token) =>
        {
            await sourceAction(builder, token);
            await action(builder, token);
        });
        await host.RunAsync();
    }
}
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669

#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
