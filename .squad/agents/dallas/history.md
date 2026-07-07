# Project Context

- **Owner:** David Driscoll
- **Project:** Clavus (formerly Rocket.Surgery.Conventions) — convention-driven .NET bootstrapping via a Roslyn incremental source generator, with MSBuild SDK tooling and hosting/DI integrations
- **Stack:** C# (LangVersion preview), .NET 10 primary, custom MSBuild SDK, TUnit/Verify testing, ModularPipelines build, mise, Astro/Starlight docs
- **Created:** 2026-07-06

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- The Clavus rename is mid-flight on `feature/clavus`. `AGENTS.md`'s "constitution" still references legacy names (`Conventions.Analyzers`, `Conventions.Abstractions`) — watch for drift between old and new naming across docs, `.props`/`.targets`, and code.
