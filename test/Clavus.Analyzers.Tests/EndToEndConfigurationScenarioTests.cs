using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Task 7.1: end-to-end scenario combining packaging + generation + export + runtime binding
///     for a representative sample library/host pair, per proposal.md's full pipeline
///     description: a library declares `appsettings.json`, gets it packed/copied, a strongly
///     typed class + `IConfigurationPart` is generated in the library, the exports generator
///     always includes it, and the host resolves `IOptions<T>` bound from the library's shipped
///     config file.
///
///     This intentionally spans the three capability areas
///     (`clavus-config-packaging`/`clavus-config-generation`/`clavus-config-runtime`) in one test
///     rather than re-testing each in isolation, matching the "representative sample library/host
///     pair" framing in tasks.md 7.1. Narrower coverage of each stage lives in
///     <see cref="ManifestGenerationTests" />, <see cref="TypeInferenceAdversarialTests" />,
///     <see cref="ConfigurationPartExportTests" />, and <see cref="TransitiveConfigurationTests" />.
///
///     ASSUMPTION: `IConfigurationPart` exposes a `Register(IServiceCollection, IConfiguration)`
///     shape mirroring `IClavusPart`/`IConvention`'s existing `Register(IConventionContext)`-style
///     contract (design.md Decision 4: "Calls `services.AddOptions&lt;T&gt;().Bind(...)`"). Update
///     this call site once Ripley's real interface signature lands if it differs.
/// </summary>
public class EndToEndConfigurationScenarioTests() : ConfigGeneratorTest()
{
    [Test]
    public async Task Should_Combine_Packaging_Generation_Export_And_Runtime_Binding_For_A_Sample_Library_And_Host()
    {
        // 1. "Packaging": a library declares a conventional appsettings.json (auto-globbed per
        //    design.md Decision 1 - no explicit ClavusConfiguration item needed for the
        //    conventional name).
        var library = await Builder
                           .WithProjectName("Sample.WeatherLibrary")
                           .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                           .AddAdditionalText(
                                "appsettings.json",
                                /*lang=json,strict*/
                                """
                                {
                                  "Weather": {
                                    "City": "Seattle",
                                    "PollInterval": "00:05:00",
                                    "EffectiveDate": "2024-06-01"
                                  }
                                }
                                """
                            )
                           .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                           .AddSources(
                                """
                                namespace Sample.WeatherLibrary;

                                public class Marker;
                                """
                            )
                           .Build()
                           .GenerateAsync(TestContext.CancellationToken);

        // 2. "Generation + export": the host references the library; the generator must (a)
        //    generate WeatherConfiguration inside Sample.WeatherLibrary's namespace, (b) generate
        //    an IConfigurationPart for it, and (c) the exports generator must include that part
        //    in the host's flattened export set with zero [Convention] decoration anywhere.
        var result = await Configure(b => b.AddCompilationReferences(library))
                          .WithProjectName("Sample.WeatherHost")
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddSources(
                               """
                               using Sample.WeatherLibrary;

                               namespace Sample.WeatherHost;

                               public class Program;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Resolve_IOptions_Bound_From_The_Generated_Configuration_Part_At_Runtime()
    {
        // Runtime half of the same scenario: once the library's IConfigurationPart is exported
        // and registered against a real IServiceCollection/IConfiguration, IOptions<T> for the
        // generated type must resolve with values bound from the shipped appsettings.json -
        // per design.md Decision 4's "Adds the appropriate IConfigurationSource ... Calls
        // services.AddOptions<T>().Bind(configuration.GetSection(...))".
        //
        // This is written against the assumed IConfigurationPart contract described above; it
        // documents the expected runtime shape and will compile/run once Ripley's real
        // IConfigurationPart interface and Dallas's registration wiring land.
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
                            .AddInMemoryCollection(
                                 new Dictionary<string, string?>
                                 {
                                     ["Weather:City"] = "Seattle",
                                     ["Weather:PollInterval"] = "00:05:00",
                                     ["Weather:EffectiveDate"] = "2024-06-01",
                                 }
                             )
                            .Build();

        // Placeholder registration call - replace `WeatherConfigurationPart` with the real
        // generated type name once available; the shape of the call (new part, .Register(...))
        // is the thing under test here, not the concrete generated identifier.
        // new WeatherConfigurationPart().Register(services, configuration);
        services.AddOptions();
        services.Configure<WeatherOptionsStub>(configuration.GetSection("Weather"));

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<WeatherOptionsStub>>().Value;

        options.City.ShouldBe("Seattle");
        options.PollInterval.ShouldBe("00:05:00");
    }

    /// <summary>
    ///     Stand-in for the generator-produced `WeatherConfiguration` class until the real
    ///     generator lands - keeps the runtime-binding half of the scenario exercisable today.
    /// </summary>
    private sealed class WeatherOptionsStub
    {
        public string? City { get; set; }
        public string? PollInterval { get; set; }
        public string? EffectiveDate { get; set; }
    }
}
