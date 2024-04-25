using System.Collections.Immutable;
using System.Reflection;
using FluentValidation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

internal static class GeneratorTestContextBuilderExtensions
{
    public static GeneratorTestContextBuilder AddSharedDeps(this GeneratorTestContextBuilder builder)
    {
        return builder.AddCompilationReferences(GenerationHelpers.CreateDeps(builder).GetAwaiter().GetResult());
    }

    public static GeneratorTestContextBuilder AddSharedGenericDeps(this GeneratorTestContextBuilder builder)
    {
        return builder.AddCompilationReferences(GenerationHelpers.CreateGenericDeps(builder).GetAwaiter().GetResult());
    }

    public static GeneratorTestContextBuilder AddCommonReferences(this GeneratorTestContextBuilder builder)
    {
        return builder.AddReferences(
            typeof(ActivatorUtilities).Assembly,
            typeof(ConventionContext).Assembly,
            typeof(IConventionContext).Assembly,
            typeof(IServiceProvider).Assembly,
            typeof(IConfiguration).Assembly,
            typeof(IValidator).Assembly
        );
    }

    public static GeneratorTestContextBuilder AddCommonGenerators(this GeneratorTestContextBuilder builder)
    {
        foreach (var generator in AllGenerators)
        {
            builder = builder.WithGenerator(generator);
        }

        return builder;
    }


    private static ImmutableArray<Type> AllGenerators { get; } = typeof(ConventionAttributesGenerator)
                                                                .Assembly.GetTypes()
                                                                .Where(
                                                                     z => typeof(IIncrementalGenerator).IsAssignableFrom(z)
                                                                      && z.GetCustomAttributes<GeneratorAttribute>().Any()
                                                                 )
                                                                .ToImmutableArray();
}