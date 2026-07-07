# Squad Team

> Clavus (formerly Rocket.Surgery.Conventions) — a .NET convention-driven bootstrapping framework with a Roslyn incremental source generator at its core.

## Coordinator

| Name | Role | Notes |
|------|------|-------|
| Squad | Coordinator | Routes work, enforces handoffs and reviewer gates. |

## Members

| Name | Role | Charter | Status |
|------|------|---------|--------|
| Dallas | Lead | `.squad/agents/dallas/charter.md` | ✅ Active |
| Ripley | Generator Dev | `.squad/agents/ripley/charter.md` | ✅ Active |
| Parker | Build Engineer | `.squad/agents/parker/charter.md` | ✅ Active |
| Ash | Tester | `.squad/agents/ash/charter.md` | ✅ Active |
| Lambert | Docs | `.squad/agents/lambert/charter.md` | ✅ Active |
| Scribe | Session Logger | `.squad/agents/scribe/charter.md` | 📋 Silent |
| Ralph | Work Monitor | — | 🔄 Monitor |

## Project Context

- **Owner:** David Driscoll
- **Stack:** C# (LangVersion preview), .NET 10 primary (net8.0/netstandard2.0 for compat + the generator), Roslyn incremental source generators, custom MSBuild SDK, TUnit + Verify snapshot testing, FakeItEasy, Shouldly, ModularPipelines build script, mise task runner, Astro/Starlight docs
- **Description:** Auto-discovers and orders `[Convention]`-attributed classes at compile time (via `Clavus.Analyzers`) and emits the startup wiring with zero runtime reflection, so apps get convention-driven bootstrapping across hosting models (Web, MAUI, WASM, Aspire) and DI containers (Autofac, DryIoc) without hand-wiring extension methods.
- **Created:** 2026-07-06
