using AutoMapper;
using MediatR;
using Microsoft.CodeAnalysis;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests;

public class LibraryIntegrationTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Create_Program_For_Top_Level_Statements()
    {
        var result = await Builder
                          .AddSources(
                               """"
                               var a = 1;
                               if (true) return;
                               """"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Create_Program_For_Top_Level_Statements_Returns()
    {
        var result = await Builder
                          .AddSources(
                               """"
                               var a = 1;
                               if (true) return -1;
                               return a = 1;
                               """"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Theory]
    [MemberData(nameof(Should_Rewrite_Program_For_Top_Level_Statements_Returns_To_Inject_Conventions_Data))]
    public async Task Should_Rewrite_Program_For_Top_Level_Statements_Returns_To_Inject_Conventions(string name, string source)
    {
        var result = await Builder
                          .AddSources(source)
                          .Build()
                          .GenerateAsync();

        await Verify(result).UseTextForParameters(name);
    }

    public static IEnumerable<object[]> Should_Rewrite_Program_For_Top_Level_Statements_Returns_To_Inject_Conventions_Data()
    {
        yield return
        [
            "LaunchWith_WithDelegate",
            """"
            using Microsoft.Extensions.Hosting;

            var builder = Host.CreateApplicationBuilder(args);
            var host = await builder.LaunchWith(RocketBooster.For(Imports.Instance), (z, c) => ValueTask.CompletedTask);
            await host.RunAsync();
            """"
        ];
        yield return
        [
            "LaunchWith",
            """"
            using Microsoft.Extensions.Hosting;

            var builder = Host.CreateApplicationBuilder(args);
            var host = await builder.LaunchWith(RocketBooster.For(Imports.Instance));
            await host.RunAsync();
            """"
        ];
        yield return
        [
            "UseRocketBooster_WithDelegate",
            """"
            using Microsoft.Extensions.Hosting;

            var builder = Host.CreateApplicationBuilder(args);
            var host = await builder.UseRocketBooster(RocketBooster.For(Imports.Instance), (z, c) => ValueTask.CompletedTask);
            await host.RunAsync();
            """"
        ];
        yield return
        [
            "UseRocketBooster",
            """"
            using Microsoft.Extensions.Hosting;

            var builder = Host.CreateApplicationBuilder(args);
            var host = await builder.UseRocketBooster(RocketBooster.For(Imports.Instance));
            await host.RunAsync();
            """"
        ];
        yield return
        [
            "ConfigureRocketSurgery_WithDelegate",
            """"
            using Microsoft.Extensions.Hosting;

            var builder = Host.CreateApplicationBuilder(args);
            var host = await builder.ConfigureRocketSurgery(Imports.Instance, (z, c) => ValueTask.CompletedTask);
            await host.RunAsync();
            """"
        ];
        yield return
        [
            "ConfigureRocketSurgery_WithDelegate_Without_CancellationToken",
            """"
            using Microsoft.Extensions.Hosting;

            var builder = Host.CreateApplicationBuilder(args);
            var host = await builder.ConfigureRocketSurgery(Imports.Instance, z => ValueTask.CompletedTask);
            await host.RunAsync();
            """"
        ];
        yield return
        [
            "ConfigureRocketSurgery_WithDelegate_Action",
            """"
            using Microsoft.Extensions.Hosting;

            var builder = Host.CreateApplicationBuilder(args);
            var host = await builder.ConfigureRocketSurgery(Imports.Instance, z => {});
            await host.RunAsync();
            """"
        ];
        yield return
        [
            "ConfigureRocketSurgery",
            """"
            using Microsoft.Extensions.Hosting;

            var builder = Host.CreateApplicationBuilder(args);
            var host = await builder.ConfigureRocketSurgery(Imports.Instance);
            await host.RunAsync();
            """"
        ];
    }

    [Fact]
    public async Task Should_Work_With_AutoMapper()
    {
        var result = await WithSharedDeps()
                          .AddReferences(typeof(IMapper).Assembly)
                          .AddReferences(GetType().Assembly)
                          .AddSources(
                               @"
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

public class TestConvention : IServiceAsyncConvention {
    public ValueTask Register(IConventionContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken)
    {
        var provider = context.AssemblyProvider;
        var assemblies = provider.GetTypes(z => z.FromAssemblyDependenciesOf<IMapper>().GetTypes(f => f.AssignableTo<Profile>()));
        var assemblies2 = provider.GetTypes(z => z.FromAssemblyDependenciesOf<IMapper>().GetTypes(f => f.AssignableToAny(
                              typeof(IValueResolver<,,>),
                              typeof(IMemberValueResolver<,,,>),
                              typeof(ITypeConverter<,>),
                              typeof(IValueConverter<,>),
                              typeof(IMappingAction<,>)
                          )
                         .NotInfoOf(TypeInfoFilter.Abstract)
                         .KindOf(TypeKindFilter.Class)));
        return ValueTask.CompletedTask;
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Work_With_Interface_Assignability()
    {
        var targetDep = await Builder
                             .WithProjectName("targetDp")
                             .AddReferences(typeof(IMediator))
                             .AddReferences(typeof(INotification))
                             .AddSources(
                                  """"
                                  using MediatR;
                                  namespace Syndicates.Core.Support;

                                  public interface IResponseEvent : INotification;
                                  """"
                              )
                             .GenerateAsync();
        targetDep.EnsureDiagnosticSeverity(DiagnosticSeverity.Error);
        var depWithEvents = await Builder
                                 .WithProjectName("depWithEvents")
                                 .AddCompilationReferences(targetDep)
                                 .AddReferences(targetDep.FinalCompilation.References.ToArray())
                                 .AddSources(
                                      """"
                                      using Syndicates.Core.Support;
                                      namespace OtherDep;

                                      public partial record CreatedNotification() : IResponseEvent;
                                      public partial record UpdatedNotification() : IResponseEvent;
                                      """"
                                  )
                                 .GenerateAsync();
        depWithEvents.EnsureDiagnosticSeverity(DiagnosticSeverity.Error);
        var intermediate = await Builder
                                .WithProjectName("intermediate")
                                .AddReferences(depWithEvents.FinalCompilation.References.ToArray())
                                .AddCompilationReferences(depWithEvents)
                                .AddSources(
                                     @"
using MediatR;
using Syndicates.Core.Support;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

public class TestConvention : IServiceAsyncConvention {
    public ValueTask Register(IConventionContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken)
    {
        var provider = context.AssemblyProvider;
        var assemblies = provider.GetTypes(z => z.FromAssemblyDependenciesOf<IResponseEvent>().GetTypes(f => f.AssignableToAny(typeof(IResponseEvent))));
        return ValueTask.CompletedTask;
    }
}
"
                                 )
                                .Build()
                                .GenerateAsync();
        intermediate.EnsureDiagnosticSeverity(DiagnosticSeverity.Error);
        var result = await Builder
                          .AddReferences(intermediate.FinalCompilation.References.ToArray())
                          .AddCompilationReferences(intermediate)
                          .Build()
                          .GenerateAsync();
        await Verify(result);
    }

    private class Profile1 : Profile;

    private class Mapper : Profile;

    public class DocumentCreatedByValueResolver<TSource, TDestination> : DocumentStringMetadataValueResolver<TSource, TDestination>;

    public abstract class DocumentStringMetadataValueResolver<TSource, TDestination> : IValueResolver<TSource, TDestination, string>
    {
        protected virtual string GetValue(string metadataValue)
        {
            throw new NotImplementedException();
        }

        public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }


    private class A : IValueResolver<Source, Destination, string>
    {
        public string Resolve(Source source, Destination destination, string destMember, ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    private abstract class B : IMemberValueResolver<Source, Destination, string, string>
    {
        public string Resolve(Source source, Destination destination, string sourceMember, string destMember, ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    private class C : ITypeConverter<Source, Destination>
    {
        public Destination Convert(Source source, Destination destination, ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    private class D : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    private class E : IMappingAction<Source, Destination>
    {
        public void Process(Source source, Destination destination, ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    private class Source
    {
        public string Name { get; set; }
    }

    private class Destination
    {
        public string Name { get; set; }
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

public class MediatRTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
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
