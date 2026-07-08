# Project Context

- **Owner:** David Driscoll
- **Project:** Clavus (formerly Rocket.Surgery.Conventions) — convention-driven .NET bootstrapping via a Roslyn incremental source generator, with MSBuild SDK tooling and hosting/DI integrations
- **Stack:** Custom MSBuild SDK (`Clavus.Sdk`), central package management, ModularPipelines build script, mise task runner, GitVersion/GitReleaseManager
- **Created:** 2026-07-06

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- Build orchestration goes through `mise run build`, which invokes `dotnet run build/Build.cs` (a ModularPipelines-based C# build script using Sourcy/GitVersion) — not raw `dotnet build` for CI-equivalent runs.
- `RS0017` (public API tracking) is the only warning configured as an error across the solution.
- `RS0017` (public API tracking) is the only warning configured as an error across the solution, but there is no `PublicAPI.Unshipped.txt`/PublicAPIAnalyzers infrastructure anywhere in the repo yet — the gate is currently a no-op. Whoever adds the first new public C# type needs to wire that infrastructure up, not just add entries to an existing file.
- Worktrees for this squad's agents are pinned to whatever commit existed when the worktree was created; the shared repo's `feature/clavus` branch can move ahead in the meantime (e.g. `Clavus.Sdk` didn't exist in my worktree until I fast-forward-merged local `feature/clavus`). Check `git log --oneline -1 -- <path>` in both the worktree and the main repo checkout before concluding something "doesn't exist yet."
- `openspec/` is untracked in this repo (by design, presumably) — each worktree has its own filesystem snapshot of it, and edits inside a worktree sandbox cannot touch the main repo's copy directly (blocked by the harness). Shared coordination files like `tasks.md` need the Scribe/coordinator to reconcile per-worktree edits back into the canonical copy.
- MSBuild batching gotcha: within one `<ItemGroup><X>...</X></ItemGroup>` block, sibling metadata `Condition`s all evaluate against the item's state as of the start of that block — they do NOT see each other's newly-assigned values within the same block, even in document order. Any metadata computation that depends on another metadata value set earlier in the same target must go in its own, subsequent `<ItemGroup>` step. Cost a full debugging cycle on the clavus-managed-configuration layer/precedence logic; see `src/Clavus.Sdk/Sdk/Sdk.Configuration.targets` for the working pattern and inline comment.

## clavus-managed-configuration (2026-07-06)

Implemented my assigned slice of `openspec/changes/clavus-managed-configuration/tasks.md`: tasks 1.1-1.6 (MSBuild Foundation) and 2.1-2.4 (Packaging and Distribution), plus 7.3/7.4 verification.

- Added `EnableClavusConfiguration` (default `false`, gates the whole pipeline) and `ClavusConfigurationEnableNodaTime` (default `false`) properties to `src/Clavus.Sdk/Sdk/Sdk.props`, plus the conventional `appsettings.{ext}`/`appsettings.*.{ext}` glob for `ClavusConfiguration` items (json/yaml/yml/toml), gated on `EnableClavusConfiguration`.
- New file `src/Clavus.Sdk/Sdk/Sdk.Configuration.targets` (imported from `Sdk.targets`), covering:
  - Item-metadata computation (`Format`/`BaseName`/`Layer`/`EnvironmentName`/`IsLocal`/`Precedence`) for base/environment/local layering, resolved via regex-based filename parsing.
  - `AdditionalFiles` + `CompilerVisibleItemMetadata`/`CompilerVisibleProperty` wiring so the (future) generator can read discovered config files and their layer metadata per-file, mirroring the existing `ClavusMetadata`/`ClavusHostType` `CompilerVisibleProperty` pattern.
  - Pack routing: non-local items -> `contentFiles/any/any/clavus/<file>`, local excluded.
  - Cross-`ProjectReference` copy target (`GetClavusConfigurationItems` callback target + `MSBuild` task collection, same pattern the SDK itself uses for `GetCopyToOutputDirectoryItems`-style propagation) copying into the referencing project's output under `clavus/`, matching the packed layout for debug/package parity.
  - `.gitignore` scaffold target for `appsettings.local.{ext}`, idempotent and BOM-free.
- Verified via isolated MSBuild smoke tests (bypassing NuGet-network-dependent `Clavus`/`Clavus.Analyzers` package restores) plus a real `dotnet pack` -> `dotnet restore` round trip against a throwaway multi-target test package — confirmed `.nupkg` contents and restored package cache both exclude `appsettings.local.json` and include the base/environment files at the expected path.
- Full `dotnet build Clavus.slnx`: 33 projects succeed; only a pre-existing, unrelated `sample/AspireSample` MSB3202 error remains (confirmed present with my changes stashed out). `mise run build` itself is currently broken on this branch for an unrelated, pre-existing reason (`build/Build.cs` references `ClavusContextBuilder`, not resolvable via that file's NuGet-only package references) — also confirmed present without my changes.
- Flagged two things for the squad in `.squad/decisions/inbox/parker-clavus-configuration-msbuild-surface.md`: (1) the new `ClavusConfiguration` *item* coexists with a pre-existing, unrelated `ClavusConfiguration` *property* (build config Debug/Release) in `Clavus.Reference.targets` — not a real collision since items/properties are separate namespaces, but easy to misread; (2) task 7.3 (RS0017 entries) is N/A for my slice since 1.1-1.6/2.1-2.4 introduce zero new public C# types.
- Did not touch: 2.5-4.6 (marker attribute, manifest generator, `IConfigurationPart`, export integration — Ripley's/others'), 5.x/6.x (runtime providers/IOptions — Dallas's), 7.1/7.2 (end-to-end + incrementality tests — Ash's), 8.x (docs/dogfooding — Lambert's).
