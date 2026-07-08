---
title: Managed Configuration
description: Author library configuration once and have it packaged, typed, and wired into any host application automatically.
tags: [source-generator, configuration, msbuild]
---

import { CardGrid, LinkCard } from '@astrojs/starlight/components';

# Managed Configuration

Clavus extends the same "compile-time discovery + generated wiring" approach it already uses for
[Conventions](/concepts/introduction/) to **configuration**. A library drops an `appsettings`-style
file into its project, and Clavus takes care of the rest: packaging the file into the NuGet
package, copying it into a same-solution host application during development, generating a
strongly-typed configuration class, and registering it with `IOptions<T>` — all without a single
line of hand-written binding code.

> [!NOTE]
> This feature is opt-in. Existing Clavus consumers see no behavior change until they enable it
> (see [MSBuild Surface Reference](#msbuild-surface-reference) below).

## The problem this solves

Without a managed pipeline, a library that ships configuration has to:

- Hand-author an options class that mirrors the config file's shape.
- Hand-wire `IConfiguration` binding, usually in a `[Convention]`-decorated class.
- Ensure the config file itself is actually packed into the NuGet package and lands next to the
  consuming application at run time.

Every library reinvents this, config keys drift from what's actually consumed, and there's no
compile-time guarantee that a library's configuration surface is registered in the host at all.

## Authoring configuration in a library

Add a configuration file using the conventional `appsettings` naming pattern and Clavus discovers
it automatically — most libraries need zero explicit MSBuild declaration:

```
MyLibrary/
├── appsettings.json
├── appsettings.Development.json
├── appsettings.local.json   # never packed, never committed — see below
└── MyLibrary.csproj
```

If you need to include a file that doesn't match the conventional naming pattern, declare it
explicitly with the `ClavusConfiguration` item:

```xml
<ItemGroup>
  <ClavusConfiguration Include="settings/feature-flags.json" />
</ItemGroup>
```

Supported formats are JSON, YAML (`.yaml`/`.yml`), and TOML.

### Layering and precedence

Files sharing a base name layer in ascending precedence, mirroring the familiar ASP.NET Core
`appsettings.json` / `appsettings.{Environment}.json` pattern, with one addition:

1. `appsettings.{ext}` — base configuration.
2. `appsettings.{Environment}.{ext}` — environment-specific overrides.
3. `appsettings.local.{ext}` — highest precedence, developer-machine-only overrides.

`appsettings.local.{ext}` is a pseudo-environment layer that always applies on top of whichever
environment is active. It is **never packed** into the NuGet package and Clavus scaffolds a
`.gitignore` entry for it by default, so it's safe to drop personal secrets, local ports, or
feature toggles into it without fear of committing or shipping them.

### Generated configuration class

For each configuration file, Clavus generates a `sealed partial class` **inside the owning
library's own root namespace** — not the consuming application's — named after the file. For
example, `appsettings.json` in `MyLibrary` generates `MyLibrary.AppSettingsConfiguration`.

Generating the class in the owning library (rather than the consumer) avoids name collisions
across libraries and lets the library bind and use its own configuration internally, in addition
to exposing it to consumers.

### Type inference

Clavus infers a specific CLR type for each leaf value based on its shape, applied in this order
(first strict match wins):

1. `TimeSpan` — duration-shaped strings (`d.hh:mm:ss[.fffffff]` / `hh:mm:ss`).
2. `DateOnly` — ISO 8601 date-only (`yyyy-MM-dd`).
3. `TimeOnly` — time-only, no date component (`HH:mm[:ss[.fff]]`).
4. `DateTimeOffset` — ISO 8601 date+time, with or without an offset.
5. Fallback — `bool`, `int`/`long`, `double`, else `string`.

Each check is anchored to a strict format regex before attempting a parse, so values like
`"14:30"` and `"2024-01-01"` resolve unambiguously instead of guessing.

> [!TIP]
> If inference guesses the "wrong" type for a value, regenerating the property as `string` and
> parsing it yourself is the current escape hatch. A more targeted per-property override is being
> tracked — see [open questions](#known-limitations--open-questions).

### Opt-in NodaTime types

If your library already references [NodaTime](https://nodatime.org/), you can opt the generator
into emitting NodaTime types instead of the BCL equivalents:

| Inferred shape | BCL type (default) | NodaTime type (opt-in) |
| -------------- | ------------------ | ---------------------- |
| Duration       | `TimeSpan`         | `Duration`             |
| Date           | `DateOnly`         | `LocalDate`            |
| Time           | `TimeOnly`         | `LocalTime`            |
| Date + time    | `DateTimeOffset`   | `OffsetDateTime`       |

Enabling the NodaTime property without an actual `NodaTime` reference is a compile-time error
(`CLAVUS_CFG002`) rather than a silent fallback to BCL types — a generated public API's shape
should never depend on an easily-missed reference. NodaTime mode is a **per-library** decision:
a library built with NodaTime types self-declares that fact, so a mismatch with a consumer that
doesn't reference NodaTime surfaces as a compile error, not a runtime failure.

## Consuming configuration from a host application

Nothing extra is required in the host application beyond referencing the library. When a library
declares configuration:

- Its config file(s) are copied into the host's output directory at build time (when referenced
  via a same-solution `ProjectReference`) or restored from the packed NuGet content (when
  referenced as a package) — either way, the on-disk layout matches, so debug-time behavior
  mirrors packaged behavior.
- A generated, host-visible `ClavusConfigurationManifest` lists every referenced assembly that
  contributes configuration and its relative config path, so the host can enumerate configuration
  sources at build time instead of relying on convention alone.
- The library's generated `IConfigurationPart` is automatically included in the library's
  exported convention set — you do **not** need to add `[Convention]` yourself, since
  `IConfigurationPart` instances are generator-authored, not user-decorated.
- Standard `IOptions<T>` / `IOptionsMonitor<T>` resolve the generated configuration type once the
  host application starts, and `IOptionsMonitor<T>` picks up file changes for reload, the same as
  any other `Microsoft.Extensions.Configuration` source.

```c#
public class MyService
{
    private readonly IOptionsMonitor<MyLibrary.AppSettingsConfiguration> _options;

    public MyService(IOptionsMonitor<MyLibrary.AppSettingsConfiguration> options)
        => _options = options;

    public void DoWork()
    {
        var current = _options.CurrentValue;
        // ...
    }
}
```

### Transitive configuration

A library's own configuration dependencies flow to its dependents without redeclaration. If
`LibraryA` references `LibraryB`, and `LibraryB` declares configuration, a host application that
only references `LibraryA` still sees `LibraryB`'s configuration in its manifest and export set —
the generator reuses the same reference-graph walk already used to flatten transitive
[Convention exports](/concepts/source-generation/).

## MSBuild Surface Reference

| Property / Item              | Default                | Purpose                                                                                                                                                                                                                                                                                                                                                          |
| ---------------------------- | ---------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `EnableClavusConfiguration`  | `false`                | Gates the entire managed-configuration pipeline for a project. Existing Clavus consumers see no behavior change until this is set to `true`.                                                                                                                                                                                                                     |
| `ClavusConfiguration` (item) | Conventionally globbed | Declares a configuration file to include in the pipeline. Conventional `appsettings.{ext}` / `appsettings.{Environment}.{ext}` / `appsettings.local.{ext}` files (`.json`/`.yaml`/`.yml`/`.toml`) are discovered automatically — most projects don't need to add this item explicitly. Use it to include files that don't match the conventional naming pattern. |
| NodaTime type-mode property  | `false`                | Opts the generator into emitting [NodaTime](https://nodatime.org/) types (`LocalDate`, `LocalTime`, `OffsetDateTime`, `Duration`) instead of BCL equivalents. Requires an actual `NodaTime` package reference in the same project, or the generator reports `CLAVUS_CFG002`.                                                                                     |

```xml
<PropertyGroup>
  <EnableClavusConfiguration>true</EnableClavusConfiguration>
  <!-- Opt into NodaTime types for date/time/duration properties -->
  <ClavusConfigurationUseNodaTime>true</ClavusConfigurationUseNodaTime>
</PropertyGroup>

<ItemGroup>
  <!-- Only needed for files that don't match the conventional appsettings naming pattern -->
  <ClavusConfiguration Include="settings/feature-flags.json" />
</ItemGroup>
```

> [!NOTE]
> The exact property name for the NodaTime opt-in is still settling as the feature lands; the
> shape above reflects the current design and may be renamed before release. Check the
> `Clavus.Sdk` package's `.props`/`.targets` for the authoritative, currently-shipping name.

## Known limitations / open questions

A few design questions are still open and may change how this feature looks before it's finalized:

- Whether an explicit per-property type override is needed for cases where inference guesses wrong.
- The exact generated-type naming convention when a library declares more than one configuration file.
- Whether the host-visible manifest should also be emitted as a human-readable file (e.g. for
  tooling/ops visibility) in addition to the generated type.

See the [managed configuration open questions](https://github.com/RocketSurgeonsGuild/Conventions/blob/main/openspec/changes/clavus-managed-configuration/open-questions.md)
for the full list and current thinking on each.
