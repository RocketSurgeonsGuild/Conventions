# Open Questions — clavus-managed-configuration

Captured from `design.md` § "Open Questions" per task 8.4. These are follow-up items to track as
GitHub issues once repo write access is available from a worktree that has it — this file is the
interim record. Remove an entry (or link its resolving PR/issue) once it's settled.

## 1. Per-property type override

**Question:** Should there be an explicit per-property type override (e.g. a sibling
`.clavus.types.json` file, or an inline `$type` hint) for cases where the type-inference chain
(`TimeSpan` → `DateOnly` → `TimeOnly` → `DateTimeOffset` → primitive fallback) guesses wrong? Or
is "regenerate as `string` and cast/parse manually" an acceptable escape hatch for v1?

**Status:** Unresolved as of this writing. `design.md` Decision 3 treats inference as opt-out
rather than opt-in, but does not commit to a specific override mechanism.

**Suggested follow-up issue:**

- Title: "Design per-property type override for Clavus managed configuration generator"
- Scope: Decide between (a) no override for v1 (string fallback only), (b) item metadata on
  `ClavusConfiguration` (e.g. `ClavusConfiguration Include="appsettings.json" TypeOverrides="Foo.Bar=string"`),
  or (c) a sibling manifest file. Evaluate against the "zero new files to author" goal that
  motivated conventional appsettings globbing in the first place.

## 2. Generated-type naming for multiple config files

**Question:** What's the exact generated-type naming convention when a library declares more
than one configuration file? Suffix by file name (e.g. `AppSettingsConfiguration` +
`FeatureFlagsConfiguration`)? Require explicit item metadata (e.g. `ClassName="FeatureFlags"`) to
avoid collisions or awkward auto-derived names?

**Status:** Unresolved. `design.md` Decision 4 only specifies the single-file case
(`appsettings.json` → `AppSettingsConfiguration`).

**Suggested follow-up issue:**

- Title: "Define generated-type naming convention for multiple ClavusConfiguration files per project"
- Scope: Pin down the derivation rule for the common cases (`appsettings.json` +
  `appsettings.Development.json` should almost certainly _not_ generate two separate types, since
  they're layers of the same file — this needs to be disambiguated from genuinely distinct
  additional files like `settings/feature-flags.json`), and decide whether `ClassName` item
  metadata is needed as an explicit override.

## 3. Human-readable configuration manifest

**Question:** Should the host-visible configuration manifest (`ClavusConfigurationManifest`,
Decision 2) be a generated type only, or should `Clavus.Sdk` also emit a human-readable file
(e.g. `bin/clavus-configuration.json`) for tooling/ops visibility outside the compiler?

**Status:** Unresolved. Decision 2 explicitly treats the generated type as the single source of
truth for v1 and defers a human-readable artifact as something "an MSBuild target can regenerate
... if needed later."

**Suggested follow-up issue:**

- Title: "Evaluate need for a human-readable Clavus configuration manifest file"
- Scope: Survey whether ops/tooling scenarios (e.g. a deployment script that wants to know which
  config files a published app depends on without invoking Roslyn) actually need this before
  building it. If so, spec it as an MSBuild target that reads the same generated-type data via
  reflection at build time.

## Related, not yet split into issues

Two additional open items from `design.md` fall outside this change's `clavus-managed-configuration`
capability set as scoped (they concern YAML/TOML support specifically, tracked under
`clavus-config-runtime`), but are worth carrying forward if not resolved during implementation:

- Nested object / array-of-objects representation for YAML/TOML (nested generated classes vs.
  flattened dotted-key sections) — needs pinning down before the generator's shape-inference walk
  for non-JSON formats is implemented.
- Choice of YAML and TOML libraries for the generator-side parse dependency vs. the runtime
  provider dependency — needs a `netstandard2.0` compatibility check (generator host constraint)
  before committing to either.
