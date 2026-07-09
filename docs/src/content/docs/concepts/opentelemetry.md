---
title: OpenTelemetry
description: Opt-in OpenTelemetry integration that exposes tracing, metrics, logging, and resource builders as Clavus parts.
tags: [opentelemetry, observability, tracing, metrics, logging]
---

# OpenTelemetry

`Clavus.OpenTelemetry` exposes OpenTelemetry's configuration surface — the resource builder, the
tracer/meter/logger providers, and the top-level `IOpenTelemetryBuilder` — as ordinary Clavus
[parts](/concepts/defining-conventions/). Any library can contribute an activity source, a meter,
a log processor, or a resource attribute without taking a direct dependency on how (or whether) the
host wires up OpenTelemetry.

> [!NOTE]
> This integration is entirely opt-in. Referencing `Clavus.OpenTelemetry` does **not** enable
> OpenTelemetry on its own — a host application must call `UseOpenTelemetry()` explicitly. Nothing
> auto-enables telemetry as a side effect of adding the package.

## The five part kinds

`Clavus.OpenTelemetry` declares five `<ClavusPart>` items, one per OpenTelemetry builder. Each
follows the same generated shape as every other Clavus part: a sync interface, an async interface
(`I{Kind}AsyncPart`), matching delegate types, a `ClavusContextBuilder.Configure{Kind}(...)`
builder method, and an `Apply{Kind}` runtime extension.

| Part interface         | Async variant               | Builder type                                 |
| ---------------------- | --------------------------- | -------------------------------------------- |
| `IOpenTelemetryPart`   | `IOpenTelemetryAsyncPart`   | `OpenTelemetry.IOpenTelemetryBuilder`        |
| `IResourceBuilderPart` | `IResourceBuilderAsyncPart` | `OpenTelemetry.Resources.ResourceBuilder`    |
| `ITracerProviderPart`  | `ITracerProviderAsyncPart`  | `OpenTelemetry.Trace.TracerProviderBuilder`  |
| `IMeterProviderPart`   | `IMeterProviderAsyncPart`   | `OpenTelemetry.Metrics.MeterProviderBuilder` |
| `ILoggerProviderPart`  | `ILoggerProviderAsyncPart`  | `OpenTelemetry.Logs.LoggerProviderBuilder`   |

Splitting the builders into distinct part kinds means a library that only wants to register an
activity source can implement `ITracerProviderPart` without knowing anything about
`WithMetrics`/`WithLogging`/resource plumbing — the same "narrow, composable extension point"
philosophy the rest of Clavus follows.

## Apply order

When `UseOpenTelemetry()` runs, the five kinds are applied in a fixed order, with each kind's own
parts running in the executor's usual priority order:

```
OpenTelemetry → ResourceBuilder → TracerProvider → MeterProvider → LoggerProvider
```

The top-level `IOpenTelemetryPart` runs first (it's the natural place for cross-cutting setup that
doesn't belong to a single provider), then the resource builder (so resource attributes are in
place before providers are configured), then tracing, metrics, and logging in that order.

## Enabling it

Call `UseOpenTelemetry()` on the `ClavusContextBuilder` during host setup:

```c#
using Clavus;

var builder = Host.CreateApplicationBuilder(args);

builder.UseClavus(context => context.UseOpenTelemetry());
```

Every `[ClavusExport]`-annotated part (and anything registered explicitly via
`Configure{Kind}(...)`) is discovered and applied automatically — no separate registration step is
needed beyond the one `UseOpenTelemetry()` call.

## Example: contributing an activity source

A library adds a `System.Diagnostics.ActivitySource` to tracing by implementing
`ITracerProviderPart` and marking the class `[ClavusExport]`, the same pattern used for any other
auto-discovered Clavus part:

```c#
using Clavus;
using OpenTelemetry.Trace;

namespace MyLibrary;

[ClavusExport]
public class MyLibraryTracing : ITracerProviderPart
{
    public static readonly ActivitySource ActivitySource = new("MyLibrary");

    public void Register(IClavusContext context, TracerProviderBuilder builder) =>
        builder.AddSource(ActivitySource.Name);
}
```

For work that needs to run asynchronously (for example, resolving exporter configuration from a
remote source before registering it), implement `ITracerProviderAsyncPart` instead — Clavus runs
the async apply phase before the OpenTelemetry provider is built, so there's no
`.GetAwaiter().GetResult()` anywhere in the pipeline.

```c#
[ClavusExport]
public class MyLibraryAsyncTracing : ITracerProviderAsyncPart
{
    public async ValueTask Register(IClavusContext context, TracerProviderBuilder builder, CancellationToken cancellationToken)
    {
        var samplingRate = await LoadSamplingRateAsync(cancellationToken);
        builder.SetSampler(new TraceIdRatioBasedSampler(samplingRate));
    }
}
```

## What this package does _not_ do

`Clavus.OpenTelemetry` only opens the extension points — it does not ship any exporters or
instrumentation. In particular, it does **not** include:

- The console exporter, the OTLP exporter, or any other exporter package.
- ASP.NET Core, HttpClient, or other instrumentation packages.
- Any default sampler, resource attribute, or processor configuration.

Consumers add exporters and instrumentation the same way they add anything else to a provider:
by writing their own `ITracerProviderPart`/`IMeterProviderPart`/`ILoggerProviderPart`
implementation (or a plain `Configure{Kind}(...)` delegate) that calls the relevant
`AddConsoleExporter()`, `AddOtlpExporter()`, `AddAspNetCoreInstrumentation()`, etc. This keeps
`Clavus.OpenTelemetry` itself free of exporter dependencies, which also keeps it friendly to
trimming/AOT — see [Non-Goals in the OpenTelemetry design doc](https://github.com/RocketSurgeonsGuild/Conventions/blob/main/openspec/changes/add-clavus-opentelemetry/design.md#goals--non-goals)
for the full rationale.
