using System;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions
{
    internal static class Diagnostics
    {
        public static DiagnosticDescriptor ConventionHasToManyConstructors { get; } = new DiagnosticDescriptor(
            "RSG0001",
            "Convention has to Many Constructors",
            "Conventions only allow one (or the default) constructor",
            "RocketSurgeonsGuild",
            DiagnosticSeverity.Error,
            true
        );

        public static DiagnosticDescriptor ConventionCannotBeGeneric { get; } = new DiagnosticDescriptor(
            "RSG0002",
            "Convention cannot be generic",
            "Conventions cannot be generic types",
            "RocketSurgeonsGuild",
            DiagnosticSeverity.Error,
            true
        );
    }
}