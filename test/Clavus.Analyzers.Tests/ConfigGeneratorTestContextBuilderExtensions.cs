using Clavus.Configuration.Toml;
using Clavus.Configuration.Yaml;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Mirrors `Rocket.Surgery.Conventions.Analyzers.Tests.GeneratorTestContextBuilderExtensions`
///     for the configuration pipeline. Adds references to `Clavus` core (for `IConfigurationPart`,
///     `IClavusPart`, and the `Clavus.ConfigurationAssembly` marker attribute per design.md
///     Decision 2/4) and picks up every `IIncrementalGenerator` in `Clavus.Analyzers`, which per
///     the design is where the config-discovery, type-inference, and manifest-emission stages live.
/// </summary>
internal static class ConfigGeneratorTestContextBuilderExtensions
{
    public static GeneratorTestContextBuilder AddConfigSharedDeps(this GeneratorTestContextBuilder builder, CancellationToken cancellationToken) =>
        builder.AddCompilationReferences(ConfigGenerationHelpers.CreateManifestDeps(builder, cancellationToken).GetAwaiter().GetResult());

    public static GeneratorTestContextBuilder AddConfigCommonReferences(this GeneratorTestContextBuilder builder) => builder.AddReferences(
        typeof(IConfigurationPart).Assembly,
        typeof(IClavusPart).Assembly,
        typeof(IServiceCollection).Assembly,
        typeof(IConfigurationBuilder).Assembly,
        typeof(OptionsServiceCollectionExtensions).Assembly,
        typeof(JsonConfigurationExtensions).Assembly,
        typeof(YamlConfigurationExtensions).Assembly,
        typeof(TomlConfigurationExtensions).Assembly,
        typeof(BinderOptions).Assembly
    );

    public static GeneratorTestContextBuilder AddConfigCommonGenerators(this GeneratorTestContextBuilder builder)
    {
        foreach (var generator in GetAllGenerators(typeof(ConfigGeneratorTestContextBuilderExtensions).Assembly.GetIndagoProvider()))
        {
            builder = builder.WithGenerator(generator);
        }

        return builder;
    }

    private static IEnumerable<Type> GetAllGenerators(IIndagoProvider provider) => provider.GetTypes(s => s
                                                                                              .FromAssemblyOf<ClavusAttributesGenerator>()
                                                                                              .GetTypes(f => f.WithAttribute<GeneratorAttribute>().AssignableTo<IIncrementalGenerator>())
        );
}
