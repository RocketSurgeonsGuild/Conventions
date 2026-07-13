using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Base class for generator tests that need `Clavus`-core references/generators wired up
///     (convention export/import, `IConfigurationPart`-implementing runtime conventions). Mirrors
///     `Rocket.Surgery.Conventions.Analyzers.Tests.GeneratorTest`.
///
///     NOTE: this previously also underpinned the `clavus-managed-configuration` generator tests
///     (config-discovery, type-inference, manifest-emission stages, via a now-removed
///     `WithSharedDeps()` helper). That generator pipeline was deleted in `1bd74928` ("simplify
///     configuration") along with every test exercising it — see
///     `.squad/decisions/inbox/ash-analyzers-managed-configuration-tests-removed.md`. The surviving
///     consumers of this base class (`MultiFormatConfigurationTests`,
///     `EndToEndConfigurationScenarioTests`) only use it for the plain `Builder`/`Configure` helpers.
/// </summary>
public abstract class ConfigGeneratorTest() : LoggerTest<TestRecord>(TestRecord.Create())
{
    protected GeneratorTestContextBuilder Builder { get; } = GeneratorTestContextBuilder
                                                                         .Create()
                                                                         .AddClavusConfiguration(importNamespace: "Sample", exportNamespace: "Sample")
                                                                         .AddGlobalOption("build_property.RootNamespace", "Sample")
                                                                         .AddConfigCommonReferences()
                                                                         .AddConfigCommonGenerators();

    protected GeneratorTestContextBuilder Configure(Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder> builder) => builder(Builder);
}
