using FluentValidation;

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.DependencyInjection.Compiled;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

internal static class GeneratorTestContextBuilderExtensions
{
    public static GeneratorTestContextBuilder AddSharedDeps(this GeneratorTestContextBuilder builder) => builder.AddCompilationReferences(GenerationHelpers.CreateDeps(builder).GetAwaiter().GetResult());

    public static GeneratorTestContextBuilder AddSharedGenericDeps(this GeneratorTestContextBuilder builder) => builder.AddCompilationReferences(GenerationHelpers.CreateGenericDeps(builder).GetAwaiter().GetResult());

    public static GeneratorTestContextBuilder AddCommonReferences(this GeneratorTestContextBuilder builder) => builder.AddReferences(
        typeof(ActivatorUtilities),
        typeof(ConventionContext),
        typeof(IConventionContext),
        typeof(IServiceProvider),
        typeof(IConfiguration),
        typeof(IValidator)
        );

    public static GeneratorTestContextBuilder AddCommonGenerators(this GeneratorTestContextBuilder builder)
    {
        foreach (var generator in GetAllGenerators(typeof(GeneratorTestContextBuilderExtensions).Assembly.GetCompiledTypeProvider()))
        {
            builder = builder.WithGenerator(generator);
        }

        return builder;
    }

    private static IEnumerable<Type> GetAllGenerators(ICompiledTypeProvider provider) => provider.GetTypes(s => s
                                                                                              .FromAssemblyOf<ConventionAttributesGenerator>()
                                                                                              .GetTypes(f => f.WithAttribute<GeneratorAttribute>().AssignableTo<IIncrementalGenerator>())
        );
}
