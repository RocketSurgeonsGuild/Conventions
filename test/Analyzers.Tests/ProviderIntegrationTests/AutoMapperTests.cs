using AutoMapper;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests;

public class AutoMapperTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Work_With_AutoMapper()
    {
        var result = await WithSharedDeps()
                          .AddReferences(typeof(IMapper).Assembly)
                          .AddReferences(GetType().Assembly)
                          .AddSources(
                               @"
using AutoMapper;
using Rocket.Surgery.Conventions;

public class TestConvention : IServiceAsyncConvention {
    public ValueTask Register(IConventionContext context, IServiceCollection services, CancellationToken cancellationToken)
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
        return Task.CompletedTask;
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    private class Profile1 : Profile;

    private class Mapper : Profile;


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