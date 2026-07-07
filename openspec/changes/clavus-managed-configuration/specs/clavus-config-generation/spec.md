## ADDED Requirements

### Requirement: Strongly-typed configuration class generation

The system SHALL generate a strongly-typed configuration class inside the owning library's own namespace for each discovered configuration file, so the class is usable by the library itself and does not depend on a downstream consumer's namespace.

#### Scenario: Configuration class is generated in the owning library

- **WHEN** a library project with a discovered configuration file is compiled
- **THEN** a configuration class corresponding to that file SHALL be generated within the owning library's own root namespace and assembly

#### Scenario: Configuration class shape matches the config file structure

- **WHEN** a discovered configuration file defines a nested object with multiple properties
- **THEN** the generated configuration class SHALL expose a corresponding property structure that can be bound from that configuration section

### Requirement: Type inference for date, time, and duration values

The system SHALL infer `DateOnly`, `TimeOnly`, `DateTimeOffset`, or `TimeSpan` property types from a configuration value's string shape, applying date-only, time-only, duration, and combined date+time detection in a fixed, unambiguous precedence order.

#### Scenario: Date-only value becomes DateOnly

- **WHEN** a configuration value is `"2024-01-15"`
- **THEN** the corresponding generated property SHALL have type `DateOnly`

#### Scenario: Time-only value becomes TimeOnly

- **WHEN** a configuration value is `"14:30:00"` and does not match the duration shape
- **THEN** the corresponding generated property SHALL have type `TimeOnly`

#### Scenario: Combined date and time value becomes DateTimeOffset

- **WHEN** a configuration value is `"2024-01-15T14:30:00Z"`
- **THEN** the corresponding generated property SHALL have type `DateTimeOffset`

#### Scenario: Duration-shaped value becomes TimeSpan

- **WHEN** a configuration value is `"1.00:00:00"`
- **THEN** the corresponding generated property SHALL have type `TimeSpan`

#### Scenario: Non-matching value falls back to a primitive type

- **WHEN** a configuration value does not match any date, time, or duration shape
- **THEN** the corresponding generated property SHALL have an inferred primitive type (`bool`, numeric, or `string`) based on the value's shape

### Requirement: Opt-in NodaTime type mode

The system SHALL emit NodaTime equivalent types (`LocalDate`, `LocalTime`, `OffsetDateTime`, `Duration`) in place of the corresponding BCL types when an MSBuild property enabling NodaTime type generation is set for the project AND the project references `NodaTime`. When the property is set without a `NodaTime` reference present, the system SHALL report a compile-time diagnostic instead of silently generating BCL types.

#### Scenario: NodaTime types are generated when enabled and referenced

- **WHEN** a project sets the NodaTime type generation MSBuild property to enabled and references the `NodaTime` package
- **THEN** date/time/duration-shaped configuration values SHALL generate properties using the corresponding NodaTime types instead of BCL types

#### Scenario: Diagnostic is reported when NodaTime is enabled without a reference

- **WHEN** a project sets the NodaTime type generation MSBuild property to enabled but does not reference the `NodaTime` package
- **THEN** the build SHALL report a compile-time diagnostic and SHALL NOT silently fall back to BCL types for that project

#### Scenario: BCL types are used by default

- **WHEN** a project does not set the NodaTime type generation MSBuild property
- **THEN** date/time/duration-shaped configuration values SHALL generate properties using BCL types (`DateOnly`, `TimeOnly`, `DateTimeOffset`, `TimeSpan`)

### Requirement: Generated IConfigurationPart per configuration file

The system SHALL emit a generated `IConfigurationPart` implementation for each discovered configuration file, responsible for registering that file's configuration source and binding its generated configuration class.

#### Scenario: IConfigurationPart is generated for a discovered file

- **WHEN** a library project has a discovered configuration file
- **THEN** the build SHALL generate an `IConfigurationPart` implementation associated with that configuration file

### Requirement: Export generator always includes generated configuration parts

The system SHALL ensure that generated `IConfigurationPart` implementations are always included in a library's export set, independent of whether they carry a `[Convention]` attribute, since they are generator-authored and not user-decorated.

#### Scenario: Generated configuration part is exported without a Convention attribute

- **WHEN** a library generates an `IConfigurationPart` for a discovered configuration file and the generated type carries no `[Convention]` attribute
- **THEN** the library's export set SHALL include that generated `IConfigurationPart`

### Requirement: Transitive availability of dependency configuration

The system SHALL make a library's own referenced libraries' exported `IConfigurationPart`s available to that library's dependents, without requiring the intermediate library to redeclare or re-export configuration it merely depends on.

#### Scenario: Configuration flows through two levels of project references

- **WHEN** library A contributes an `IConfigurationPart`, library B references library A, and application C references library B
- **THEN** application C's export/import set SHALL include library A's `IConfigurationPart` without library B declaring any configuration of its own

#### Scenario: Intermediate library need not redeclare dependency configuration

- **WHEN** library B references library A (which contributes configuration) but declares no configuration files itself
- **THEN** library B's own generated export set SHALL still surface library A's `IConfigurationPart` to library B's consumers
