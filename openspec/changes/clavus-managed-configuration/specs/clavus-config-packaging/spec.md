## ADDED Requirements

### Requirement: Config file discovery

The system SHALL discover a project's configuration files using the standard `appsettings` naming convention — a base `appsettings.{ext}` file, optional environment-specific `appsettings.{Environment}.{ext}` files, and an optional `appsettings.local.{ext}` file for developer-local overrides, for each supported extension (`.json`, `.yaml`/`.yml`, `.toml`) — or by explicit declaration via a `ClavusConfiguration` MSBuild item, without requiring any source code changes.

#### Scenario: Base file is discovered automatically

- **WHEN** a project with `EnableClavusConfiguration` set to `true` contains an `appsettings.json` file at its root and declares no explicit `ClavusConfiguration` item
- **THEN** the build SHALL treat `appsettings.json` as a discovered configuration file for that project

#### Scenario: Environment-specific file is discovered automatically

- **WHEN** a project contains an `appsettings.Production.json` file at its root
- **THEN** the build SHALL treat `appsettings.Production.json` as a discovered configuration file layered against the `Production` environment

#### Scenario: Local override file is discovered automatically

- **WHEN** a project contains an `appsettings.local.json` file at its root
- **THEN** the build SHALL treat `appsettings.local.json` as a discovered, developer-local configuration file for that project

#### Scenario: Explicitly declared file is discovered

- **WHEN** a project declares `<ClavusConfiguration Include="config/settings.yaml" />`
- **THEN** the build SHALL treat `config/settings.yaml` as a discovered configuration file for that project, in addition to any conventionally named files present

### Requirement: Configuration layering and precedence

The system SHALL layer discovered configuration files belonging to the same base configuration set in ascending order of precedence: `appsettings.{ext}` (lowest), then `appsettings.{Environment}.{ext}`, then `appsettings.local.{ext}` (highest). A value present in a higher-precedence file SHALL override the same key's value from any lower-precedence file.

#### Scenario: Environment file overrides base file value

- **WHEN** `appsettings.json` and `appsettings.Production.json` both define a value for the same key
- **THEN** the effective value at runtime in the `Production` environment SHALL be the value from `appsettings.Production.json`

#### Scenario: Local file overrides both base and environment file values

- **WHEN** `appsettings.json`, `appsettings.Production.json`, and `appsettings.local.json` all define a value for the same key
- **THEN** the effective value at runtime SHALL be the value from `appsettings.local.json`

#### Scenario: Base value is used when no override is present

- **WHEN** a key is defined only in `appsettings.json` and not in any environment-specific or local file
- **THEN** the effective value at runtime SHALL be the value from `appsettings.json`

### Requirement: Local override files are excluded from packaging and version control

The system SHALL exclude `appsettings.local.{ext}` files from the packed NuGet package output, and SHALL NOT require or expect them to be committed to source control.

#### Scenario: Local override file is not packed

- **WHEN** a project containing an `appsettings.local.json` file is packed via `dotnet pack`
- **THEN** the produced `.nupkg` SHALL NOT contain `appsettings.local.json`

#### Scenario: Local override file is still used at build time when present

- **WHEN** a project containing an `appsettings.local.json` file is built locally
- **THEN** the local override file SHALL still be discovered and layered per the configuration layering and precedence requirement, even though it is never packed or checked in

#### Scenario: New projects gitignore the local override pattern by default

- **WHEN** a new project is scaffolded using `Clavus.Sdk`
- **THEN** the project's `.gitignore` SHALL exclude the `appsettings.local.{ext}` file pattern by default

### Requirement: Packing configuration files into the NuGet package

The system SHALL include every discovered, non-local configuration file in the project's produced NuGet package on `dotnet pack`, using a stable, predictable package path so consuming projects can locate it after restore.

#### Scenario: Config file is packed

- **WHEN** a project with a discovered `appsettings.json` file is packed via `dotnet pack`
- **THEN** the produced `.nupkg` SHALL contain `appsettings.json` at its designated package path

#### Scenario: Multiple config files are all packed

- **WHEN** a project declares more than one `ClavusConfiguration` item, or discovers both a base file and an environment-specific file
- **THEN** the produced `.nupkg` SHALL contain all declared/discovered non-local configuration files, each at its own designated package path

### Requirement: Copying configuration files to a same-solution host application

The system SHALL copy a library's discovered configuration files into a referencing project's build output directory when the library is consumed via `ProjectReference` within the same solution, mirroring the layout the file would have if consumed as a restored NuGet package.

#### Scenario: Config file is copied on build via ProjectReference

- **WHEN** a host application project has a `ProjectReference` to a library project that declares a discovered configuration file
- **THEN** building the host application SHALL copy that configuration file into the host application's output directory

#### Scenario: Config file layout matches packaged layout

- **WHEN** a configuration file is copied to a host application via `ProjectReference`
- **THEN** its relative path under the host application's output directory SHALL match the relative path it would have when restored from the packed NuGet package

### Requirement: Host-visible configuration manifest

The system SHALL make available, at compile time, a manifest listing every referenced assembly that contributes configuration and the relative path(s) of the configuration file(s) it contributes, so the host application can enumerate its configuration sources without runtime file-system probing.

#### Scenario: Manifest lists a single contributing assembly

- **WHEN** a host application references exactly one project/package that contributes a configuration file
- **THEN** the generated manifest available to the host application SHALL list that assembly and the relative path of its configuration file

#### Scenario: Manifest lists multiple contributing assemblies transitively

- **WHEN** a host application references a project that itself references another project, and both contribute configuration files
- **THEN** the generated manifest available to the host application SHALL list both contributing assemblies and their respective configuration file paths

#### Scenario: Manifest omits non-contributing assemblies

- **WHEN** a host application references a project that does not declare or discover any configuration file
- **THEN** the generated manifest SHALL NOT list that project as a configuration-contributing assembly
