namespace Clavus.Analyzers.Tests;

/// <summary>
///     Task 4.4: Verify snapshot tests confirming generated `IConfigurationPart`s appear in the
///     export set without any attribute decoration, per design.md Decision 4 - the exports
///     generator's existing attribute-driven scan is extended with an unconditional "always
///     include generated `IConfigurationPart` types" step, because these parts are
///     generator-authored rather than user-decorated with `[Convention]`/`[ExportConvention]`.
///
///     Mirrors `Rocket.Surgery.Conventions.Analyzers.Tests.ExportedConventionsGenericTests`, but
///     the config file is the *only* thing declared - no `[ExportConvention]`-attributed class
///     anywhere in source - so a passing snapshot demonstrates the "no attribute decoration
///     required" contract, not just that export in general still works.
/// </summary>
public class ConfigurationPartExportTests() : ConfigGeneratorTest()
{
    [Test]
    public async Task Should_Include_Generated_IConfigurationPart_In_Export_Set_With_No_Convention_Attributes_Present()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Sample": { "Name": "value" } }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .AddSources(
                               """
                               namespace Sample.NoConventions;

                               // Deliberately contains no [Convention]/[ExportConvention]-attributed types anywhere -
                               // the generated IConfigurationPart must still show up in the export set.
                               public class Unrelated;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Include_Generated_IConfigurationPart_Alongside_A_Regular_Exported_Convention()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Sample": { "Name": "value" } }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .AddSources(
                               """
                               using Rocket.Surgery.Conventions;

                               namespace Sample.WithConvention;

                               [ExportConvention]
                               internal class Contrib : IConvention { }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // Both the attribute-driven convention export and the unconditional IConfigurationPart
        // export step must contribute to the same export set without one suppressing the other.
        await Verify(result);
    }

    [Test]
    public async Task Should_Not_Emit_IConfigurationPart_When_No_Configuration_Files_Declared()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddSources(
                               """
                               namespace Sample.NoConfig;

                               public class Unrelated;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        // No ClavusConfiguration items -> no IConfigurationPart should be generated or exported.
        await Verify(result);
    }
}
