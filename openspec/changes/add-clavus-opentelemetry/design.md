## Context

Clavus generates convention "part" types entirely from MSBuild: each `<ClavusPart>` item in a `.csproj` is expanded by `src/Clavus/build/Clavus.Parts.targets` against `src/Clavus/build/ClavusPart.cs.template`, producing `I{Name}Part`, `I{Name}AsyncPart`, `{Name}Part`/`{Name}AsyncPart` delegates, a `{Name}PartExtensions.Apply{Name}(subject, context, ct)` runtime extension, and `ClavusContextBuilder.Configure{Name}(...)` builder overloads. Existing integration packages (`Clavus.Serilog`, `Clavus.DryIoc`, `Clavus.Autofac`, `Clavus.Aspire`) each declare their part(s) this way and ship `build`/`buildTransitive`/`buildMultiTargeting` asset targets plus a `Use{X}`/`RocketHostExtensions` runtime entry point.

Parts are applied at host-build time. `ClavusHostApplicationHelpers.Configure` (in `src/Clavus.Hosting/RocketHostApplicationExtensions.cs`) awaits `builder.Services.ApplyService(context, ct)` and `builder.Logging.ApplyLogging(context, ct)` in sequence. Under the hood `Apply{Name}` calls `ClavusContextExtensions.RegisterConventions`, which builds a `CalvusExecutor` and iterates the already-materialized, already-ordered `context.Parts`, running sync handlers then awaiting async handlers per part.

OpenTelemetry's configuration surface (`OpenTelemetry` + `OpenTelemetry.Extensions.Hosting`) is: `services.AddOpenTelemetry()` returns `IOpenTelemetryBuilder`, which exposes `ConfigureResource(Action<ResourceBuilder>)`, `WithTracing(Action<TracerProviderBuilder>)`, `WithMetrics(Action<MeterProviderBuilder>)`, and `WithLogging(Action<LoggerProviderBuilder>)`. **`WithTracing`/`WithMetrics`/`WithLogging` run their callback synchronously and immediately at call time** — they hand back a real, immediately usable builder facade (e.g. `TracerProviderBuilderBase`) and invoke `configure` against it inline, even though that facade's own methods still queue deferred `services.Configure…` registrations underneath. This is the crux constraint for those three kinds: there is no async seam inside the callback to `await` a telemetry part, so the integration must capture-then-await outside it (Decision 2a).

**Correction (post-3.1-3.4, found by Ash's runtime tests, confirmed by decompiling OpenTelemetry 1.16.0):** `ConfigureResource` is the one exception. `OpenTelemetryBuilderSdkExtensions.ConfigureResource` never invokes `configure` itself — it forwards the untouched delegate into `ConfigureOpenTelemetryTracerProvider`/`MeterProvider`/`LoggerProvider`, and each provider's builder (`TracerProviderBuilderSdk.ConfigureResource` etc.) only invokes it lazily, when that provider is actually built (first DI resolution), not at the `ConfigureResource` call site. A local captured via `ConfigureResource(rb => captured = rb)` is therefore still `null` immediately after the call returns. See Decision 2a's "Follow-up" note below for the fix.

## Goals / Non-Goals

**Goals:**

- Expose the five OpenTelemetry builders (`IOpenTelemetryBuilder`, `TracerProviderBuilder`, `MeterProviderBuilder`, `LoggerProviderBuilder`, `ResourceBuilder`) as distinct Clavus part kinds using the existing `<ClavusPart>` mechanism — no new codegen infrastructure.
- Support **async** telemetry parts with full API parity to `Service`/`Logging` (Option B), without blocking (`.GetAwaiter().GetResult()`) inside OpenTelemetry's synchronous callbacks.
- Compose correctly with the container integrations (DryIoc/Autofac) and Serilog, in the standard host-build sequence.
- Prove the above with a test matrix across SDK packaging, both configuration-discovery modes, and all three third-party integrations.

**Non-Goals:**

- Shipping specific instrumentation/exporter packages (AspNetCore, HttpClient, OTLP, Console). Consumers add those via their own parts; the integration only opens the extension points.
- Changing the part-generation template or `Clavus.Parts.targets`.
- Auto-enabling OpenTelemetry without an explicit `UseOpenTelemetry()` opt-in.
- Bridging Serilog's own sink pipeline into OpenTelemetry logging (that is a separate concern; here they compose only in that both parts run).

## Decisions

### Decision 1: Five `<ClavusPart>` declarations, one per builder

Each builder becomes a non-generic part whose trailing parameter is the builder type, exactly like `Clavus.Serilog`'s `<ClavusPart Include="Serilog" ParameterType="global::Serilog.LoggerConfiguration" ParameterName="loggerConfiguration" />`:

```xml
<ClavusPart Include="OpenTelemetry"   Namespace="Clavus" ParameterType="global::OpenTelemetry.IOpenTelemetryBuilder"       ParameterName="builder" />
<ClavusPart Include="TracerProvider"  Namespace="Clavus" ParameterType="global::OpenTelemetry.Trace.TracerProviderBuilder"  ParameterName="builder" />
<ClavusPart Include="MeterProvider"   Namespace="Clavus" ParameterType="global::OpenTelemetry.Metrics.MeterProviderBuilder"  ParameterName="builder" />
<ClavusPart Include="LoggerProvider"  Namespace="Clavus" ParameterType="global::OpenTelemetry.Logs.LoggerProviderBuilder"    ParameterName="builder" />
<ClavusPart Include="ResourceBuilder" Namespace="Clavus" ParameterType="global::OpenTelemetry.Resources.ResourceBuilder"     ParameterName="builder" />
```

This yields `IOpenTelemetryPart`/`ITracerProviderPart`/`IMeterProviderPart`/`ILoggerProviderPart`/`IResourceBuilderPart` (+ async variants + `Configure…` builder methods + `Apply…` extensions) for free.

**Alternative considered:** a single `IOpenTelemetryPart` taking `IOpenTelemetryBuilder` and letting consumers reach the sub-builders themselves. Rejected — it loses the point of distinct extension points (a library that only wants to add a metric source shouldn't have to know about `WithMetrics` plumbing) and gives no per-builder ordering.

### Decision 2: Async apply-per-kind via eager pre-run + sync replay (Option B)

The runtime integration owns the single `AddOpenTelemetry()` call. Because the builder callbacks are synchronous, each part kind's async `Apply` is split into two phases:

```
Host build (async context)                      OTel callback (sync, deferred-registered)
──────────────────────────                      ─────────────────────────────────────────
1. otel = services.AddOpenTelemetry()
2. await PrepareTracerParts(context, ct) ──┐
   (run every ITracerProvider[Async]Part,  │   3. otel.WithTracing(tracer =>
    each producing/queuing its config       │        replay captured config onto `tracer`)
    against a captured builder handle)  ────┘
   … same for Meter / Logger / Resource / OpenTelemetry
```

Two concrete implementation options for "capture + replay", to be finalized in tasks:

- **(2a) Capture-the-builder:** call `WithTracing(b => capturedTracerBuilder = b)` first to grab the real `TracerProviderBuilder`, then `await capturedTracerBuilder.ApplyTracerProvider(context, ct)` in the async host context. Since OTel's callback runs inline, the capture completes before the await. This reuses the generated async `Apply…` directly and is the least code.
- **(2b) Deferred delegate list:** pre-run async parts to populate an ordered `List<Action<TracerProviderBuilder>>`, then `WithTracing(b => list.ForEach(a => a(b)))`. Requires parts to yield a sync action, which the generated `Register(context, builder, ct)` signature does not do cleanly — so 2a is preferred.

**Why not Option A (pure sync iteration):** it cannot honor `I…AsyncPart` at all. The user chose full async parity, so we keep the async `Apply…` and bridge with 2a.

**Invariant:** telemetry parts SHALL be applied in the same priority/dependency order the executor already guarantees for `context.Parts`, and each builder's parts run before the OpenTelemetry provider is built.

**Follow-up (post-3.1-3.4, Resource kind only):** 2a's "capture-the-builder" premise — that the OTel callback runs synchronously and hands back a real builder instance — does **not** hold for `ConfigureResource` (see the Context correction above). `capturedResourceBuilder` stayed `null`, and `await capturedResourceBuilder.ApplyResourceBuilder(...)` threw `ArgumentNullException`, caught by Ash's `ResourceBuilderPart_Adds_Attribute_Present_On_Built_Providers` test. Fix implemented in `RocketOpenTelemetryExtensions.UseOpenTelemetry`: build a private `ResourceBuilder.CreateEmpty()` outside OTel's plumbing, run every `IResourceBuilderPart`/Async part against it with a real `await` (no timing dependency), `Build()` it into a `Resource`, then register `otelBuilder.ConfigureResource(rb => rb.AddAttributes(resource.Attributes))` — a callback that only closes over an already-fully-computed value, so it is correct regardless of when or how many times OpenTelemetry invokes it. `CreateEmpty()` (not `CreateDefault()`) is deliberate: OTel's own `TracerProviderBuilderSdk.ConfigureResource` already seeds `state.ResourceBuilder ??= ResourceBuilder.CreateDefault()` before invoking our merge callback, so starting from empty avoids double-adding the default detector's attributes. The other four kinds (`OpenTelemetry`, `TracerProvider`, `MeterProvider`, `LoggerProvider`) are unaffected — 2a holds for all of them, confirmed passing in the runtime test suite.

### Decision 3: `UseOpenTelemetry(this ClavusContextBuilder)` registers a `Service` part

Following `Clavus.DryIoc`'s `UseDryIoc`, the entry point appends an internal registration (an `IServicePart`, or a dedicated setup hook) that runs during `ApplyService`, calls `AddOpenTelemetry()`, and performs the Decision-2 bridging. This keeps the integration self-contained in the package and composes automatically with whatever container factory (DryIoc/Autofac) is selected, because container selection happens at `ConfigureContainer` after services are populated.

**Alternative considered:** adding a call site directly in `Clavus.Hosting`. Rejected as default — it would couple core hosting to OpenTelemetry. Left as an open question only if a `Service`-part registration proves insufficient for ordering.

**Follow-up (post-3.1-3.4, re-examined):** a request came in to move the `OpenTelemetry.Extensions.Hosting` `PackageReference` out of `Clavus.OpenTelemetry.csproj` and into `Clavus.Hosting.csproj`. Re-confirms the rejection above, with concrete evidence: `Clavus.Hosting` is a transitive dependency of `Clavus.CommandLine`, `Clavus.Hosting.Web`, `Clavus.Hosting.Maui`, and `Clavus.Hosting.WebAssembly` — none of which use OpenTelemetry — so an unconditional `PackageReference` there would force OTel assemblies onto all of them, contradicting this package's opt-in design. It also wouldn't simplify anything: `UseOpenTelemetry()` never touches `Clavus.Hosting` (it drives `ConfigureServices` on `ClavusContextBuilder` from `Clavus` core), and `IOpenTelemetryBuilder` (the `<ClavusPart>` `ParameterType`) already resolves from the base `OpenTelemetry` package alone, not `.Extensions.Hosting`. The only consumer of `OpenTelemetry.Extensions.Hosting` is the single `services.AddOpenTelemetry()` call inside `Clavus.OpenTelemetry` itself, which is exactly where the package reference belongs. No change made; see tasks.md 3.6.

### Decision 4: Test in two layers

- **SDK/packaging layer (rsg-sdk `SdkTestProject`):** scaffold throwaway consumers against the packed SDKs to prove `Clavus.OpenTelemetry` resolves, its `build` assets inject the config `AdditionalFiles`/analyzers, and a consumer referencing it + DryIoc/Autofac/Serilog compiles and packs. This is where **library-based vs project-reference-based configuration** is exercised: the same telemetry/appsettings config discovered (a) from a referenced package's `build` assets vs (b) from a `<ProjectReference>`.
- **Runtime behavioral layer (in Conventions repo):** build a real host, register fake `ITracerProviderPart`/`IMeterProviderPart`/`ILoggerProviderPart`/`IResourceBuilderPart`/`IOpenTelemetryPart` (sync and async), start it, and assert via the OpenTelemetry **in-memory exporter** that spans/metrics/log records/resource attributes reflect every part, in priority order — under each of DryIoc, Autofac, and Serilog.

## Risks / Trade-offs

- **OTel callback executes inline assumption (Decision 2a)** → If a future OTel version defers the `Action<T>` to provider-build time, capture-the-builder still works (the captured builder is the same instance), but the _await_ must complete before `BuildServiceProvider`. Mitigation: apply all telemetry parts during host build, before the provider is resolved; add a runtime test that asserts an async part's effect is present, which fails loudly if the timing model changes.
- **Async part inside sync callback** → Chosen 2a avoids `GetAwaiter().GetResult()` entirely by awaiting outside the callback. Risk is contained to "capture happens synchronously," which the inline-execution semantics guarantee.
- **Package/version coupling to OpenTelemetry SDK** → `IOpenTelemetryBuilder` moved/renamed across early OTel versions. Mitigation: pin `OpenTelemetry`/`OpenTelemetry.Extensions.Hosting` via CPM and verify the exact type/namespace against the referenced version before finalizing the `<ClavusPart>` `ParameterType` values.
- **Trimming/AOT** → `Clavus` is `IsTrimmable`; OTel builder configuration is largely reflection-free but exporters may not be. Mitigation: keep the integration package free of exporter references; leave trim annotations to consumers' parts.
- **Test flakiness from real host startup** → Runtime tests spin a host per case. Mitigation: use the in-memory exporter and `HostType.UnitTest`, force synchronous flush, and avoid network exporters.

## Migration Plan

Additive only — no existing behavior changes. Deploy by adding the new project + package, wiring it into `Clavus.slnx` and the pack/release flow. Rollback is removing the project; no consumer is affected until they reference `Clavus.OpenTelemetry` and call `UseOpenTelemetry()`.

## Open Questions

1. **2a vs 2b** for the capture/replay mechanism — confirm 2a (capture-the-builder + reuse generated `Apply…`) holds against the pinned OTel version's callback timing.
2. **`IOpenTelemetryPart` role** — is the top-level `IOpenTelemetryBuilder` part applied before or after the four sub-builder parts? Proposed: `OpenTelemetry` (top-level) first, then Resource → Tracer → Meter → Logger.
3. **Exact OTel package version / `ParameterType` namespaces** — verify `OpenTelemetry.IOpenTelemetryBuilder`, `OpenTelemetry.Logs.LoggerProviderBuilder`, and `WithLogging` exist in the pinned version (some were newer additions).
4. **Where runtime tests live** — new `Clavus.OpenTelemetry.Tests` in Conventions vs extending the rsg-sdk suite. Proposed: both layers, as in Decision 4.
