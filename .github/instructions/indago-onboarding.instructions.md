---
name: Indago Codebase Onboarding
description: Architecture, conventions, and navigation guide for the Indago compile-time assembly-scanning library
applyTo: '**'
---

# Indago Codebase Onboarding

Indago is a **compile-time assembly/type-scanning library** for .NET from Rocket Surgeons Guild. It replaces runtime reflection-based DI scanning (Scrutor-style) with a **Roslyn incremental source generator** that evaluates selector expressions at build time and emits a strongly-typed `IIndagoProvider`. This makes scanning AOT/trimming-friendly and eliminates runtime reflection cost.

The public API surface is intentionally tiny — nearly all the work lives in the generator.

## Tech Stack

| Layer               | Technology                                              | Version                         |
| ------------------- | ------------------------------------------------------- | ------------------------------- |
| Language            | C#                                                      | preview (`LangVersion=preview`) |
| Runtime targets     | .NET 8 + .NET 10                                        | `net8.0;net10.0`                |
| Generator target    | netstandard2.0                                          | Roslyn component                |
| Roslyn              | Microsoft.CodeAnalysis.CSharp                           | 5.3.0 (also 4.8, 4.14 variants) |
| Testing framework   | TUnit on Microsoft.Testing.Platform                     | 1.56.x                          |
| Assertions          | Shouldly                                                | 4.x                             |
| Mocks               | FakeItEasy                                              | —                               |
| Snapshot testing    | Verify.TUnit                                            | 31.x                            |
| Build orchestration | ModularPipelines (`mise run build`)                     | —                               |
| CI                  | GitHub Actions + mise                                   | —                               |
| Package management  | Central Package Management (`Directory.Packages.props`) | —                               |
| Formatting          | prettier + dotnet format                                | prettier 3.8.x                  |
| Docs site           | VitePress                                               | 2.0.0-alpha                     |

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│  User Assembly                                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ provider.GetTypes(s => s.FromAssemblyOf<MyClass>()      │   │
│  │                    .AddClasses().AsMatchingInterface())  │   │
│  └─────────────┬───────────────────────────────────────────┘   │
│                │ selector expression hashed at call site        │
└────────────────┼────────────────────────────────────────────────┘
                 │
         [Build time — Roslyn IIncrementalGenerator]
                 │
┌────────────────▼────────────────────────────────────────────────┐
│ IndagoProviderGenerator                                         │
│  ├── AssemblyCollection   → GetAssemblies() call sites          │
│  ├── ReflectionCollection → GetTypes() call sites               │
│  └── ServiceDescriptorCollection → Scan() call sites            │
│                                                                  │
│  Cross-assembly cache: IndagoProvider.ctpjson (AdditionalText)  │
│  Emits: IndagoProvider.g.cs  (IIndagoProvider implementation)   │
└─────────────────────────────────────────────────────────────────┘
```

**How a scan resolves:**

1. A call to `provider.GetTypes(selector)` is found by the syntax provider via `[CallerArgumentExpression]`.
2. The generator hashes the selector expression string to produce a stable key.
3. `AssemblyProviders/` symbol visitors walk Roslyn `ITypeSymbol` graphs applying compiled filters (namespaces, type kinds, attribute presence, assignability).
4. Results are serialized to `IndagoProvider.ctpjson` so downstream assemblies consume them without re-resolving.
5. The generator emits a concrete class that returns pre-resolved collections — zero reflection at runtime.

## Directory Map

```
src/Indago/                     — Runtime NuGet package (IIndagoProvider, attributes, DI extensions)
  Abstractions/                 — Fluent selector interfaces and opt-out attributes
  build*/                       — MSBuild props/targets packed INTO the NuGet package (not build scripts)

src/Indago.Analyzers/           — The IIncrementalGenerator (netstandard2.0, packed as analyzer)
  AssemblyProviders/            — Roslyn symbol visitors + compiled type/assembly filters
  Configuration/                — JSON-serializable data records + JsonSourceGenerationContext
  Descriptors/                  — IAssemblyDescriptor / ITypeFilterDescriptor implementations

src/Indago.Analyzers.supports/  — Multi-Roslyn-version variant projects (roslyn4.8, 4.14, 5.0)
                                  Each pins a specific Roslyn version via VersionOverride to verify
                                  generator compatibility with older Visual Studio hosts

test/Indago.Tests/              — TUnit + Verify generator tests
  snapshots/                    — Verified .cs snapshots of generated output
test/TestAssembly/              — Sample types the generator scans in tests

.build/                         — Nuke pipeline (not used day-to-day)
build/                          — ModularPipelines pipeline — invoked by `mise run build` and CI
docs/                           — VitePress docs site (`mise run docs`)
```

> **Critical:** `.build/` (Nuke) and `build/` (ModularPipelines) are two different build systems. The `src/Indago/build*/` directories are MSBuild props/targets packed into the NuGet package — they are **not** build scripts.

## Key Entry Points

| Purpose                     | Path                                                      |
| --------------------------- | --------------------------------------------------------- |
| Public API contract         | `src/Indago/IIndagoProvider.cs`                           |
| Generator entry point       | `src/Indago.Analyzers/CompiledTypeProviderGenerator.cs`   |
| Assembly filter visitors    | `src/Indago.Analyzers/AssemblyProviders/`                 |
| Serializable config records | `src/Indago.Analyzers/Configuration/`                     |
| Compiled filter descriptors | `src/Indago.Analyzers/Descriptors/`                       |
| Fluent selector interfaces  | `src/Indago/Abstractions/`                                |
| DI registration helper      | `src/Indago/IndagoProviderServiceCollectionExtensions.cs` |
| Test fixture types          | `test/TestAssembly/`                                      |
| Snapshot tests              | `test/Indago.Tests/AssemblyScanningTests.cs`              |

## Common Tasks

| Task                    | Command                                                                                                                   |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| Build                   | `dotnet build Indago.sln`                                                                                                 |
| Run all tests           | `dotnet test test/Indago.Tests/Indago.Tests.csproj`                                                                       |
| Run a single test       | `dotnet run --project test/Indago.Tests -- --treenode-filter "/*/*/AssemblyScanningTests/Should_Generate_All_The_Things"` |
| Full CI build           | `mise run build`                                                                                                          |
| Docs dev server         | `mise run docs`                                                                                                           |
| Accept snapshot changes | `dotnet verify accept`                                                                                                    |
| Format code             | `prettier --write .` and `dotnet format`                                                                                  |

## Conventions

### Code Style

- `Nullable=enable`, `ImplicitUsings=enable`, `LangVersion=preview`, `Features=strict`
- Global usings: `JetBrains.Annotations`, `System.Diagnostics.CodeAnalysis`, `NotNullAttribute` alias
- Public types annotated `[PublicAPI]`
- `RS0017` is a build error — public API surface is tracked; changing it requires updating the public API file

### Naming

- Files: PascalCase matching the type name
- Data records (serializable config): `*Data` suffix
- Compiled filter logic: `*Descriptor` suffix
- Fluent selectors: `I*Selector` / `I*Filter` interfaces

### Analyzer Project Constraints

- `Indago.Analyzers` targets `netstandard2.0` with `EnforceExtendedAnalyzerRules`
- Use `Polyfill` for newer APIs — do **not** reference runtime-only packages from it
- The `Indago.Analyzers.supports/` projects pin older Roslyn versions for compatibility testing

### Package Management

- All versions in `Directory.Packages.props` — add `<PackageVersion>` there, reference without a version in the csproj
- `GlobalPackageReference` entries apply analyzers/build tooling to every project automatically

### Snapshot Tests

- Generated output is verified against `test/Indago.Tests/snapshots/*.verified.cs`
- A failing snapshot means the generated code changed — review the `.received.*` diff
- Accept intended changes with `dotnet verify accept` or by moving `.received.` → `.verified.`
- Temp paths are scrubbed to `{TempPath}` in snapshots

## Where to Look

| I want to…                      | Look at…                                                                                       |
| ------------------------------- | ---------------------------------------------------------------------------------------------- |
| Change what the generator emits | `src/Indago.Analyzers/CompiledTypeProviderGenerator.cs`                                        |
| Add a new type filter           | `src/Indago.Analyzers/Descriptors/` + `src/Indago/Abstractions/ITypeFilter.cs`                 |
| Add a new assembly filter       | `src/Indago.Analyzers/Descriptors/IAssemblyDescriptor.cs` implementors                         |
| Change serialized cache shape   | `src/Indago.Analyzers/Configuration/` + update `JsonSourceGenerationContext`                   |
| Change the public API           | `src/Indago/IIndagoProvider.cs` (update RS0017 public API tracking file too)                   |
| Add a DI registration feature   | `src/Indago/IndagoProviderServiceCollectionExtensions.cs`                                      |
| Add test fixtures               | `test/TestAssembly/`                                                                           |
| Add a snapshot test             | `test/Indago.Tests/AssemblyScanningTests.cs` — new test auto-creates `.received.cs`, accept it |
| Add a Roslyn version variant    | `src/Indago.Analyzers.supports/` — copy and pin Roslyn version                                 |
