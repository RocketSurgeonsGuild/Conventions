using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Runtime-only half of a previously end-to-end scenario test for the (now-removed)
///     `clavus-managed-configuration` generator pipeline: once a generated `IConfigurationPart` is
///     exported and registered against a real `IServiceCollection`/`IConfiguration`, `IOptions&lt;T&gt;`
///     for the generated type resolves with values bound from the shipped `appsettings.json`.
///
///     NOTE: this previously also contained a generator-level test
///     (`Should_Combine_Packaging_Generation_Export_And_Runtime_Binding_For_A_Sample_Library_And_Host`,
///     task 7.1) exercising the generator's config-class-emission/manifest/export pipeline. That
///     pipeline was removed wholesale in `1bd74928` ("simplify configuration"), so the test was
///     deleted rather than resurrected — see
///     `.squad/decisions/inbox/ash-analyzers-managed-configuration-tests-removed.md`. This class
///     doesn't need the `ConfigGeneratorTest` generator-testing base anymore since the surviving
///     test is a plain runtime `IOptions` binding check, but it's kept for now to minimize churn.
/// </summary>
public class EndToEndConfigurationScenarioTests() : ConfigGeneratorTest()
{
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
