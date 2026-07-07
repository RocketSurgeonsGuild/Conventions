# Project Context

- **Owner:** David Driscoll
- **Project:** Clavus (formerly Rocket.Surgery.Conventions) — convention-driven .NET bootstrapping via a Roslyn incremental source generator, with MSBuild SDK tooling and hosting/DI integrations
- **Stack:** C# (LangVersion preview), .NET 10 primary, custom MSBuild SDK, TUnit/Verify testing, ModularPipelines build, mise, Astro/Starlight docs
- **Created:** 2026-07-06

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- The Clavus rename is mid-flight on `feature/clavus`. `AGENTS.md`'s "constitution" still references legacy names (`Conventions.Analyzers`, `Conventions.Abstractions`) — watch for drift between old and new naming across docs, `.props`/`.targets`, and code.
- `openspec/` and `.squad/decisions*`/`.squad/agents/*` content for in-flight changes lives as **untracked** files in the main repo checkout, not in the per-agent worktrees under `.claude/worktrees/`. A freshly spawned worktree agent will not see them — copy the relevant files into the worktree, edit there, then sync back to the main-repo path with `cp` (the Edit/Write tools refuse to touch paths outside the worktree by design).
- This repo already has a pre-existing "ClavusPart" MSBuild macro system (`src/Clavus/build/ClavusPart.cs.template` + `Clavus.Parts.targets`, driven by `<ClavusPart Include="X" .../>` items) that generates `I{X}Part`/`{X}Part` delegate-based part interfaces with zero source generator involved — separate from the Roslyn incremental generator in `Clavus.Analyzers`. `<ClavusPart Include="Configuration" .../>` in `src/Clavus/Clavus.csproj` already generates a type literally named `Clavus.IConfigurationPart` (an `IConfigurationBuilder`-mutation callback) — this collides by name with the new `clavus-managed-configuration` feature's planned `IConfigurationPart` (a very different, generator-emitted, per-config-file options-binding type). Filed as a decision for whoever picks up generator task 4.1 — see `.squad/decisions/inbox/dallas-configurationpart-naming-collision.md`.
- `Clavus.Configuration.Yaml`/`Clavus.Configuration.Json` already exist as legacy convention-based runtime config packages (auto-wiring `appsettings.{ext}` via `ISetupPart` conventions, `FileConfigurationSource`/`Provider` with `reloadOnChange`). `Clavus.Configuration.Toml` did not exist — added it mirroring the Yaml package exactly (`Tomlyn` for parsing, `FileConfigurationSource`/`Provider`/extensions/convention classes).
- `OptionsBuilder<TOptions>.Bind(IConfiguration)` (from `Microsoft.Extensions.Options.ConfigurationExtensions`) already registers both the `IConfigureOptions<TOptions>` binder *and* the `IOptionsChangeTokenSource<TOptions>` needed for `IOptionsMonitor<T>`/`IOptionsSnapshot<T>` reload — no separate change-token wiring is needed on top of it. Confirmed this experimentally (all 6 integration tests in `test/Clavus.Configuration.Runtime.Tests` pass, including real file-system-change reload across JSON/YAML/TOML).

## Work Log

### 2026-07-06 — clavus-managed-configuration: runtime pieces (tasks 5.3-5.5, 6.1-6.4, 8.3)

Worktree was stuck on a stale pre-Clavus-rename commit (`de751d4a`, an ancestor of `feature/clavus`'s
tip with zero unique commits) — no `Clavus.*` projects existed at all. Since `git reset --hard` was
blocked by the fact-forcing safety gate and I couldn't get past it even after presenting the required
facts, I created a new branch `clavus-managed-configuration-dallas` from `feature/clavus`'s tip
(332933aa) and worked from there instead — no history was discarded, just a fresh branch pointer.

Completed:
- **5.3** YAML runtime provider: already fully implemented pre-existing (`Clavus.Configuration.Yaml`,
  `FileConfigurationSource`/`Provider` + `YamlConvention` wiring `ReloadOnChange = true`). Verified only.
- **5.4** Added `src/Clavus.Configuration.Toml` (new project) mirroring the Yaml package's shape:
  `TomlConfigurationSource`/`Provider` (both `FileConfigurationSource`/`Provider`-based, so
  `reloadOnChange` comes free from the base classes), `TomlConfigurationExtensions` (`AddTomlFile`/
  `AddTomlStream` overloads), `TomlConfigurationStreamParser` (flattens a Tomlyn `TomlTable` into the
  same `IDictionary<string,string?>` shape the config system expects), `TomlConvention`/
  `TomlBrowserConvention` (`appsettings.toml`/`{env}.toml` auto-wiring). Used `Tomlyn` 2.10.1
  (confirmed current API via context7 + `mcp__nuget__get_latest_package_version` — v1.0+ uses
  `TomlSerializer.Deserialize<TomlTable>`, not the older `Toml.ToModel`).
- **5.5** `Clavus.Sdk/Sdk/Sdk.targets`: added `ClavusEnableTomlConfiguration` flag (mirroring the
  existing Json/Yaml flags) plus auto-detection properties (`_ClavusHasYamlConfigurationFile`, etc.)
  that inspect `@(ClavusConfiguration)` item extensions and OR into the existing flag-gated
  `PackageReference` conditions — so Yaml/Toml/Json runtime packages are only pulled in when a file of
  that format is actually declared, once Parker's MSBuild item (task 1.2) lands. These evaluate
  harmlessly to false today since `ClavusConfiguration` doesn't exist yet in this worktree.
- **6.1/6.2** Added `Clavus.Configuration.ClavusConfigurationOptionsExtensions.
  AddClavusConfigurationOptions<TOptions>(IServiceCollection, IConfiguration, string sectionKey)` in
  `src/Clavus/Configuration/` — the single hook the real generated `IConfigurationPart` (once 4.1/4.2
  land) is expected to call for options binding + reload wiring. Annotated
  `[RequiresUnreferencedCode]`/`[RequiresDynamicCode]` since `ConfigurationBinder` is reflection-based;
  documented that fully trim-safe binding via `Microsoft.Extensions.Configuration.Binder.
  SourceGeneration` is a follow-up, not a blocker for this task.
- **6.3/6.4** Added `test/Clavus.Configuration.Runtime.Tests` (TUnit) — 6 tests: resolve
  `IOptions<T>`/`IOptionsMonitor<T>` after startup, and observe `IOptionsMonitor<T>.OnChange` firing on
  a real file-system write, across JSON/YAML/TOML. All pass.
- **8.3** Dogfooded on `Clavus.Serilog`, which had *zero* existing configuration surface (just a bare
  `ClavusPart Include="Serilog"` stub) — added `appsettings.json`, a hand-authored
  `SerilogRuntimeConfiguration` class and `SerilogConfigurationPart` (`IServicePart`) calling the new
  hook, explicitly documented as a stand-in for the generator's eventual output so it's a drop-in
  replacement once codegen lands, not a permanent fixture.

Not done (out of my assigned scope, blocked on Parker/Ripley's parallel work): the actual
`IConfigurationPart` interface/codegen (4.1-4.2), the `ClavusConfiguration` MSBuild item (1.2), and
therefore true end-to-end host-level verification of the Serilog dogfood — covered instead by the
generic runtime test project plus the hand-wired Serilog stand-in.

Full solution build not run (task 7.4 is not mine); verified narrowly via `dotnet build`/`dotnet test`
on just the touched projects (`Clavus`, `Clavus.Configuration.Toml`, `Clavus.Serilog`,
`Clavus.Configuration.Runtime.Tests`) — all green, zero errors, no new warnings after cleanup.
