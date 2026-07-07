## Context

Clavus's existing model is: `Clavus.Analyzers` scans assembly-level `[Convention]` attributes at compile time via an `IIncrementalGenerator`, topologically orders them (`Before`/`After`/`DependsOn`/`DependentOf`), and emits an `Imports`/export class with zero runtime reflection. `Clavus.Sdk` is a custom MSBuild SDK that auto-selects packages and emits bootstrapping based on the host SDK. There is currently no equivalent pipeline for _configuration_: a library that wants to ship default config today has to hand-author an options class, hand-wire `IConfiguration` binding, and separately ensure any config file actually ships with the package and reaches the host app. This design extends the same "compile-time discovery + generated wiring" philosophy to configuration.

Constraints carried over from the existing system:

- Zero runtime reflection in the generated wiring path (AOT/trimming-safe).
- `Clavus.Analyzers` targets `netstandard2.0`/`net8.0` (the generator itself must run inside the Roslyn analyzer host, not the target framework of the consuming project).
- `RS0017` public API tracking is enforced — every new generated/public type needs an API-tracking entry.
- The generator is incremental; any new pipeline stage must not defeat incrementality (no `Compilation`-wide re-scans on unrelated edits).

## Goals / Non-Goals

**Goals:**

- Config files declared in a library are packed on `Pack` and copied to a same-solution host app on build, with the host able to enumerate contributing assemblies at build time.
- Strongly-typed configuration classes are generated inside the owning library from JSON/YAML/TOML config files, with sensible type inference (`DateOnly`/`TimeOnly`/`DateTimeOffset`/`TimeSpan`) and an opt-in NodaTime type mode.
- Generated configuration is exposed through a new `IConfigurationPart` that the exports generator always includes in a library's export set.
- A library's own transitive configuration dependencies flow to its dependents without redeclaration.
- Runtime consumption uses standard `IOptions<T>`/`IOptionsMonitor<T>` and supports reload.

**Non-Goals:**

- Replacing `Microsoft.Extensions.Configuration` itself — this builds generated types and registration _on top of_ the existing configuration abstractions, not a new config runtime.
- Runtime (non-compile-time) discovery of arbitrary, unpacked config files dropped next to an assembly — discovery is generator/MSBuild-driven, not reflection-based file probing.
- Full JSON Schema-style validation authoring — type inference covers primitive shape detection only; anything more exotic falls back to `string`/`object` and is left to the consumer.
- Migrating existing hand-written options classes in current Clavus integration packages — this design introduces the capability; adopting it in `Clavus.Hosting*`/`Clavus.Aspire`/etc. is follow-up work, not part of this change.

## Decisions

### 1. Config file declaration and packaging use MSBuild items, not attribute scanning

Config files are declared via a new MSBuild item, e.g. `<ClavusConfiguration Include="appsettings.json" />` (default-globbed for the standard `appsettings` naming convention — `appsettings.{ext}`, `appsettings.{Environment}.{ext}`, `appsettings.local.{ext}` for `.json`/`.yaml`/`.yml`/`.toml` — so most libraries need zero explicit declaration). `Clavus.Sdk` targets:

- On `Pack`: route each non-local `ClavusConfiguration` item through `Content`/`None` with `Pack=true` and a stable `PackagePath` (e.g. `contentFiles/any/any/clavus/`), so the file round-trips through NuGet like any packed content file. `appsettings.local.{ext}` is never packed — see Decision 8.
- On build, when a `ClavusConfiguration`-bearing project is referenced via `ProjectReference` in the same solution: a target copies the file into the referencing (host) project's output directory, mirroring the same relative layout the NuGet package would produce, so debug-time behavior matches packaged behavior.
- At runtime, discovered files for the same base name layer in ascending precedence — base → environment-specific → local — mirroring the well-known ASP.NET Core `appsettings.json`/`appsettings.{Environment}.json` layering, with `appsettings.local.{ext}` added as a highest-precedence, never-committed override layer for developer machines.

**Alternative considered:** Emit config as embedded resources and extract at runtime. Rejected — breaks the "config file is inspectable/editable on disk next to the host app" expectation and complicates hot-reload (embedded resources aren't file-watchable).

**Alternative considered:** Invent a Clavus-specific config file name (e.g. `clavus.json`) instead of mirroring `appsettings.json`. Rejected — `appsettings.{base|Environment|local}.{ext}` is already the convention every .NET developer recognizes from ASP.NET Core; reusing it means zero new mental model for layering/precedence, and the only genuinely new concept is the `local` pseudo-environment for gitignored developer overrides.

### 2. Host awareness via a generated manifest, not runtime probing

The generator emits, per assembly that declares `ClavusConfiguration` items, an assembly-level marker (`[assembly: Clavus.ConfigurationAssembly("<name>", "<relativePath>", ...)]`) rather than a side-channel file. The host-side generator pass reads these markers off `Compilation.ReferencedAssemblyNames`/`GetAttributes()` on referenced assemblies (the same mechanism already used to discover `[Convention]` attributes transitively) and emits a host-visible manifest type (e.g. `internal static class ClavusConfigurationManifest`) listing contributing assemblies and their relative config paths. This keeps discovery entirely compile-time and reuses the existing attribute-scanning infrastructure instead of introducing a second discovery mechanism (e.g. hand-rolled JSON manifest files read by MSBuild `Exec` tasks).

**Alternative considered:** A generated JSON manifest file written to `obj/` and read by both MSBuild targets and the generator. Rejected as a first pass — two sources of truth (assembly attribute vs. file) invite drift; the attribute is the single source of truth, and an MSBuild target can regenerate a human-readable manifest from it if needed later.

### 3. Type inference precedence and ambiguity handling

Given a raw config value's string representation, the generator applies this precedence (first match wins), each check anchored to a strict format regex before attempting a culture-invariant parse (to avoid `"14:30"` ambiguity between `TimeOnly` and `TimeSpan`, and `"2024-01-01"` vs. locale-dependent date parsing):

1. `TimeSpan` — strict `d.hh:mm:ss[.fffffff]` / `hh:mm:ss` duration-with-colons-and-no-date-component shape.
2. `DateOnly` — strict `yyyy-MM-dd` (ISO 8601 date-only).
3. `TimeOnly` — strict `HH:mm[:ss[.fff]]` with no date component and not already matched as `TimeSpan`.
4. `DateTimeOffset` — ISO 8601 date+time (`yyyy-MM-ddTHH:mm:ss(.fff)?(Z|±HH:mm)?`).
5. Fallback: `bool`, `int`/`long`, `double`, else `string`.

When the NodaTime MSBuild property is enabled _and_ `NodaTime` is present in `Compilation.ReferencedAssemblyNames`, steps 1–4 map to `Duration`, `LocalDate`, `LocalTime`, and `OffsetDateTime` respectively instead of the BCL types. If the property is enabled but `NodaTime` is not referenced, the generator reports a diagnostic (`CLAVUS_CFG002`) rather than silently falling back — silent fallback would make a project's generated public API shape depend on an easily-missed reference.

**Alternative considered:** Always emit `string` and let consumers parse. Rejected — defeats the "strongly typed" goal that's the point of this feature; type inference is opt-out (an explicit `string`-typed override escape hatch is left as an open question, see below) rather than opt-in.

### 4. Configuration class generated in the owning library, `IConfigurationPart` for wiring

For each config file, the generator emits a `sealed partial class` inside the _owning_ library's root namespace (not the consumer's), named from the file (e.g. `appsettings.json` → `AppSettingsConfiguration`), plus a generated `IConfigurationPart` implementation that:

- Adds the appropriate `IConfigurationSource` (JSON/YAML/TOML) pointing at the packaged/copied file.
- Calls `services.AddOptions<T>().Bind(configuration.GetSection(...))` and registers `IOptionsChangeTokenSource<T>` wiring so reload works through the standard `IOptionsMonitor<T>`/`IOptionsSnapshot<T>` path.

Because `IConfigurationPart` instances are generator-authored (not user-decorated with `[Convention]`), the exports generator's existing attribute-driven export scan is extended with an unconditional "always include generated `IConfigurationPart` types for this compilation" step — otherwise these parts would silently never be picked up, defeating the entire feature.

**Alternative considered:** Require library authors to manually add `[Convention]` to the generated partial class via a second, hand-written partial. Rejected — reintroduces the "forgot a step" failure mode this feature exists to eliminate.

### 5. Transitive configuration via the same reference-graph walk as convention export

A library's dependent libraries already need to see its exported `IClavusPart`s through the existing export/import graph. `IConfigurationPart`s ride the same graph: the generator's dependency walk (already required for cross-project convention export) is reused to flatten a project's own referenced libraries' `IConfigurationPart` exports into its own export set, so a consumer three levels down the dependency chain gets configuration parts without redeclaring anything. No new graph-walk mechanism is introduced.

### 6. Multi-format support via pluggable `IConfigurationSource` providers, generator-side parsers are format-agnostic at the "shape" level

JSON is already covered by `Microsoft.Extensions.Configuration.Json`. YAML and TOML need both (a) a compile-time parser the generator uses purely to discover key/value shape (independent of the runtime provider), and (b) a runtime `IConfigurationSource`/`IConfigurationProvider` that supports `reloadOnChange`. The generator-side parse and the runtime provider are decoupled — the generator only needs a flat key→raw-string-value view (same shape it already needs from JSON) to run type inference; it does not need to share code with the runtime provider.

**Alternative considered:** Convert YAML/TOML to JSON at build time and only ever ship/read JSON at runtime. Rejected — loses the point of "author in YAML/TOML," and complicates hot-reload (the source file the user edits would no longer be the file being watched).

### 7. Rollout gated behind an MSBuild opt-in switch

The whole pipeline is gated by `EnableClavusConfiguration` (default `false` initially), so existing Clavus consumers see no behavior change until they opt in. This mirrors how the NodaTime type mode is itself gated by its own property — the feature is additive and off by default at both the "is this pipeline active at all" and "which type system does it use" levels.

### 8. `local` is a pseudo-environment layer, excluded from packing and version control

`appsettings.local.{ext}` is treated as the highest-precedence layer above the base and environment-specific files, matching a pattern already common in ASP.NET Core projects (`appsettings.Development.json` checked in, an uncommitted local file layered on top). Unlike `appsettings.{Environment}.{ext}`, `appsettings.local.{ext}` is never included in the packed NuGet package and `Clavus.Sdk` scaffolds a `.gitignore` entry for the pattern by default — it exists purely for a developer's own machine-local overrides (secrets, local ports, personal toggles) and is discovered/layered at build time exactly like any other configuration file, it simply never leaves the developer's machine.

**Alternative considered:** Treat `local` as just another value of `{Environment}` (e.g. `ASPNETCORE_ENVIRONMENT=local`), relying on existing environment-layering with no special-casing. Rejected — conflating "which environment am I deployed to" with "did the developer override something locally" would mean a real `local` _environment_ (e.g. a genuinely named deployment tier) collides with the override mechanism; keeping `local` a distinct, always-highest-precedence layer regardless of the active environment avoids that collision and matches the user's explicit requirement that it layer on top of _any_ environment.

## Risks / Trade-offs

- **[Risk] Type-inference ambiguity between `TimeSpan` and `TimeOnly` for colon-delimited strings** → Mitigation: strict anchored regexes per format (see Decision 3) applied in a fixed precedence order, documented and snapshot-tested with adversarial inputs (`"24:00"`, `"1.00:00:00"`, `"00:00:00.5"`).
- **[Risk] NodaTime opt-in mismatch between a library built with NodaTime types and a consumer without the NodaTime reference** → Mitigation: compile-time diagnostic (`CLAVUS_CFG002`) when the property is set without the reference; document that NodaTime mode is a library-level (not solution-wide) decision and generated types always self-declare which mode produced them (so a mismatch surfaces as a compile error in the consumer, not a runtime failure).
- **[Risk] Generated classes across many libraries collide on namespace/type names** → Mitigation: types are generated into the owning library's own root namespace (Decision 4), never a shared/global namespace, so collisions are no worse than any other type-naming collision within that library.
- **[Risk] Reload support conflicting with generator-produced immutable POCOs** → Mitigation: consumers are only ever handed `IOptions<T>`/`IOptionsMonitor<T>`/`IOptionsSnapshot<T>`; the generated `T` itself has settable properties suitable for re-binding on change, consistent with standard `Microsoft.Extensions.Options` binding conventions.
- **[Risk] Adding a new incremental generator stage regresses build/generation performance** → Mitigation: config discovery reuses the existing `AdditionalFiles`/`AnalyzerConfigOptionsProvider` machinery already read for `ClavusMetadata` etc., and is keyed so unrelated source edits don't invalidate the configuration pipeline stage (see `Clavus.Analyzers.Tests` incrementality snapshot conventions).
- **[Risk] YAML/TOML runtime provider dependencies increase package footprint for consumers who only use JSON** → Mitigation: format support is split into separate optional packages/`ItemGroup`-conditional references (only pulled in when a YAML/TOML config file is actually declared), consistent with existing Clavus pattern of narrowly-scoped integration packages.

## Migration Plan

This is a net-new, opt-in capability — there's no existing behavior to migrate away from. Rollout sequence:

1. Ship `clavus-config-packaging` (discovery, packing, copy, manifest) behind `EnableClavusConfiguration` — no codegen yet, so it's inert without the generation stage but establishes the MSBuild surface.
2. Ship `clavus-config-generation` (typed classes, `IConfigurationPart`, export integration, transitive flow) — libraries can now generate and export configuration, still opt-in per project.
3. Ship `clavus-config-runtime` (multi-format providers, `IOptions` wiring, reload) — end-to-end feature complete.
4. Dogfood on one existing Clavus integration package (candidate: `Clavus.Serilog`, which already has an obvious config surface) before recommending broader adoption.
5. No rollback concerns beyond "don't set `EnableClavusConfiguration`" — the property is additive and off by default throughout the rollout.

## Open Questions

- Should there be an explicit per-property type override (e.g. a sibling `.clavus.types.json` or inline `$type` hint) for the cases where inference guesses wrong, or is "regenerate as `string` and cast manually" an acceptable escape hatch for v1?
- What's the exact generated-type naming convention when a library declares more than one config file (suffix by file name? require an explicit item metadata `ClassName`?)?
- Should the host-visible configuration manifest be a generated type only, or should `Clavus.Sdk` also emit a human-readable file (e.g. `bin/clavus-configuration.json`) for tooling/ops visibility outside the compiler?
- How should nested objects and arrays-of-objects in YAML/TOML be represented — nested generated classes, or flattened dotted-key sections? (JSON precedent from `Microsoft.Extensions.Configuration` binding suggests nested classes, but this needs to be pinned down before the generator's shape-inference walk is implemented.)
- Which YAML and TOML libraries become the generator-side parse dependency and the runtime provider dependency respectively — needs a compatibility check against `netstandard2.0` (generator host) before committing.
