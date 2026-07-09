using System.Diagnostics;
using System.Diagnostics.Metrics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Rocket.Surgery.Extensions.Testing;

namespace Clavus.OpenTelemetry.Tests;

/// <summary>
///     Runtime behavioral tests for the Clavus OpenTelemetry bridge (design.md Decision 4, "runtime behavioral layer").
///     Every test here spins a real host via <c>ConfigureClavus</c> and observes effects through the OpenTelemetry
///     in-memory exporter rather than asserting against internal state, per design.md's flakiness mitigation
///     ("use the in-memory exporter... force synchronous flush, and avoid network exporters").
/// </summary>
public partial class OpenTelemetryBridgeTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    // tasks.md 4.2
    [Test]
    public async Task Without_UseOpenTelemetry_AddOpenTelemetry_Is_Not_Called()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus();

        host.Services.GetService<TracerProvider>().ShouldBeNull();
        host.Services.GetService<MeterProvider>().ShouldBeNull();
        host.Services.GetService<LoggerProvider>().ShouldBeNull();
    }

    // tasks.md 4.3
    [Test]
    public async Task Sync_TracerProviderPart_Adds_ActivitySource_And_InMemoryExporter_Captures_Span()
    {
        const string sourceName = "Clavus.OpenTelemetry.Tests.Sync";
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

        // OpenTelemetry.Extensions.Hosting builds the real TracerProvider lazily, the first time it is
        // resolved from DI (or when the host is started). Resolve it before creating any activity so the
        // ActivityListener is actually wired up when `StartActivity` is called.
        var tracerProvider = host.Services.GetRequiredService<TracerProvider>();

        using var source = new ActivitySource(sourceName);
        using (var activity = source.StartActivity("test-span"))
        {
            activity.ShouldNotBeNull("the tracer part should have subscribed the source, otherwise no listener samples it");
        }

        tracerProvider.ForceFlush();

        exportedActivities.ShouldContain(a => a.DisplayName == "test-span");
    }

    // tasks.md 4.4
    [Test]
    public async Task Async_TracerProviderPart_Performs_Awaited_Work_Then_Adds_Source_Without_Deadlock()
    {
        const string sourceName = "Clavus.OpenTelemetry.Tests.Async";
        List<Activity> exportedActivities = [];

        // If the async-apply bridge (design Decision 2a) ever regresses to sync-over-async, this awaited
        // delay would deadlock the host build; the test completing at all is part of the assertion.
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(
                                   rb => rb
                                        .ConfigureTracerProvider(
                                             async (_, builder, cancellationToken) =>
                                             {
                                                 await Task.Delay(10, cancellationToken).ConfigureAwait(false);
                                                 builder.AddSource(sourceName).AddInMemoryExporter(exportedActivities);
                                             }
                                         )
                               );

        var tracerProvider = host.Services.GetRequiredService<TracerProvider>();

        using var source = new ActivitySource(sourceName);
        using (var activity = source.StartActivity("test-span"))
        {
            activity.ShouldNotBeNull();
        }

        tracerProvider.ForceFlush();

        exportedActivities.ShouldContain(a => a.DisplayName == "test-span");
    }

    // tasks.md 4.5 (metric)
    [Test]
    public async Task MeterProviderPart_Adds_Meter_And_InMemoryExporter_Captures_Metric()
    {
        const string meterName = "Clavus.OpenTelemetry.Tests.Meter";
        List<Metric> exportedMetrics = [];

        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(
                                   rb => rb
                                        .ConfigureMeterProvider(
                                             (_, builder) => builder
                                                             .AddMeter(meterName)
                                                             .AddInMemoryExporter(exportedMetrics)
                                         )
                               );

        var meterProvider = host.Services.GetRequiredService<MeterProvider>();

        using var meter = new Meter(meterName);
        var counter = meter.CreateCounter<int>("test.counter");
        counter.Add(1);

        meterProvider.ForceFlush();

        exportedMetrics.ShouldContain(m => m.Name == "test.counter");
    }

    // tasks.md 4.5 (log record)
    [Test]
    public async Task LoggerProviderPart_Adds_InMemoryExporter_Captures_LogRecord()
    {
        List<LogRecord> exportedLogRecords = [];

        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(
                                   rb => rb
                                        .ConfigureLoggerProvider((_, builder) => builder.AddInMemoryExporter(exportedLogRecords))
                               );

        var loggerProvider = host.Services.GetRequiredService<LoggerProvider>();
        var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Clavus.OpenTelemetry.Tests");
        logger.LogInformation("test-log-record");

        loggerProvider.ForceFlush();

        exportedLogRecords.ShouldContain(r => r.Body == "test-log-record");
    }

    // tasks.md 4.5 (resource attribute)
    // KNOWN FAILING as of this writing: `IOpenTelemetryBuilder.ConfigureResource` does not invoke its
    // callback synchronously/inline (unlike `WithTracing`/`WithMetrics`/`WithLogging`), so the
    // "capture-the-builder" bridge in RocketOpenTelemetryExtensions.UseOpenTelemetry captures a null
    // ResourceBuilder and this part throws ArgumentNullException instead of applying. Repro'd directly
    // against the bare OpenTelemetry.Extensions.Hosting API (no Clavus involved), so this contradicts
    // design.md's Context section claim that all five builder callbacks "run synchronously and
    // immediately at call time". See .squad/decisions/inbox/ash-clavus-opentelemetry-tests.md.
    // Left unskipped intentionally so it fails loudly until Decision 2a is revisited for ResourceBuilder.
    [Test]
    public async Task ResourceBuilderPart_Adds_Attribute_Present_On_Built_Providers()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(
                                   rb => rb
                                        .ConfigureResourceBuilder(
                                             (_, builder) => builder.AddAttributes(
                                                 [new KeyValuePair<string, object>("test.resource.attribute", "test-value")]
                                             )
                                         )
                               );

        var resource = host.Services.GetRequiredService<TracerProvider>().GetResource();

        resource.Attributes.ShouldContain(new KeyValuePair<string, object>("test.resource.attribute", "test-value"));
    }

    // tasks.md 4.6
    [Test]
    public async Task Two_Parts_Same_Kind_Different_Priorities_Apply_In_Ascending_Priority_Order()
    {
        List<string> order = [];

        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(
                                   rb => rb
                                        .ConfigureTracerProvider((_, _) => order.Add("high-priority-runs-second"), priority: 10)
                                        .ConfigureTracerProvider((_, _) => order.Add("low-priority-runs-first"), priority: -10)
                               );

        _ = host;

        order.ShouldBe(["low-priority-runs-first", "high-priority-runs-second"]);
    }

    // tasks.md 4.7
    [Test]
    public async Task ClavusExport_Annotated_TracerPart_Is_Auto_Discovered_Without_Manual_Configure()
    {
        List<Activity> exportedActivities = [];

        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(
                                   rb => rb
                                        .ConfigureTracerProvider((_, builder) => builder.AddInMemoryExporter(exportedActivities))
                               );

        var tracerProvider = host.Services.GetRequiredService<TracerProvider>();

        using var source = new ActivitySource(OpenTelemetryFixtures.AutoDiscoveredSourceName);
        using (var activity = source.StartActivity("auto-discovered-span"))
        {
            activity.ShouldNotBeNull(
                "the [ClavusExport]-annotated AutoDiscoveredTracerPart should have been discovered and applied without an explicit ConfigureTracerProvider call"
            );
        }

        tracerProvider.ForceFlush();

        exportedActivities.ShouldContain(a => a.DisplayName == "auto-discovered-span");
    }
}
