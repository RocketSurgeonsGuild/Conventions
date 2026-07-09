## ADDED Requirements

### Requirement: UseOpenTelemetry entry point

`Clavus.OpenTelemetry` SHALL expose a `UseOpenTelemetry(this ClavusContextBuilder)` extension that opts the host into OpenTelemetry. When called, the host build SHALL invoke `services.AddOpenTelemetry()` exactly once and drive all five telemetry builders (`IOpenTelemetryBuilder`, `ResourceBuilder`, `TracerProviderBuilder`, `MeterProviderBuilder`, `LoggerProviderBuilder`) from the registered parts.

#### Scenario: OpenTelemetry is not configured without opt-in

- **WHEN** a host is built without calling `UseOpenTelemetry`
- **THEN** `AddOpenTelemetry` is not called and no telemetry parts are applied

#### Scenario: Opt-in wires all builders

- **WHEN** a host is built after `UseOpenTelemetry` and parts of each of the five kinds are registered
- **THEN** each part's `Register` is invoked against its corresponding builder before the OpenTelemetry providers are built

### Requirement: Async parts applied without blocking

Because OpenTelemetry's `ConfigureResource`/`WithTracing`/`WithMetrics`/`WithLogging` callbacks execute synchronously, async telemetry parts (`I{Kind}AsyncPart`) SHALL be applied during the asynchronous host-build phase and their configuration replayed into the synchronous builder callbacks. The implementation MUST NOT block on async work with `.GetAwaiter().GetResult()` (or equivalent) inside a builder callback.

#### Scenario: Async tracer part effect is present

- **WHEN** an `ITracerProviderAsyncPart` that performs an awaited operation before adding an activity source is registered and the host is built
- **THEN** the resulting `TracerProviderBuilder` reflects that source and the host build completes without a deadlock or sync-over-async block

#### Scenario: Sync and async parts of the same kind compose in order

- **WHEN** both a sync `IMeterProviderPart` and an async `IMeterProviderAsyncPart` are registered
- **THEN** both configure the same `MeterProviderBuilder` in the executor's part order

### Requirement: Composition with container and logging integrations

The OpenTelemetry integration SHALL compose with the container service-provider factories (`Clavus.DryIoc`, `Clavus.Autofac`) and with `Clavus.Serilog`, such that telemetry parts run in the standard host-build sequence regardless of which container or logging integration is active.

#### Scenario: Telemetry parts run under DryIoc

- **WHEN** a host uses `UseDryIoc` and `UseOpenTelemetry` with telemetry parts registered
- **THEN** the parts are applied and telemetry is emitted through the DryIoc-built service provider

#### Scenario: Telemetry parts run under Autofac

- **WHEN** a host uses `UseAutofac` and `UseOpenTelemetry` with telemetry parts registered
- **THEN** the parts are applied and telemetry is emitted through the Autofac-built service provider

#### Scenario: Telemetry parts run alongside Serilog

- **WHEN** a host uses the Serilog integration and `UseOpenTelemetry` together
- **THEN** both the Serilog part and the telemetry parts are applied during host build without conflict

### Requirement: Verifiable telemetry output for tests

The integration SHALL be verifiable end to end using the OpenTelemetry in-memory exporter, so tests can assert that spans, metrics, log records, and resource attributes contributed by registered parts are present after building and running a host.

#### Scenario: In-memory exporter captures part-contributed telemetry

- **WHEN** parts add an activity source, a meter, a resource attribute, and a log record source, and the host runs with an in-memory exporter registered
- **THEN** the exported spans, metrics, resource attributes, and log records include the part-contributed values
