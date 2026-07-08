using Microsoft.CodeAnalysis;

namespace Clavus;

internal static class Diagnostics
{
    public static DiagnosticDescriptor ConventionHasToManyConstructors { get; } = new(
        "RSG0001",
        "Convention has to Many Constructors",
        "Conventions only allow one (or the default) constructor",
        "RocketSurgeonsGuild",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor ConventionCannotBeGeneric { get; } = new(
        "RSG0002",
        "Convention cannot be generic",
        "Conventions cannot be generic types",
        "RocketSurgeonsGuild",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor MustBeAnExpression { get; } = new(
        "RSG0003",
        "Must be a expression",
        "Methods that will be analyzed statically must be an expression, blocks and variables are not allowed",
        "RocketSurgeonsGuild",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor MustBeTypeOf { get; } = new(
        "RSG0004",
        "Must use typeof",
        "Method must be called with typeof, variables are not allowed",
        "RocketSurgeonsGuild",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor UnhandledSymbol { get; } = new(
        "RSG0005",
        "Symbol could not be handled",
        "The indicated symbol could not be handled correctly",
        "RocketSurgeonsGuild",
        DiagnosticSeverity.Warning,
        true
    );

    public static DiagnosticDescriptor MustBeAString { get; } = new(
        "RSG0006",
        "Value must be a string",
        "The given value must be a constant string",
        "RocketSurgeonsGuild",
        DiagnosticSeverity.Warning,
        true
    );

    /// <summary>
    ///     Reported when the NodaTime configuration type-mode MSBuild property is enabled for a project that does
    ///     not reference the <c>NodaTime</c> assembly. Silent fallback to BCL types would make the generated
    ///     public API shape depend on an easily-missed reference, so this is an error rather than a warning.
    /// </summary>
    public static DiagnosticDescriptor NodaTimeEnabledWithoutReference { get; } = new(
        "CLAVUS_CFG002",
        "NodaTime configuration type-mode enabled without a NodaTime reference",
        "The NodaTime configuration type-mode property is enabled, but this project does not reference the "
      + "NodaTime assembly. Add a reference to NodaTime, or disable the property, to continue",
        "Clavus.Configuration",
        DiagnosticSeverity.Error,
        true
    );
}
