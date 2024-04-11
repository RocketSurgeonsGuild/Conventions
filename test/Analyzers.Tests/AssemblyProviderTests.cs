using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
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

    [Theory]
    [MemberData(nameof(GetTypesTestsData.GetTypesData), MemberType = typeof(GetTypesTestsData))]
    public async Task Should_Generate_Assembly_Provider_For_GetTypes(GetTypesTestsData.GetTypesItem getTypesItem)
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               $$$""""
                               using Rocket.Surgery.Conventions;
                               using Rocket.Surgery.Conventions.Configuration;
                               using Rocket.Surgery.Conventions.DependencyInjection;
                               using Rocket.Surgery.Conventions.Reflection;
                               using Microsoft.Extensions.Configuration;
                               using Microsoft.Extensions.DependencyInjection;
                               using System.Threading;
                               using System.Threading.Tasks;
                               using System;

                               public class TestConvention : IServiceAsyncConvention
                               {
                                   public ValueTask Register(IConventionContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken)
                                   {
                                       var assemblies = context.AssemblyProvider.GetTypes({{{getTypesItem.Expression}}});
                                       return ValueTask.CompletedTask;
                                   }
                               }
                               """"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result).UseHashedParameters(getTypesItem.Name);
    }


    [Theory]
    [MemberData(nameof(GetTypesTestsData.GetTypesData), MemberType = typeof(GetTypesTestsData))]
    public async Task Should_Generate_Assembly_Provider_For_GetTypes_From_Another_Assembly(GetTypesTestsData.GetTypesItem getTypesItem)
    {
        var other = await WithSharedDeps()
                         .AddSources(
                              $$$""""
                              using Rocket.Surgery.Conventions;
                              using Rocket.Surgery.Conventions.Configuration;
                              using Rocket.Surgery.Conventions.DependencyInjection;
                              using Rocket.Surgery.Conventions.Reflection;
                              using Microsoft.Extensions.Configuration;
                              using Microsoft.Extensions.DependencyInjection;
                              using System.Threading;
                              using System.Threading.Tasks;
                              using System;

                              public class TestConvention : IServiceAsyncConvention
                              {
                                  public ValueTask Register(IConventionContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken)
                                  {
                                      var assemblies = context.AssemblyProvider.GetTypes({{{getTypesItem.Expression}}});
                                      return ValueTask.CompletedTask;
                                  }
                              }
                              """"
                          )
                         .Build()
                         .GenerateAsync();

        var diags = other.FinalDiagnostics.Where(x => x.Severity >= DiagnosticSeverity.Error).ToArray();
        if (diags.Length > 0)
        {
            await Verify(diags).UseHashedParameters(getTypesItem.Name);
        }
        other.EnsureDiagnosticSeverity(DiagnosticSeverity.Error);

        var result = await Builder
                          .AddCompilationReferences(other)
                          .Build()
                          .GenerateAsync();

        await Verify(result).UseHashedParameters(getTypesItem.Name);
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
