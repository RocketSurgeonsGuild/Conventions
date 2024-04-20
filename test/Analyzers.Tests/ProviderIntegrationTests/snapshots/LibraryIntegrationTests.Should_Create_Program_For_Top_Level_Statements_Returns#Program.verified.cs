//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Program.cs
using Rocket.Surgery.Conventions;
using System.Threading;
using System.Threading.Tasks;

public partial class Program
{
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public async Task<int> RunAsync(string[] args, IConventionFactory factory = null, Func<ConventionContextBuilder, CancellationToken, ValueTask> action = null)
    {
        var a = 1;
        if (true)
            return -1;
        return a = 1;
    }
}