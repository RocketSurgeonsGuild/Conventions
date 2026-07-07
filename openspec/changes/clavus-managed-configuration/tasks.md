## 1. MSBuild Foundation

- [ ] 1.1 Add `EnableClavusConfiguration` MSBuild property (default `false`) to `Clavus.Sdk`, gating the entire pipeline
- [ ] 1.2 Add `ClavusConfiguration` MSBuild item group and conventional-file-name globbing for `appsettings.{ext}`, `appsettings.{Environment}.{ext}`, and `appsettings.local.{ext}` (`.json`/`.yaml`/`.yml`/`.toml`) in `Clavus.Sdk`
- [ ] 1.3 Add MSBuild property for NodaTime type generation opt-in (default `false`)
- [ ] 1.4 Surface discovered `ClavusConfiguration` items to the analyzer via `AdditionalFiles`/`AnalyzerConfigOptionsProvider`, consistent with existing `ClavusMetadata`/`ClavusHostType` plumbing
- [ ] 1.5 Implement base → environment → local layering/precedence resolution for discovered configuration files sharing the same base name
- [ ] 1.6 Scaffold a `.gitignore` entry for `appsettings.local.{ext}` by default in `Clavus.Sdk`-based projects

## 2. Packaging and Distribution (clavus-config-packaging)

- [ ] 2.1 Add `Clavus.Sdk` target that routes each non-local `ClavusConfiguration` item into `Pack` with a stable `PackagePath`, explicitly excluding `appsettings.local.{ext}`
- [ ] 2.2 Verify packed config files round-trip correctly through `dotnet pack` → restore for a sample multi-target project, and verify `appsettings.local.{ext}` never appears in the produced `.nupkg`
- [ ] 2.3 Add `Clavus.Sdk` target that copies `ClavusConfiguration` items into a referencing project's output directory when consumed via same-solution `ProjectReference`
- [ ] 2.4 Ensure copied-file relative layout matches the packed-package relative layout (debug/package parity)
- [x] 2.5 Define the `[assembly: Clavus.ConfigurationAssembly(...)]` marker attribute in `Clavus` core
- [x] 2.6 Emit the marker attribute from `Clavus.Analyzers` for each project with discovered configuration files
- [x] 2.7 Add a host-side generator pass that reads configuration markers off referenced assemblies (reusing the existing convention-export reference walk) and emits a `ClavusConfigurationManifest` type
- [ ] 2.8 Add Verify snapshot tests for manifest generation: single contributor, transitive multi-level contributors, and non-contributing references excluded

## 3. Strongly-Typed Configuration Generation (clavus-config-generation)

- [x] 3.1 Implement a generator-side flat key→raw-string-value reader for JSON configuration files
- [x] 3.2 Design and implement the generated configuration class shape (nested objects → nested generated classes), emitted into the owning library's root namespace
- [x] 3.3 Implement the type-inference precedence chain (`TimeSpan` → `DateOnly` → `TimeOnly` → `DateTimeOffset` → primitive fallback) with strict anchored-regex shape checks
- [ ] 3.4 Add adversarial-input Verify snapshot tests for type inference (`"24:00"`, `"1.00:00:00"`, `"00:00:00.5"`, ISO date/time/duration edge cases)
- [x] 3.5 Implement NodaTime type-mode substitution (`LocalDate`/`LocalTime`/`OffsetDateTime`/`Duration`) gated on the MSBuild property and a `NodaTime` reference check via `Compilation.ReferencedAssemblyNames`
- [x] 3.6 Implement diagnostic `CLAVUS_CFG002` for NodaTime property enabled without a `NodaTime` reference
- [ ] 3.7 Add Verify snapshot tests covering BCL mode, NodaTime mode, and the mismatch diagnostic

## 4. IConfigurationPart and Export Integration (clavus-config-generation)

- [x] 4.1 Define the `IConfigurationPart` interface in `Clavus` core, alongside `IClavusPart` (already exists, generated via the pre-existing `ClavusPart` MSBuild-item templating mechanism - see decisions.md/history.md; reused rather than duplicated)
- [x] 4.2 Generate an `IConfigurationPart` implementation per discovered configuration file that registers the appropriate `IConfigurationSource` and binds the generated configuration class
- [x] 4.3 Update the exports generator to unconditionally include generated `IConfigurationPart` types in a library's export set (no `[Convention]` attribute required)
- [ ] 4.4 Add Verify snapshot tests confirming generated `IConfigurationPart`s appear in the export set without any attribute decoration
- [x] 4.5 Extend the existing convention export/import reference-graph walk to flatten a project's referenced libraries' `IConfigurationPart` exports into its own export set (no new code needed - rides the existing export/import graph, see decisions.md)
- [ ] 4.6 Add Verify snapshot tests for transitive configuration flow across two and three levels of project references, including the "intermediate library declares no configuration of its own" case

## 5. Multi-Format Runtime Support (clavus-config-runtime)

- [x] 5.1 Select and add generator-side (netstandard2.0-compatible) YAML parsing dependency for shape inference (`YamlDotNet`; dependency wired, shape-reader implementation left as follow-up - see history.md)
- [x] 5.2 Select and add generator-side (netstandard2.0-compatible) TOML parsing dependency for shape inference (`Tomlyn` 0.20.0, pinned below 1.x/2.x to avoid a `System.Text.Json` version conflict; dependency wired, shape-reader implementation left as follow-up - see history.md)
- [ ] 5.3 Select and wire runtime `IConfigurationSource`/`IConfigurationProvider` for YAML with `reloadOnChange` support
- [ ] 5.4 Select and wire runtime `IConfigurationSource`/`IConfigurationProvider` for TOML with `reloadOnChange` support
- [ ] 5.5 Split YAML/TOML runtime provider references into conditional `ItemGroup`s so they're only pulled in when a file of that format is actually declared
- [ ] 5.6 Add Verify/integration tests confirming JSON, YAML, and TOML configuration files produce equivalent generated classes and bind equivalent values

## 6. IOptions Registration and Reload (clavus-config-runtime)

- [ ] 6.1 Implement `services.AddOptions<T>().Bind(...)` registration inside each generated `IConfigurationPart`
- [ ] 6.2 Wire `IOptionsChangeTokenSource<T>` registration so `IOptionsMonitor<T>`/`IOptionsSnapshot<T>` reload correctly on file change
- [ ] 6.3 Add integration tests verifying `IOptions<T>` and `IOptionsMonitor<T>` resolve the generated configuration type after startup
- [ ] 6.4 Add integration tests verifying a file-system change to the underlying config file triggers `IOptionsMonitor<T>` change callbacks, across all three supported formats

## 7. Testing and Validation

- [ ] 7.1 Add end-to-end `Clavus.Analyzers.Tests` scenarios combining packaging + generation + export + runtime binding for a representative sample library/host pair
- [ ] 7.2 Confirm incrementality: unrelated source edits do not invalidate the configuration generator pipeline stage (per existing incrementality snapshot conventions)
- [ ] 7.3 Add `RS0017` public API tracking entries for all new public types (`IConfigurationPart`, `ClavusConfigurationManifest`, MSBuild-driven generated classes' public surface expectations)
- [ ] 7.4 Run full solution build (`mise run build`) and full test suite to confirm no regressions in existing convention generation behavior

## 8. Documentation and Dogfooding

- [ ] 8.1 Write a concept guide (Astro/Starlight `docs/`) explaining authoring library configuration and consuming it from a host application
- [ ] 8.2 Document the MSBuild surface: `EnableClavusConfiguration`, `ClavusConfiguration` item, NodaTime opt-in property
- [ ] 8.3 Dogfood the feature by migrating `Clavus.Serilog`'s configuration surface onto the new pipeline
- [ ] 8.4 Capture open questions from `design.md` (per-property type override, generated-type naming for multiple config files, human-readable manifest file) as follow-up issues if not resolved during implementation
