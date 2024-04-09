using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal static class DataHelpers
{
    public static void HandleInvocationExpressionSyntax(
        SourceProductionContext context,
        SemanticModel semanticModel,
        ExpressionSyntax methodCallExpression,
        ExpressionSyntax selectorExpression,
        List<IAssemblyDescriptor> assemblies,
        List<ITypeFilterDescriptor> typeFilters,
        INamedTypeSymbol objectType,
        ref ClassFilter classFilter,
        CancellationToken cancellationToken
    )
    {
        if (methodCallExpression is not InvocationExpressionSyntax expression)
        {
            if (methodCallExpression is not SimpleLambdaExpressionSyntax simpleLambdaExpressionSyntax
             || simpleLambdaExpressionSyntax is { ExpressionBody: null }
             || simpleLambdaExpressionSyntax.ExpressionBody is not
                    InvocationExpressionSyntax body)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeAnExpression, methodCallExpression.GetLocation()));
                return;
            }

            expression = body;
        }

        if (expression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax
         || !memberAccessExpressionSyntax.IsKind(SyntaxKind.SimpleMemberAccessExpression))
        {
            return;
        }

        if (memberAccessExpressionSyntax.Expression is InvocationExpressionSyntax childExpression)
        {
            if (cancellationToken.IsCancellationRequested) return;
            HandleInvocationExpressionSyntax(
                context,
                semanticModel,
                childExpression,
                selectorExpression,
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
            if (typeName == "Rocket.Surgery.DependencyInjection.Compiled.ICompiledAssemblySelector")
            {
                var selector = HandleCompiledAssemblySelector(
                    semanticModel,
                    expression,
                    memberAccessExpressionSyntax.Name
                );
                if (selector is { })
                {
                    assemblies.Add(selector);
                }
            }

            if (typeName == "Rocket.Surgery.DependencyInjection.Compiled.ICompiledImplementationTypeSelector")
            {
                if (cancellationToken.IsCancellationRequested) return;
                var selector = HandleCompiledAssemblySelector(
                    semanticModel,
                    expression,
                    memberAccessExpressionSyntax.Name
                );
                if (selector is { })
                {
                    assemblies.Add(selector);
                }
                else
                {
                    classFilter = HandleCompiledImplementationTypeSelector(expression, memberAccessExpressionSyntax.Name);
                }
            }

            if (typeName == "Rocket.Surgery.DependencyInjection.Compiled.ICompiledImplementationTypeFilter")
            {
                if (cancellationToken.IsCancellationRequested) return;
                typeFilters.AddRange(
                    HandleCompiledImplementationTypeFilter(context, semanticModel, expression, memberAccessExpressionSyntax.Name, objectType)
                );
            }

            if (typeName == "Rocket.Surgery.DependencyInjection.Compiled.ICompiledServiceTypeSelector")
            {
                if (cancellationToken.IsCancellationRequested) return;
            }
        }

        foreach (var argument in expression.ArgumentList.Arguments.Where(argument => argument.Expression is SimpleLambdaExpressionSyntax))
        {
            if (cancellationToken.IsCancellationRequested) return;
            HandleInvocationExpressionSyntax(
                context,
                semanticModel,
                argument.Expression,
                selectorExpression,
                assemblies,
                typeFilters,
                objectType,
                ref classFilter,
                cancellationToken
            );
        }
    }

    private static IAssemblyDescriptor? HandleCompiledAssemblySelector(
        SemanticModel semanticModel,
        InvocationExpressionSyntax expression,
        NameSyntax name
    )
    {
        if (name.ToFullString() == "FromAssembly")
            return new AssemblyDescriptor(semanticModel.Compilation.Assembly);
        if (name.ToFullString() == "FromAssemblies")
            return new AllAssemblyDescriptor();

        var typeSyntax = ExtractSyntaxFromMethod(expression, name);
        if (typeSyntax == null)
            return null;

        var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, typeSyntax).Type;
        if (typeInfo == null)
            return null;
        return typeInfo switch
               {
                   INamedTypeSymbol nts when name.ToFullString().StartsWith("FromAssemblyDependenciesOf", StringComparison.Ordinal) =>
                       new CompiledAssemblyDependenciesDescriptor(nts.ContainingAssembly),
                   INamedTypeSymbol nts when name.ToFullString().StartsWith("FromAssemblyOf", StringComparison.Ordinal) =>
                       new CompiledAssemblyDescriptor(nts.ContainingAssembly),
                   _ => null
               };
    }

    private static ClassFilter HandleCompiledImplementationTypeSelector(
        InvocationExpressionSyntax expression,
        NameSyntax name
    )
    {
        if (name.ToFullString() == "AddClasses" && expression.ArgumentList.Arguments.Count is >= 0 and <= 2)
        {
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (argument.Expression is LiteralExpressionSyntax literalExpressionSyntax && literalExpressionSyntax.Token.IsKind(SyntaxKind.TrueKeyword))
                {
                    return ClassFilter.PublicOnly;
                }
            }
        }

        return ClassFilter.All;
    }

    private static IEnumerable<ITypeFilterDescriptor> HandleCompiledImplementationTypeFilter(
        SourceProductionContext context,
        SemanticModel semanticModel,
        InvocationExpressionSyntax expression,
        NameSyntax name,
        INamedTypeSymbol objectType
    )
    {
        if (name.ToFullString() == "AssignableToAny")
        {
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (argument.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax)
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, typeOfExpressionSyntax.Type).Type;
                    switch (typeInfo)
                    {
                        case INamedTypeSymbol nts:
                            yield return new CompiledAssignableToAnyTypeFilterDescriptor(nts);
                            continue;

                        default:
                            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledSymbol, name.GetLocation()));
                            yield break;
                    }
                }

                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeTypeOf, argument.Expression.GetLocation()));
                yield return new CompiledAbortTypeFilterDescriptor(objectType);
                yield break;
            }

            yield break;
        }

        if (name is GenericNameSyntax genericNameSyntax && genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
        {
            if (genericNameSyntax.Identifier.ToFullString() == "AssignableTo")
            {
                var type = ExtractSyntaxFromMethod(expression, name);
                if (type is { })
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, type).Type;
                    switch (typeInfo)
                    {
                        case INamedTypeSymbol nts:
                            yield return new CompiledAssignableToTypeFilterDescriptor(nts);
                            yield break;

                        default:
                            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledSymbol, name.GetLocation()));
                            yield break;
                    }
                }

                yield break;
            }

            if (genericNameSyntax.Identifier.ToFullString() == "WithAttribute")
            {
                var type = ExtractSyntaxFromMethod(expression, name);
                if (type is { })
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, type).Type;
                    switch (typeInfo)
                    {
                        case INamedTypeSymbol nts:
                            yield return new CompiledWithAttributeFilterDescriptor(nts);
                            yield break;

                        default:
                            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledSymbol, name.GetLocation()));
                            yield break;
                    }
                }

                yield break;
            }

            if (genericNameSyntax.Identifier.ToFullString() == "WithoutAttribute")
            {
                var type = ExtractSyntaxFromMethod(expression, name);
                if (type is { })
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, type).Type;
                    switch (typeInfo)
                    {
                        case INamedTypeSymbol nts:
                            yield return new CompiledWithoutAttributeFilterDescriptor(nts);
                            yield break;

                        default:
                            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledSymbol, name.GetLocation()));
                            yield break;
                    }
                }

                yield break;
            }

            NamespaceFilter? filter = null;
            if (genericNameSyntax.Identifier.ToFullString() == "InExactNamespaceOf")
            {
                filter = NamespaceFilter.Exact;
            }

            if (genericNameSyntax.Identifier.ToFullString() == "InNamespaceOf")
            {
                filter = NamespaceFilter.In;
            }

            if (genericNameSyntax.Identifier.ToFullString() == "NotInNamespaceOf")
            {
                filter = NamespaceFilter.NotIn;
            }

            if (filter is { })
            {
                var symbol = ModelExtensions.GetTypeInfo(semanticModel, genericNameSyntax.TypeArgumentList.Arguments[0]).Type!;
                yield return new NamespaceFilterDescriptor(filter.Value, ImmutableHashSet.Create(symbol.ContainingNamespace.ToDisplayString()));
            }

            yield break;
        }

        if (name is SimpleNameSyntax simpleNameSyntax)
        {
            if (simpleNameSyntax.ToFullString() == "AssignableTo")
            {
                var type = ExtractSyntaxFromMethod(expression, name);
                if (type is { })
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, type).Type;
                    switch (typeInfo)
                    {
                        case INamedTypeSymbol nts:
                            yield return new CompiledAssignableToTypeFilterDescriptor(nts);
                            yield break;
                        default:
                            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledSymbol, name.GetLocation()));
                            yield break;
                    }
                }

                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MustBeTypeOf, simpleNameSyntax.Identifier.GetLocation()));
                yield return new CompiledAbortTypeFilterDescriptor(objectType);
                yield break;
            }

            if (simpleNameSyntax.ToFullString() == "WithAttribute")
            {
                var type = ExtractSyntaxFromMethod(expression, name);
                if (type is { })
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, type).Type;
                    switch (typeInfo)
                    {
                        case INamedTypeSymbol nts:
                            yield return new CompiledWithAttributeFilterDescriptor(nts);
                            yield break;

                        default:
                            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledSymbol, name.GetLocation()));
                            yield break;
                    }
                }

                yield break;
            }

            if (simpleNameSyntax.ToFullString() == "WithoutAttribute")
            {
                var type = ExtractSyntaxFromMethod(expression, name);
                if (type is { })
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, type).Type;
                    switch (typeInfo)
                    {
                        case INamedTypeSymbol nts:
                            yield return new CompiledWithoutAttributeFilterDescriptor(nts);
                            yield break;

                        default:
                            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledSymbol, name.GetLocation()));
                            yield break;
                    }
                }

                yield break;
            }

            {
                NamespaceFilter? filter = null;
                if (simpleNameSyntax.ToFullString() == "InExactNamespaces" || simpleNameSyntax.Identifier.ToFullString() == "InExactNamespaceOf")
                {
                    filter = NamespaceFilter.Exact;
                }

                if (simpleNameSyntax.ToFullString() == "InNamespaces" || simpleNameSyntax.Identifier.ToFullString() == "InNamespaceOf")
                {
                    filter = NamespaceFilter.In;
                }

                if (simpleNameSyntax.ToFullString() == "NotInNamespaces" || simpleNameSyntax.Identifier.ToFullString() == "NotInNamespaceOf")
                {
                    filter = NamespaceFilter.NotIn;
                }

                if (filter is { })
                {
                    var namespaces = expression
                                    .ArgumentList.Arguments
                                    .Select(
                                         argument =>
                                         {
                                             switch (argument.Expression)
                                             {
                                                 case LiteralExpressionSyntax literalExpressionSyntax
                                                     when literalExpressionSyntax.Token.IsKind(SyntaxKind.StringLiteralToken):
                                                     return literalExpressionSyntax.Token.ValueText;
                                                 case InvocationExpressionSyntax
                                                     {
                                                         Expression: IdentifierNameSyntax { Identifier: { Text: "nameof" } }
                                                     } invocationExpressionSyntax
                                                     when invocationExpressionSyntax.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax
                                                         identifierNameSyntax:
                                                     return identifierNameSyntax.Identifier.Text;
                                                 case TypeOfExpressionSyntax typeOfExpressionSyntax:
                                                     {
                                                         var symbol = ModelExtensions.GetTypeInfo(semanticModel, typeOfExpressionSyntax.Type).Type!;
                                                         return symbol.ContainingNamespace.ToDisplayString();
                                                     }
                                                 default:
                                                     context.ReportDiagnostic(
                                                         Diagnostic.Create(Diagnostics.NamespaceMustBeAString, argument.GetLocation())
                                                     );
                                                     return null!;
                                             }
                                         }
                                     )
                                    .Where(z => !string.IsNullOrWhiteSpace(z))
                                    .ToImmutableHashSet();

                    yield return new NamespaceFilterDescriptor(filter.Value, namespaces);
                }
            }


            {
                TextDirectionFilter? filter = null;
                if (simpleNameSyntax.ToString() is "Suffix" or "Postfix" or "EndsWith")
                {
                    filter = TextDirectionFilter.EndsWith;
                }

                if (simpleNameSyntax.ToFullString() is "Affix" or "Prefix" or "StartsWith")
                {
                    filter = TextDirectionFilter.StartsWith;
                }

                if (simpleNameSyntax.ToFullString() is "Contains" or "Includes")
                {
                    filter = TextDirectionFilter.Contains;
                }

                if (filter is { })
                {
                    var stringValues = expression
                                      .ArgumentList.Arguments
                                      .Select(
                                           argument =>
                                           {
                                               switch (argument.Expression)
                                               {
                                                   case LiteralExpressionSyntax literalExpressionSyntax
                                                       when literalExpressionSyntax.Token.IsKind(SyntaxKind.StringLiteralToken):
                                                       return literalExpressionSyntax.Token.ValueText;
                                                   case InvocationExpressionSyntax
                                                       {
                                                           Expression: IdentifierNameSyntax { Identifier: { Text: "nameof" } }
                                                       } invocationExpressionSyntax
                                                       when invocationExpressionSyntax.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax
                                                           identifierNameSyntax:
                                                       return identifierNameSyntax.Identifier.Text;
                                                   default:
                                                       context.ReportDiagnostic(
                                                           Diagnostic.Create(Diagnostics.NamespaceMustBeAString, argument.GetLocation())
                                                       );
                                                       return null!;
                                               }
                                           }
                                       )
                                      .Where(z => !string.IsNullOrWhiteSpace(z))
                                      .ToImmutableHashSet();

                    yield return new NameFilterDescriptor(filter.Value, stringValues);
                }
            }
        }
    }

    public static TypeSyntax? ExtractSyntaxFromMethod(
        InvocationExpressionSyntax expression,
        NameSyntax name
    )
    {
        if (name is GenericNameSyntax genericNameSyntax)
        {
            if (genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
            {
                return genericNameSyntax.TypeArgumentList.Arguments[0];
            }
        }

        if (name is SimpleNameSyntax)
        {
            if (expression.ArgumentList.Arguments.Count == 1 && expression.ArgumentList.Arguments[0].Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                return typeOfExpression.Type;
            }
        }

        return null;
    }
}
