using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions;

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
}