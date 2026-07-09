## Why

Clavus has convention "parts" for services, configuration, logging, containers (DryIoc/Autofac), and Serilog, but no first-class extension point for OpenTelemetry. Consumers who want to contribute tracing, metrics, logging, or resource configuration through conventions currently have to hand-roll `AddOpenTelemetry()` wiring in each app, defeating the point of the convention system. Telemetry is a cross-cutting concern that fits the part model exactly — many independent libraries each want to register instrumentation/exporters without knowing about each other.

## What Changes

- Add a new `Clavus.OpenTelemetry` integration project that packages five new Clavus part kinds, each declared as a `<ClavusPart>` item mirroring `Clavus.Serilog`:
    - `IOpenTelemetryPart` — subject `OpenTelemetry.IOpenTelemetryBuilder`
    - `ITracerProviderPart` — subject `OpenTelemetry.Trace.TracerProviderBuilder`
    - `IMeterProviderPart` — subject `OpenTelemetry.Metrics.MeterProviderBuilder`
    - `ILoggerProviderPart` — subject `OpenTelemetry.Logs.LoggerProviderBuilder`
    - `IResourceBuilderPart` — subject `OpenTelemetry.Resources.ResourceBuilder`
- Add a `UseOpenTelemetry(this ClavusContextBuilder)` entry point that registers a runtime integration which calls `AddOpenTelemetry()` once and drives all five extension points.
- Runtime wiring uses **async apply-per-kind (Option B)**: because OpenTelemetry's `WithTracing`/`WithMetrics`/`WithLogging`/`ConfigureResource` callbacks run synchronously and immediately, async telemetry parts are pre-run eagerly (awaited during host build) and their builder configuration is replayed inside the synchronous callbacks. This keeps API parity with `ApplyService`/`ApplyLogging` (each part kind gets a generated async `Apply{Kind}` extension) without blocking on async inside a sync callback.
- Add a test matrix (rsg-sdk `SdkTestProject` harness + in-repo runtime tests) exercising: `Clavus.Sdk` packaging, library-based configuration (package `build` assets) vs project-reference-based configuration, and composition of the telemetry parts with DryIoc, Autofac, and Serilog.

## Capabilities

### New Capabilities

- `opentelemetry-parts`: The five OpenTelemetry Clavus part kinds, their generated interfaces/delegates/`Apply`/`Configure` surface, and the ordering/priority guarantees for how registered parts configure the OpenTelemetry builders.
- `opentelemetry-runtime-integration`: The `UseOpenTelemetry` entry point and the async-apply-per-kind runtime that calls `AddOpenTelemetry()`, pre-runs async parts eagerly, and replays their configuration into the synchronous OpenTelemetry builder callbacks, composing with the container (DryIoc/Autofac) and Serilog integrations.

### Modified Capabilities

<!-- None — no existing spec-level behavior changes. -->

## Impact

- **New project**: `src/Clavus.OpenTelemetry/` (csproj with five `<ClavusPart>` declarations, `build`/`buildTransitive`/`buildMultiTargeting` asset targets like the other integration packages, `RocketOpenTelemetryExtensions`).
- **New dependencies**: `OpenTelemetry`, `OpenTelemetry.Extensions.Hosting` (source of `IOpenTelemetryBuilder`, `WithTracing`/`WithMetrics`/`WithLogging`/`ConfigureResource`).
- **New tests**: rsg-sdk `SdkTestProject`-based packaging/config-mode tests, plus in-repo runtime tests (using the OpenTelemetry in-memory exporter) asserting part invocation and ordering.
- **No changes** to existing `Clavus`, `Clavus.Hosting`, or the part-generation template/targets — the mechanism already supports new parts via `<ClavusPart>` items. `Clavus.Hosting` may gain an optional call site if `UseOpenTelemetry` is not sufficient on its own (to be resolved in design).
- **Solution files**: `Clavus.slnx` gains the new project and test project references.
