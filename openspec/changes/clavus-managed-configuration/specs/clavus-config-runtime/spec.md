## ADDED Requirements

### Requirement: Multi-format configuration source support

The system SHALL support configuration files authored in JSON, YAML, and TOML, applying the same discovery, packaging, and type-inference behavior regardless of which format a given file uses.

#### Scenario: JSON configuration file is supported

- **WHEN** a discovered configuration file is named with a `.json` extension
- **THEN** the system SHALL parse it as JSON for both generation and runtime configuration binding

#### Scenario: YAML configuration file is supported

- **WHEN** a discovered configuration file is named with a `.yaml` or `.yml` extension
- **THEN** the system SHALL parse it as YAML for both generation and runtime configuration binding

#### Scenario: TOML configuration file is supported

- **WHEN** a discovered configuration file is named with a `.toml` extension
- **THEN** the system SHALL parse it as TOML for both generation and runtime configuration binding

### Requirement: IOptions-based registration of generated configuration

The system SHALL register each generated configuration class through the standard `IOptions<T>` pattern, so consumers bind to configuration using `IOptions<T>`, `IOptionsSnapshot<T>`, or `IOptionsMonitor<T>` rather than a Clavus-specific accessor.

#### Scenario: Generated configuration class is resolvable via IOptions

- **WHEN** a generated `IConfigurationPart` is applied during host application startup
- **THEN** the corresponding generated configuration class SHALL be resolvable from the dependency injection container as `IOptions<T>` for that generated type

#### Scenario: Generated configuration class is resolvable via IOptionsMonitor

- **WHEN** a generated `IConfigurationPart` is applied during host application startup
- **THEN** the corresponding generated configuration class SHALL also be resolvable as `IOptionsMonitor<T>` for that generated type

### Requirement: Configuration reloading

The system SHALL support reloading configuration values when the underlying configuration file changes, surfacing updated values through `IOptionsMonitor<T>` change notifications and/or `IOptionsSnapshot<T>` on next resolution, consistent with standard `Microsoft.Extensions.Configuration` reload-on-change behavior.

#### Scenario: Change to underlying file updates IOptionsMonitor value

- **WHEN** the underlying configuration file for a generated configuration class is modified on disk after application startup
- **THEN** `IOptionsMonitor<T>` for that generated type SHALL reflect the updated values and SHALL invoke any registered change callbacks

#### Scenario: Reload works across supported formats

- **WHEN** a configuration file in JSON, YAML, or TOML format is modified on disk after application startup
- **THEN** the reload behavior SHALL apply consistently regardless of which of the three supported formats the file uses
