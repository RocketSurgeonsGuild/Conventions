using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class AssemblyProviderTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Generate_Assembly_Provider_For_Self_Assembly()
    {
        var result = await WithSharedDeps()
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

    [Fact]
    public async Task Should_Generate_Assembly_Provider_For_Specific_Assembly()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

public class TestConvention : IServiceAsyncConvention {
    public ValueTask Register(IConventionContext context, IServiceCollection services, CancellationToken cancellationToken)
    {
            var assemblies = context.AssemblyProvider.GetAssemblies(z => z.FromAssemblyOf<IServiceAsyncConvention>());
            var assemblies = context.AssemblyProvider.GetAssemblies(z => z.FromAssemblyOf(typeof(IServiceAsyncConvention)));
        return Task.CompletedTask;
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Assembly_Provider_For_Specific_Assembly_Dependencies()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

public class TestConvention : IServiceAsyncConvention {
    public ValueTask Register(IConventionContext context, IServiceCollection services, CancellationToken cancellationToken)
    {
            var assemblies = context.AssemblyProvider.GetAssemblies(z => z.FromAssemblyDependenciesOf<IConvention>());
            var assemblies = context.AssemblyProvider.GetAssemblies(z => z.FromAssemblyDependenciesOf(typeof(IConvention)));
        return Task.CompletedTask;
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_Assembly_Provider_For_Duplicate_Lines_Assembly()
    {
        var result = await WithSharedDeps()
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
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

public class TestConvention2 : IServiceAsyncConvention {
    public ValueTask Register(IConventionContext context, IServiceCollection services, CancellationToken cancellationToken)
    {
            var assemblies = context.AssemblyProvider.GetAssemblies(z => z.FromAssemblies());
        return Task.CompletedTask;
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Configure(
            z => z
                .AddSources(
                     @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
"
                 )
        );
    }
}
