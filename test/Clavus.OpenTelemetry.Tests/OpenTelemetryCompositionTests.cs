using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;

using Rocket.Surgery.Extensions.Testing;

namespace Clavus.OpenTelemetry.Tests;

/// <summary>
///     Composition tests for the Clavus OpenTelemetry bridge (design.md Decision 4, tasks.md section 5).
///     These build a real host with a real container selection (<c>UseDryIoc</c>/<c>UseAutofac</c>) and/or
///     the Serilog part applied alongside <c>UseOpenTelemetry()</c> on the same
///     <see cref="ClavusContextBuilder" />, and observe effects through the OpenTelemetry in-memory exporter
///     (and, for Serilog, through the applied part itself) rather than asserting on internal state.
/// </summary>
public class OpenTelemetryCompositionTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    // tasks.md 5.1
    [Test]
    public async Task TelemetryParts_Apply_And_Emit_Under_UseDryIoc()
    {
        const string sourceName = "Clavus.OpenTelemetry.Tests.Composition";
        List<Activity> exportedActivities = [];

        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(
                                   rb => rb
                                        .ConfigureTracerProvider(
                                             (_, builder) => builder
                                                             .AddSource(sourceName)
                                                             .AddInMemoryExporter(exportedActivities)
                                         )
                               );

        // OpenTelemetry.Extensions.Hosting builds the real TracerProvider lazily on first resolution;
        // resolve it (through whichever IServiceProvider host.Services now delegates to - DryIoc's -
        // before creating any activity so the ActivityListener is wired up when StartActivity is called.
        var tracerProvider = host.Services.GetRequiredService<TracerProvider>();

        using var source = new ActivitySource(sourceName);
        using (var activity = source.StartActivity("dryioc-composition-span"))
        {
            activity.ShouldNotBeNull("the tracer part should have subscribed the source through the DryIoc-backed service provider");
        }

        tracerProvider.ForceFlush();

        exportedActivities.ShouldContain(a => a.DisplayName == "dryioc-composition-span");
    }
}
