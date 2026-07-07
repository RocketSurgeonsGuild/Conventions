# Project Context

- **Owner:** David Driscoll
- **Project:** Clavus (formerly Rocket.Surgery.Conventions) — convention-driven .NET bootstrapping via a Roslyn incremental source generator, with MSBuild SDK tooling and hosting/DI integrations
- **Stack:** Custom MSBuild SDK (`Clavus.Sdk`), central package management, ModularPipelines build script, mise task runner, GitVersion/GitReleaseManager
- **Created:** 2026-07-06

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- Build orchestration goes through `mise run build`, which invokes `dotnet run build/Build.cs` (a ModularPipelines-based C# build script using Sourcy/GitVersion) — not raw `dotnet build` for CI-equivalent runs.
- `RS0017` (public API tracking) is the only warning configured as an error across the solution.
