using Clavus.Configuration.Toml;
using Clavus.Configuration.Yaml;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Mirrors `Rocket.Surgery.Conventions.Analyzers.Tests.GeneratorTestContextBuilderExtensions`.
///     Adds references to `Clavus` core (for `IConfigurationPart` — now the interface implemented by
///     the hand-authored `JsonConvention`/`YamlConvention`/`TomlConvention` runtime conventions,
///     not a generator-emitted type — and `IClavusPart`) and picks up every `IIncrementalGenerator`
///     in `Clavus.Analyzers` for the surviving generator tests in this project (convention
///     export/import; the `clavus-managed-configuration` config-discovery/type-inference/manifest
///     generator stages this class previously also wired up were removed in `1bd74928`).
/// </summary>
internal static class ConfigGeneratorTestContextBuilderExtensions
{
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
