using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class AssemblyProviderTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
"
                           )
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

public class TestConvention : IServiceAsyncConvention {
    public ValueTask Register(IConventionContext context, IServiceCollection services, CancellationToken cancellationToken)
    {
            var assemblies = context.AssemblyProvider.GetAssemblies(z => z.FromAssembly());
        return Task.CompletedTask;
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }
}
