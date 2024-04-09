using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal interface ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct NamespaceFilterDescriptor(NamespaceFilter Filter, ImmutableHashSet<string> Namespaces) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct NameFilterDescriptor(TextDirectionFilter Filter, ImmutableHashSet<string> Names) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct CompiledWithAttributeFilterDescriptor(INamedTypeSymbol Attribute) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct CompiledWithoutAttributeFilterDescriptor(INamedTypeSymbol Attribute) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct CompiledAssignableToTypeFilterDescriptor(INamedTypeSymbol Type) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct CompiledAssignableToAnyTypeFilterDescriptor(INamedTypeSymbol Type) : ITypeFilterDescriptor;

[DebuggerDisplay("{ToString()}")]
internal readonly record struct CompiledAbortTypeFilterDescriptor(INamedTypeSymbol Type) : ITypeFilterDescriptor;
