using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal static class DataHelpers
{
    public static void HandleInvocationExpressionSyntax(
        SourceProductionContext context,
        SemanticModel semanticModel,
        ExpressionSyntax selectorExpression,
        List<IAssemblyDescriptor> assemblies,
        List<ITypeFilterDescriptor> typeFilters,
        INamedTypeSymbol objectType,
        ref ClassFilter classFilter,
        CancellationToken cancellationToken
    )
    {
        if (selectorExpression is not InvocationExpressionSyntax expression)
        {
            if (selectorExpression is not SimpleLambdaExpressionSyntax simpleLambdaExpressionSyntax
             || simpleLambdaExpressionSyntax is { ExpressionBody: null, }
             || simpleLambdaExpressionSyntax.ExpressionBody is not
                    InvocationExpressionSyntax body)
                //                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeAnExpression, selectorExpression.GetLocation()));
                return;

            expression = body;
        }

        if (expression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax
         || !memberAccessExpressionSyntax.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            return;

        if (memberAccessExpressionSyntax.Expression is InvocationExpressionSyntax childExpression)
        {
            if (cancellationToken.IsCancellationRequested) return;
            HandleInvocationExpressionSyntax(
                context,
                semanticModel,
                childExpression,
                assemblies,
                typeFilters,
                objectType,
                ref classFilter,
                cancellationToken
            );
        }

        var type = ModelExtensions.GetTypeInfo(semanticModel, memberAccessExpressionSyntax.Expression);
        if (type.Type is { })
        {
            var typeName = type.Type.ToDisplayString();
            if (typeName is "Rocket.Surgery.Conventions.Reflection.IAssemblyProviderAssemblySelector"
                         or "Rocket.Surgery.Conventions.Reflection.ITypeProviderAssemblySelector")
            {
                if (cancellationToken.IsCancellationRequested) return;
                var selector = HandleCompiledAssemblySelector(semanticModel, expression, memberAccessExpressionSyntax.Name);
                if (selector is { }) assemblies.Add(selector);
            }

            if (typeName == "Rocket.Surgery.Conventions.Reflection.ITypeSelector")
            {
                if (cancellationToken.IsCancellationRequested) return;
                var selector = HandleCompiledAssemblySelector(semanticModel, expression, memberAccessExpressionSyntax.Name);
                if (selector is { })
                    assemblies.Add(selector);
                else
                    classFilter = HandleCompiledImplementationTypeSelector(expression, memberAccessExpressionSyntax.Name);
            }

            if (typeName == "Rocket.Surgery.Conventions.Reflection.ITypeFilter")
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (HandleCompiledImplementationTypeFilter(context, semanticModel, expression, memberAccessExpressionSyntax.Name) is { } filter)
                    typeFilters.Add(filter);
            }
        }

        foreach (var argument in expression.ArgumentList.Arguments.Where(argument => argument.Expression is SimpleLambdaExpressionSyntax))
        {
            if (cancellationToken.IsCancellationRequested) return;
            HandleInvocationExpressionSyntax(
                context,
                semanticModel,
                argument.Expression,
                assemblies,
                typeFilters,
                objectType,
                ref classFilter,
                cancellationToken
            );
        }
    }

    public static TypeSyntax? ExtractSyntaxFromMethod(
        ExpressionSyntax expression,
        NameSyntax name
    )
    {
        return ( name, expression ) switch
               {
                   (GenericNameSyntax { TypeArgumentList.Arguments: [var syntax,], }, _) => syntax,
                   (SimpleNameSyntax, InvocationExpressionSyntax
                   {
                       ArgumentList.Arguments: [{ Expression: TypeOfExpressionSyntax typeOfExpression, }, ..,],
                   }) => typeOfExpression.Type,
                   // lambda's are methods to be handled.
                   (_, TypeOfExpressionSyntax typeOfExpression)                                                                      => typeOfExpression.Type,
                   (_, InvocationExpressionSyntax { ArgumentList.Arguments: [{ Expression: LambdaExpressionSyntax, }, ..,], })       => null,
                   (_, InvocationExpressionSyntax { ArgumentList.Arguments: [{ Expression: LiteralExpressionSyntax, }, ..,], })      => null,
                   (_, InvocationExpressionSyntax { ArgumentList.Arguments: [{ Expression: MemberAccessExpressionSyntax, }, ..,], }) => null,
//             _ => null,
                   _ => throw new NotSupportedException(
                       name.ToFullString()
                     + " "
                     + string.Join(", ", expression.GetType().FullName + ": " + expression.ToFullString())
                   ),
               };
    }

    private static IAssemblyDescriptor? HandleCompiledAssemblySelector(
        SemanticModel semanticModel,
        InvocationExpressionSyntax expression,
        SimpleNameSyntax name
    )
    {
        if (name.ToFullString() == "FromAssembly")
            return new AssemblyDescriptor(semanticModel.Compilation.Assembly);
        if (name.ToFullString() == "FromAssemblies")
            return new AllAssemblyDescriptor();
        if (name.ToFullString() == "IncludeSystemAssemblies")
            return new IncludeSystemAssembliesDescriptor();

        var typeSyntax = ExtractSyntaxFromMethod(expression, name);
        if (typeSyntax == null)
            return null;

        var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, typeSyntax).Type;
        if (typeInfo is null)
            return null;
        return typeInfo switch
               {
                   INamedTypeSymbol nts when name is { Identifier.Text: "FromAssemblyDependenciesOf", } =>
                       new AssemblyDependenciesDescriptor(nts.ContainingAssembly),
                   INamedTypeSymbol namedType when name is { Identifier.Text: "FromAssemblyOf" or "NotFromAssemblyOf", } =>
                       name.Identifier.Text.StartsWith("Not")
                           ? new NotAssemblyDescriptor(namedType.ContainingAssembly)
                           : new AssemblyDescriptor(namedType.ContainingAssembly),
                   _ => null,
               };
    }

    private static ClassFilter HandleCompiledImplementationTypeSelector(
        InvocationExpressionSyntax expression,
        NameSyntax name
    )
    {
        if (name.ToFullString() == "GetTypes" && expression.ArgumentList.Arguments.Count is >= 0 and <= 2)
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (argument.Expression is LiteralExpressionSyntax literalExpressionSyntax && literalExpressionSyntax.Token.IsKind(SyntaxKind.TrueKeyword))
                    return ClassFilter.PublicOnly;
            }

        return ClassFilter.All;
    }

    private static ITypeFilterDescriptor? HandleCompiledImplementationTypeFilter(
        SourceProductionContext context,
        SemanticModel semanticModel,
        InvocationExpressionSyntax expression,
        SimpleNameSyntax name
    )
    {
        return ( name, GetSyntaxTypeInfo(semanticModel, expression, name) ) switch
               {
                   ({ Identifier.Text: "AssignableToAny" or "NotAssignableToAny", }, _) =>
                       createAssignableToAnyTypeFilterDescriptor(name, expression, semanticModel),
                   ({ Identifier.Text: "AssignableTo" or "NotAssignableTo", }, { } namedType) =>
                       createAssignableToTypeFilterDescriptor(name, namedType),
                   ({ Identifier.Text: "WithAttribute" or "WithoutAttribute", }, { } namedType) =>
                       createWithAttributeFilterDescriptor(name, namedType),
                   ({ Identifier.Text: "WithAttribute" or "WithoutAttribute", }, _) =>
                       createWithAttributeStringFilterDescriptor(context, name, expression, semanticModel),
                   ({ Identifier.Text: "InExactNamespaceOf" or "InNamespaceOf" or "NotInNamespaceOf", }, _) =>
                       createNamespaceTypeFilterDescriptor(context, name, expression, semanticModel),
                   ({ Identifier.Text: "InExactNamespaces" or "InNamespaces" or "NotInNamespaces", }, _) =>
                       createNamespaceStringFilterDescriptor(context, name, expression, semanticModel),
                   ({ Identifier.Text: "EndsWith" or "StartsWith" or "Contains", }, _) =>
                       createNameFilterDescriptor(context, name, expression),
                   ({ Identifier.Text: "KindOf" or "NotKindOf", }, _) =>
                       createTypeKindFilterDescriptor(context, name, expression),
                   ({ Identifier.Text: "InfoOf" or "NotInfoOf", }, _) =>
                       createTypeInfoFilterDescriptor(context, name, expression),
                   _ => throw new NotSupportedException($"Not supported type filter. Method: {name.ToFullString()}  {expression.ToFullString()} method."),
               };

        static ITypeFilterDescriptor createAssignableToTypeFilterDescriptor(SimpleNameSyntax name, INamedTypeSymbol namedType)
        {
            return name.Identifier.Text.StartsWith("Not")
                ? new NotAssignableToTypeFilterDescriptor(namedType)
                : new AssignableToTypeFilterDescriptor(namedType);
        }

        static ITypeFilterDescriptor createWithAttributeFilterDescriptor(SimpleNameSyntax name, INamedTypeSymbol namedType)
        {
            return name.Identifier.Text.StartsWith("Without") ? new WithoutAttributeFilterDescriptor(namedType) : new WithAttributeFilterDescriptor(namedType);
        }

        static ITypeFilterDescriptor createAssignableToAnyTypeFilterDescriptor(
            SimpleNameSyntax name,
            InvocationExpressionSyntax expression,
            SemanticModel semanticModel
        )
        {
            var arguments = expression
                           .ArgumentList.Arguments.Select(z => GetSyntaxTypeInfo(semanticModel, z.Expression, name))
                           .OfType<INamedTypeSymbol>()
                           .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

            return name.Identifier.Text.StartsWith("Not")
                ? new NotAssignableToAnyTypeFilterDescriptor(arguments)
                : new AssignableToAnyTypeFilterDescriptor(arguments);
        }

        static NameFilterDescriptor createNameFilterDescriptor(
            SourceProductionContext context,
            SimpleNameSyntax name,
            InvocationExpressionSyntax expression
        )
        {
            var filter = name.Identifier.Text switch
                         {
                             "EndsWith"   => TextDirectionFilter.EndsWith,
                             "StartsWith" => TextDirectionFilter.StartsWith,
                             "Contains"   => TextDirectionFilter.Contains,
                             _ => throw new NotSupportedException(
                                 $"Not supported name filter. Method: {name.ToFullString()}  {expression.ToFullString()} method."
                             ),
                         };
            var stringValues = ImmutableHashSet.CreateBuilder<string>();
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (getStringValue(argument) is not { Length: > 0, } item)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeAString, argument.GetLocation()));
                    continue;
                }

                stringValues.Add(item);
            }

            return new(filter, stringValues.ToImmutable());
        }

        static NamespaceFilterDescriptor createNamespaceTypeFilterDescriptor(
            SourceProductionContext context,
            SimpleNameSyntax name,
            InvocationExpressionSyntax expression,
            SemanticModel semanticModel
        )
        {
            var filter = name.Identifier.Text switch
                         {
                             "InExactNamespaceOf" => NamespaceFilter.Exact,
                             "InNamespaceOf"      => NamespaceFilter.In,
                             "NotInNamespaceOf"   => NamespaceFilter.NotIn,
                             _ => throw new NotSupportedException(
                                 $"Not supported namespace filter. Method: {name.ToFullString()}  {expression.ToFullString()} method."
                             ),
                         };

            var namespaces = ImmutableHashSet.CreateBuilder<string>();
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (argument.Expression is not TypeOfExpressionSyntax typeOfExpressionSyntax
                 || ModelExtensions.GetTypeInfo(semanticModel, typeOfExpressionSyntax.Type).Type is not { } type)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeAnExpression, argument.GetLocation()));
                    continue;
                }

                namespaces.Add(type.ContainingNamespace.ToDisplayString());
            }

            return new(filter, namespaces.ToImmutable());
        }

        static NamespaceFilterDescriptor createNamespaceStringFilterDescriptor(
            SourceProductionContext context,
            SimpleNameSyntax name,
            InvocationExpressionSyntax expression,
            SemanticModel semanticModel
        )
        {
            var filter = name.Identifier.Text switch
                         {
                             "InExactNamespaces" => NamespaceFilter.Exact,
                             "InNamespaces"      => NamespaceFilter.In,
                             "NotInNamespaces"   => NamespaceFilter.NotIn,
                             _ => throw new NotSupportedException(
                                 $"Not supported namespace filter. Method: {name.ToFullString()}  {expression.ToFullString()} method."
                             ),
                         };

            var namespaces = ImmutableHashSet.CreateBuilder<string>();
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (argument.Expression is MemberAccessExpressionSyntax
                    {
                        Name.Identifier.Text: "Namespace", Expression: TypeOfExpressionSyntax typeOfExpressionSyntax,
                    }
                 && GetSyntaxTypeInfo(semanticModel, typeOfExpressionSyntax, name) is { } type)
                {
                    namespaces.Add(type.ContainingNamespace.ToDisplayString());
                    continue;
                }

                if (getStringValue(argument) is { Length: > 0, } item)
                {
                    namespaces.Add(item);
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeAString, argument.GetLocation()));
            }

            return new(filter, namespaces.ToImmutable());
        }

        static ITypeFilterDescriptor? createWithAttributeStringFilterDescriptor(
            SourceProductionContext context,
            SimpleNameSyntax name,
            InvocationExpressionSyntax expression,
            SemanticModel semanticModel
        )
        {
            if (expression is not { ArgumentList.Arguments: [var argument,], }) return null;
            if (argument.Expression is MemberAccessExpressionSyntax
                {
                    Name.Identifier.Text: "FullName", Expression: TypeOfExpressionSyntax typeOfExpressionSyntax,
                }
             && GetSyntaxTypeInfo(semanticModel, typeOfExpressionSyntax, name) is { } type)
                return name.Identifier.Text.StartsWith("Without")
                    ? new WithoutAttributeStringFilterDescriptor(Helpers.GetFullMetadataName(type))
                    : new WithAttributeStringFilterDescriptor(Helpers.GetFullMetadataName(type));

            if (getStringValue(argument) is { Length: > 0, } item)
                return name.Identifier.Text.StartsWith("Without")
                    ? new WithoutAttributeStringFilterDescriptor(item)
                    : new WithAttributeStringFilterDescriptor(item);

            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeAString, argument.GetLocation()));
            return null;
        }

        static TypeKindFilterDescriptor createTypeKindFilterDescriptor(
            SourceProductionContext context,
            SimpleNameSyntax name,
            InvocationExpressionSyntax expression
        )
        {
            var include = name.Identifier.Text switch { "KindOf" => true, _ => false, };

            var namespaces = ImmutableHashSet.CreateBuilder<TypeKind>();
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (getStringValue(argument) is not { Length: > 0, } item)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeAString, argument.GetLocation()));
                    continue;
                }

                namespaces.Add(Enum.TryParse<TypeKind>(item, out var result) ? result : TypeKind.Unknown);
            }

            return new(include, namespaces.ToImmutable());
        }

        static TypeInfoFilterDescriptor createTypeInfoFilterDescriptor(
            SourceProductionContext context,
            SimpleNameSyntax name,
            InvocationExpressionSyntax expression
        )
        {
            var include = name.Identifier.Text switch { "InfoOf" => true, _ => false, };

            var namespaces = ImmutableHashSet.CreateBuilder<TypeInfoFilter>();
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (getStringValue(argument) is not { Length: > 0, } item)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeAString, argument.GetLocation()));
                    continue;
                }

                namespaces.Add(Enum.TryParse<TypeInfoFilter>(item, out var result) ? result : TypeInfoFilter.Unknown);
            }

            return new(include, namespaces.ToImmutable());
        }

        static string? getStringValue(ArgumentSyntax argument)
        {
            return argument.Expression switch
                   {
                       LiteralExpressionSyntax { Token.RawKind: (int)SyntaxKind.StringLiteralToken, Token.ValueText: { Length: > 0, } result, } => result,
                       MemberAccessExpressionSyntax { Name.Identifier.Text: var text, }                                                         => text,
                       InvocationExpressionSyntax
                       {
                           Expression: IdentifierNameSyntax { Identifier: { Text: "nameof", }, },
                           ArgumentList.Arguments: [{ Expression: IdentifierNameSyntax { Identifier.Text: { Length: > 0, } text, }, },],
                       } => text,
                       _ => null,
                   };
        }
    }

    private static INamedTypeSymbol? GetSyntaxTypeInfo(
        SemanticModel semanticModel,
        ExpressionSyntax expression,
        NameSyntax name
    )
    {
        return ExtractSyntaxFromMethod(expression, name) is not { } type
         || ModelExtensions.GetTypeInfo(semanticModel, type).Type is not INamedTypeSymbol { Kind: not SymbolKind.ErrorType, } nts
                ? null
                : nts;
    }
}