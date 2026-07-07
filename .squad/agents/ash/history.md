# Project Context

- **Owner:** David Driscoll
- **Project:** Clavus (formerly Rocket.Surgery.Conventions) — convention-driven .NET bootstrapping via a Roslyn incremental source generator, with MSBuild SDK tooling and hosting/DI integrations
- **Stack:** TUnit (Microsoft.Testing.Platform), Verify snapshot testing, FakeItEasy, Shouldly
- **Created:** 2026-07-06

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- Generator output regressions are the main risk surface in this repo — `test/Clavus.Analyzers.Tests` with its `snapshots/` directory is the key guard. Any generator change should come with an updated, reviewed snapshot.
- Test stack is TUnit, not xUnit/NUnit/MSTest — uses Microsoft.Testing.Platform runner conventions.
