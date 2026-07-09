## ADDED Requirements

### Requirement: OpenTelemetry builder part kinds

The `Clavus.OpenTelemetry` package SHALL declare five Clavus part kinds via `<ClavusPart>` items, each exposing one OpenTelemetry builder as its `Register` subject: `IOpenTelemetryPart` (`OpenTelemetry.IOpenTelemetryBuilder`), `ITracerProviderPart` (`OpenTelemetry.Trace.TracerProviderBuilder`), `IMeterProviderPart` (`OpenTelemetry.Metrics.MeterProviderBuilder`), `ILoggerProviderPart` (`OpenTelemetry.Logs.LoggerProviderBuilder`), and `IResourceBuilderPart` (`OpenTelemetry.Resources.ResourceBuilder`). Each part kind MUST also generate its async variant, delegate types, `Apply{Kind}` extension, and `Configure{Kind}` builder methods, consistent with existing parts such as `Serilog`.

#### Scenario: All five part interfaces are generated

- **WHEN** the `Clavus.OpenTelemetry` project is built
- **THEN** the compiled assembly exposes public interfaces `IOpenTelemetryPart`, `ITracerProviderPart`, `IMeterProviderPart`, `ILoggerProviderPart`, and `IResourceBuilderPart`, each with a `Register(IClavusContext, <builder>)` method

#### Scenario: Async part variants are generated

- **WHEN** the `Clavus.OpenTelemetry` project is built
- **THEN** the assembly also exposes `I{Kind}AsyncPart` interfaces whose `Register` returns `ValueTask` and accepts a `CancellationToken`, for each of the five builder kinds

#### Scenario: Builder configuration methods are generated

- **WHEN** a consumer has a `ClavusContextBuilder`
- **THEN** `ConfigureTracerProvider`, `ConfigureMeterProvider`, `ConfigureLoggerProvider`, `ConfigureResourceBuilder`, and `ConfigureOpenTelemetry` delegate-based methods are available to register inline parts

### Requirement: Telemetry part ordering

Registered telemetry parts SHALL be applied in the priority and dependency order the Clavus executor already guarantees for `context.Parts`. Within a single builder kind, a part with lower `Priority` MUST configure that builder before a part with higher `Priority`.

#### Scenario: Parts configure a builder in priority order

- **WHEN** two `ITracerProviderPart` implementations with different priorities are registered
- **THEN** their `Register` calls run against the same `TracerProviderBuilder` in ascending priority order

### Requirement: Attribute-discovered exported parts

A telemetry part type annotated with `[ClavusExport]` SHALL be discovered and registered automatically, the same way `[ClavusExport]`-annotated service/configuration parts are, without an explicit `Configure{Kind}` call.

#### Scenario: Exported tracer part is applied without manual wiring

- **WHEN** an assembly contains a `[ClavusExport]` class implementing `ITracerProviderPart` and that assembly is part of the convention scan
- **THEN** the part's `Register` is invoked against the `TracerProviderBuilder` during host build
