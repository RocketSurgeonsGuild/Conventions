# Project Context

- **Owner:** David Driscoll
- **Project:** Clavus (formerly Rocket.Surgery.Conventions) — convention-driven .NET bootstrapping via a Roslyn incremental source generator, with MSBuild SDK tooling and hosting/DI integrations
- **Stack:** C# (LangVersion preview), .NET 10 primary (net8.0/netstandard2.0 for the generator itself), Roslyn `IIncrementalGenerator`, TUnit/Verify snapshot testing
- **Created:** 2026-07-06

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- `Clavus.Analyzers` reads MSBuild properties (`ClavusMetadata`, `ClavusAssignExternal`, `ClavusHostType`, `IsTestProject`) through an `AnalyzerConfigOptionsProvider` — any new generator behavior gated by project config should go through this same path.
- Generator output regressions are caught via Verify snapshots in `test/Clavus.Analyzers.Tests/snapshots/`, driven by `GeneratorTest.cs`.
