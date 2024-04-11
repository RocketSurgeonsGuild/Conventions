using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal interface ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct NamespaceFilterDescriptor([property: JsonPropertyName("f")] NamespaceFilter Filter, [property: JsonPropertyName("n")] ImmutableHashSet<string> Namespaces) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct NameFilterDescriptor([property: JsonPropertyName("f")] TextDirectionFilter Filter, [property: JsonPropertyName("n")] ImmutableHashSet<string> Names) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct TypeKindFilterDescriptor([property: JsonPropertyName("f")] bool Include, [property: JsonPropertyName("t")] ImmutableHashSet<TypeKind> TypeKinds) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct WithAttributeFilterDescriptor(INamedTypeSymbol Attribute) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct WithoutAttributeFilterDescriptor(INamedTypeSymbol Attribute) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct WithAttributeStringFilterDescriptor([property: JsonPropertyName("a")] string AttributeClassName) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct WithoutAttributeStringFilterDescriptor([property: JsonPropertyName("a")] string AttributeClassName) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct AssignableToTypeFilterDescriptor(INamedTypeSymbol Type) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct NotAssignableToTypeFilterDescriptor(INamedTypeSymbol Type) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct AssignableToAnyTypeFilterDescriptor(ImmutableHashSet<INamedTypeSymbol> Types) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct NotAssignableToAnyTypeFilterDescriptor(ImmutableHashSet<INamedTypeSymbol> Types) : ITypeFilterDescriptor;

internal readonly record struct CompiledAbortTypeFilterDescriptor : ITypeFilterDescriptor;
