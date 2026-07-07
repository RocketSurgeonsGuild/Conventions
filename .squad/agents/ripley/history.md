# Project Context

- **Owner:** David Driscoll
- **Project:** Clavus (formerly Rocket.Surgery.Conventions) — convention-driven .NET bootstrapping via a Roslyn incremental source generator, with MSBuild SDK tooling and hosting/DI integrations
- **Stack:** C# (LangVersion preview), .NET 10 primary (net8.0/netstandard2.0 for the generator itself), Roslyn `IIncrementalGenerator`, TUnit/Verify snapshot testing
- **Created:** 2026-07-06

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- `Clavus.Analyzers` reads MSBuild properties (`ClavusMetadata`, `ClavusAssignExternal`, `ClavusHostType`, `IsTestProject`) through an `AnalyzerConfigOptionsProvider` — any new generator behavior gated by project config should go through this same path.
- Generator output regressions are caught via Verify snapshots in `test/Clavus.Analyzers.Tests/snapshots/`, driven by `GeneratorTest.cs`.
- `Clavus.csproj` has a separate, unrelated `<ClavusPart Include="..." />` MSBuild-item templating mechanism (`src/Clavus/build/Clavus.Parts.targets`) that generates one `I{Name}Part`/`I{Name}AsyncPart` interface pair per declared part name at that project's own build time (e.g. `Configuration`, `Service`, `Setup`, `Logging`, `HostCreated` already exist). Any new "part" concept must check this first to avoid name collisions — see the `IConfigurationPart` decision below.
- A project's own generated export method (`Exported_Conventions.g.cs`) only yields *directly* declared parts; the transitive multi-hop flattening comes for free from `compilation.References` already containing the full transitive assembly closure (MSBuild flattens `ProjectReference` chains before the compiler sees them) combined with `MsBuildExtensions.GetClavusReferences()`'s reference walk — no recursive graph traversal is implemented or needed in the generator itself.

## Session: clavus-managed-configuration (2026-07-06)

Implemented my assigned tasks from `openspec/changes/clavus-managed-configuration/tasks.md`: 2.5-2.7, 3.1-3.3, 3.5-3.6, 4.1-4.3, 4.5, 5.1-5.2. Branch: `feature/clavus` (this worktree started on `main` at `de751d4a` and had to be reset onto `feature/clavus` to see the openspec docs and squad charters at all — see report to Coordinator for details).

Key new files: `src/Clavus/ConfigurationAssemblyAttribute.cs`; `src/Clavus.Analyzers/Support/Configuration/*.cs` (`ConfigurationDiscovery`, `JsonFlatConfigurationReader`, `ConfigurationValueTypeInference`, `ConfigurationNode`, `ConfigurationIdentifiers`, `ConfigurationClassEmitter`, `ConfigurationAssemblyMarkerEmitter`, `ConfigurationManifestEmitter`). Wired into `ClavusAttributesGenerator.Initialize` in `ConventionAttributesGenerator.cs`. Extended `MsBuildConfig`, `Diagnostics` (`CLAVUS_CFG002`), `ExportConventions` (unconditional part export), and `MsBuildExtensions` (reference-walk for `ConfigurationAssemblyAttribute`).

Deleted my own hand-authored `IConfigurationPart` after discovering it already exists via the `ClavusPart` templating mechanism (see decision filed to `.squad/decisions/inbox/ripley-config-part-reuse.md`) - generated config parts now implement both the pre-existing `IConfigurationPart` and `IServicePart`.

Full solution build (`dotnet build Clavus.slnx`) succeeds for every project I touch or that depends on `Clavus`/`Clavus.Analyzers`; all 102 existing `Clavus.Analyzers.Tests` still pass. The only solution-build failures are pre-existing and unrelated to this work: `sample/AspireSample` has a stale relative `ProjectReference` path (MSB3202), and `Clavus.CommandLine` has pre-existing compile errors (`ClavusContextBuilder.AppendDelegate`, `IClavusContext.Conventions` missing) from unrelated, already-broken code I never touched (confirmed via `git status` showing zero changes to those files).

### Assumptions about Parker's MSBuild surface (not visible in this worktree - task 1.x)

Parker's task 1.4 ("surface `ClavusConfiguration` items to the analyzer via `AdditionalFiles`/`AnalyzerConfigOptionsProvider`") wasn't present in this worktree, so I designed `ConfigurationDiscovery.cs` against an assumed shape, documented in that file's XML doc remarks:
- Each `ClavusConfiguration` item surfaces as an `AdditionalText`.
- Per-file `AnalyzerConfigOptions` carry `build_metadata.AdditionalFiles.ClavusConfiguration` = `"true"` (marks the item), `build_metadata.AdditionalFiles.ClavusConfigurationBaseName` (layering group key, e.g. `appsettings`), and `build_metadata.AdditionalFiles.ClavusConfigurationLayer` (`Base`/`Environment`/`Local`).
- Format (JSON/YAML/TOML) is derived from the file extension, not a separate metadata key.
- Global MSBuild properties: `EnableClavusConfiguration` (bool) and `ClavusConfigurationNodaTime` (bool, NodaTime opt-in - name guessed, not specified verbatim anywhere in design.md/tasks.md).

If Parker's actual plumbing uses different metadata key names, only `ConfigurationDiscovery.GetSourceFiles`/`TryRead` need to change - everything downstream (grouping, type inference, class/part emission, marker/manifest emission) is decoupled from the exact metadata shape.

### Scope boundaries I deliberately kept narrow

- Task 3.1 is JSON-only by its literal wording; I implemented `JsonFlatConfigurationReader` fully but did not write YAML/TOML generator-side shape readers (only added the `YamlDotNet`/`Tomlyn` dependencies per 5.1/5.2). `ConfigurationDiscovery.GroupJsonFiles` filters to JSON groups only for class/part codegen; `GroupAllFiles` (format-agnostic) still emits marker attributes for every discovered format, since packaging visibility (task 2.6) isn't format-restricted.
- Tomlyn is pinned to `0.20.0` in `Directory.Packages.props`, not the latest (`2.10.1`), because `1.0.0`+ requires `System.Text.Json >= 10.0.2`, which conflicts with the `9.0.9` pin in `src/Clavus.Analyzers.supports/*.csproj` (needed for the older Roslyn-host support builds). `0.20.0` has zero dependencies and still targets `netstandard2.0`.
- Did not touch section 6 (`IOptions`/reload wiring beyond a plain `.Bind()` call) or section 1 (MSBuild property/item definitions) - explicitly out of my assigned scope per the Coordinator's task list.
- Did not write any Verify snapshot tests (2.8, 3.4, 3.7, 4.4, 4.6) - explicitly Ash's per the task brief. My generator output is deterministic (sorted keys, ordered groups) and should be directly testable once Ash's snapshot infra points at it.
