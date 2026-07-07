using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Base class for `clavus-managed-configuration` generator tests. Mirrors
///     `Rocket.Surgery.Conventions.Analyzers.Tests.GeneratorTest`. Adds the config-pipeline
///     analog of <c>AddCommonReferences</c>/<c>AddCommonGenerators</c> so config-specific
///     generator stages (manifest emission, typed-class generation, `IConfigurationPart`
///     export) run alongside the base convention generators, matching the design's
///     "reuses the existing attribute-scanning/reference-walk infrastructure" decision.
/// </summary>
public abstract class ConfigGeneratorTest() : LoggerTest<TestRecord>(TestRecord.Create())
{
    protected GeneratorTestContextBuilder Builder { get; } = GeneratorTestContextBuilder
                                                                         .Create()
                                                                         .AddClavusConfiguration(importNamespace: "Sample", exportNamespace: "Sample")
                                                                         .AddGlobalOption("build_property.RootNamespace", "Sample")
                                                                         .AddConfigCommonReferences()
                                                                         .AddConfigCommonGenerators();

    protected GeneratorTestContextBuilder WithSharedDeps() => Builder.AddConfigSharedDeps(TestContext.CancellationToken);

    protected GeneratorTestContextBuilder Configure(Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder> builder) => builder(Builder);
}
