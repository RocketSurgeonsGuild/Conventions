## Why

Clavus already auto-discovers and orders `[Convention]`-attributed classes at compile time, but configuration is still hand-wired: a library that ships `appsettings`-style config has no standard way to get that config packaged, copied to a host app, discovered, strongly typed, or bound. Every library currently reinvents this, config keys drift from what's actually consumed, and there's no compile-time guarantee that a library's configuration surface is registered in the host. As Clavus grows more hosting/DI integrations, a first-class, generator-driven configuration pipeline removes an entire class of "I forgot to add this options class" and "the config file didn't ship with the package" bugs.

## What Changes

- Config files (`.json`/`.yaml`/`.toml`) authored in a Clavus library are packed into the library's NuGet package on `dotnet pack`, and copied into the consuming host application's output/deployment when referenced via `ProjectReference` in the same solution.
- The host application gains a generated manifest of which referenced assemblies contribute configuration, so it can enumerate/inspect config sources at build time instead of relying on convention-only discovery.
- Each library's configuration file gets a strongly-typed configuration class generated **in that library**, avoiding consumer-side name clashes and letting the library use its own config internally.
- Property type inference is added to the config generator: values that parse as a date become `DateOnly`, time-only values become `TimeOnly`, combined date+time becomes `DateTimeOffset`, and duration-shaped strings become `TimeSpan`. **BREAKING (opt-in)**: when `NodaTime` is referenced and an MSBuild property enables it, the equivalent NodaTime types (`LocalDate`, `LocalTime`, `Instant`/`OffsetDateTime`, `Duration`) are emitted instead of BCL types.
- `Clavus.Analyzers` is extended to recognize referenced configuration files and emit an `IConfigurationPart` for each, and the existing exports generator is updated to always include generated `IConfigurationPart`s in a library's export set (since parts aren't picked up by default attribute scanning alone).
- Configuration contributed by a library's own dependencies is made available (flattened/transitive) to dependent libraries, so a library doesn't need to redeclare config it merely depends on.
- Runtime binding uses the standard `IOptions<T>` / `IOptionsMonitor<T>` pattern and supports configuration reloading (`IOptionsSnapshot`/change tokens) rather than a bespoke binding mechanism.

## Capabilities

### New Capabilities

- `clavus-config-packaging`: Discovering config files in a library, packing them into the NuGet package on `Pack`, copying them into a same-solution host application on build, and generating a host-visible manifest of which assemblies contribute configuration.
- `clavus-config-generation`: Source-generator support for emitting a strongly-typed configuration class per config file inside the owning library, including value-shape type inference (`DateOnly`/`TimeOnly`/`DateTimeOffset`/`TimeSpan`), an opt-in NodaTime type mode gated by an MSBuild property, emission of a generated `IConfigurationPart` per config file, export-generator integration so generated parts are always included in a library's export set, and transitive flow of a library's own dependency configuration into dependent libraries.
- `clavus-config-runtime`: Multi-format configuration source support (JSON, YAML, TOML), registration of generated configuration types via `IOptions<T>`/`IOptionsMonitor<T>`, and support for configuration reloading.

### Modified Capabilities

<!-- No existing openspec/specs/ capabilities yet in this repo; nothing to modify. -->

## Impact

- **Affected projects:** `Clavus.Analyzers` (new incremental generator stages for config discovery, type inference, `IConfigurationPart`/export emission), `Clavus` (new `IConfigurationPart` abstraction alongside `IClavusPart`), `Clavus.Sdk` (MSBuild targets for packing/copying config files, new MSBuild properties such as the NodaTime opt-in switch), new format-reader dependencies for YAML/TOML parsing (JSON already covered by `Microsoft.Extensions.Configuration.Json`).
- **New MSBuild surface:** properties/items to declare config files, opt into NodaTime types, and control packaging/copy behavior.
- **New public API surface:** generated per-library configuration classes, `IConfigurationPart`, host-visible configuration manifest — all subject to the existing `RS0017` public API tracking gate.
- **Testing:** `Clavus.Analyzers.Tests` gains new Verify snapshots for generated configuration classes and `IConfigurationPart` emission across JSON/YAML/TOML inputs and both BCL and NodaTime type modes.
- **Docs:** new concept guide needed for authoring library configuration and consuming it from a host application.
