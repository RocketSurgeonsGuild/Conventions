using FluentValidation;

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Clavus.Analyzers.Tests;

internal static class GeneratorTestContextBuilderExtensions
{
    public static GeneratorTestContextBuilder AddSharedDeps(this GeneratorTestContextBuilder builder, CancellationToken cancellationToken) => builder.AddCompilationReferences(GenerationHelpers.CreateDeps(builder, cancellationToken).GetAwaiter().GetResult());

    public static GeneratorTestContextBuilder AddSharedGenericDeps(this GeneratorTestContextBuilder builder, CancellationToken cancellationToken) => builder.AddCompilationReferences(GenerationHelpers.CreateGenericDeps(builder, cancellationToken).GetAwaiter().GetResult());

    /// <summary>
    ///     Supplies the Clavus import/export configuration build properties that the Clavus MSBuild targets
    ///     (<c>Clavus.targets</c>) provide during a real build. The generator reads these directly, so the test
    ///     harness must mirror them or the generated code resolves to <c>##??NOT DEFINED??##</c> and fails to compile.
    /// </summary>
    public static GeneratorTestContextBuilder AddClavusConfiguration(
        this GeneratorTestContextBuilder builder,
        string importNamespace,
        string exportNamespace,
        string importClassName = "Imports",
        string importMethodName = "Ashlar",
        string exportClassName = "Exports",
        string exportMethodName = "Ashlar"
    ) => builder
        .AddGlobalOption("build_property.ClavusImportNamespace", importNamespace)
        .AddGlobalOption("build_property.ClavusImportClassName", importClassName)
        .AddGlobalOption("build_property.ClavusImportMethodName", importMethodName)
        .AddGlobalOption("build_property.ClavusExportNamespace", exportNamespace)
        .AddGlobalOption("build_property.ClavusExportClassName", exportClassName)
        .AddGlobalOption("build_property.ClavusExportMethodName", exportMethodName);

    public static GeneratorTestContextBuilder AddCommonReferences(this GeneratorTestContextBuilder builder) => builder.AddReferences(
        typeof(ActivatorUtilities),
        typeof(ClavusContext),
        typeof(IServiceProvider),
        typeof(IConfiguration),
        typeof(IValidator)
        );

    public static GeneratorTestContextBuilder AddCommonGenerators(this GeneratorTestContextBuilder builder)
    {
        foreach (var generator in GetAllGenerators(typeof(GeneratorTestContextBuilderExtensions).Assembly.GetIndagoProvider()))
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
