using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal static class StatementGeneration
{
    public static ExpressionSyntax? GetAssemblyExpression(Compilation compilation, IAssemblySymbol assembly)
    {
        return FindTypeVisitor.FindType(compilation, assembly) is { } keyholdType
            ? MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                GetTypeOfExpression(compilation, keyholdType),
                IdentifierName("Assembly")
            )
            : null;
    }

    public static ExpressionSyntax GetTypeOfExpression(Compilation compilation, INamedTypeSymbol type)
    {
        if (type.IsGenericType && !type.IsOpenGenericType())
        {
            var result = compilation.IsSymbolAccessibleWithin(type.ConstructUnboundGenericType(), compilation.Assembly);
            if (result)
            {
                var name = ParseTypeName(type.ConstructUnboundGenericType().ToDisplayString());
                if (name is GenericNameSyntax genericNameSyntax)
                {
                    name = genericNameSyntax.WithTypeArgumentList(
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                                genericNameSyntax.TypeArgumentList.Arguments.Select(_ => OmittedTypeArgument()).ToArray()
                            )
                        )
                    );
                }

                return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            TypeOfExpression(name),
                            IdentifierName("MakeGenericType")
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                type.TypeArguments
                                    .Select(t => Argument(GetTypeOfExpression(compilation, ( t as INamedTypeSymbol )!)))
                            )
                        )
                    );
            }
        }

        if (compilation.IsSymbolAccessibleWithin(type, compilation.Assembly))
        {
            return TypeOfExpression(ParseTypeName(Helpers.GetGenericDisplayName(type)));
        }

        return GetPrivateType(compilation, type);
    }

    private static ExpressionSyntax GetTypeOfExpression(
        Compilation compilation,
        INamedTypeSymbol type,
        INamedTypeSymbol relatedType
    )
    {
        if (type.IsUnboundGenericType)
        {
            if (relatedType.IsGenericType && relatedType.Arity == type.Arity)
            {
                type = type.Construct(relatedType.TypeArguments.ToArray());
            }
            else
            {
                var baseType = Helpers
                              .GetBaseTypes(compilation, type)
                              .FirstOrDefault(z => z.IsGenericType && compilation.HasImplicitConversion(z, relatedType));
                if (baseType == null)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    baseType = type.AllInterfaces.FirstOrDefault(z => z.IsGenericType && compilation.HasImplicitConversion(z, relatedType));
                }

                if (baseType != null)
                {
                    type = type.Construct(baseType.TypeArguments.ToArray());
                }
            }
        }

        return GetTypeOfExpression(compilation, type);
    }

    private static InvocationExpressionSyntax GetPrivateType(Compilation compilation, INamedTypeSymbol type)
    {
        var expression = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("context"),
                                IdentifierName("LoadFromAssemblyName")
                            )
                        )
                       .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(IdentifierName(AssemblyVariableName(type.ContainingAssembly)))
                                )
                            )
                        ),
                    IdentifierName("GetType")
                )
            )
           .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(
                            LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal(Helpers.GetFullMetadataName(type))
                            )
                        )
                    )
                )
            );
        if (type.IsGenericType && !type.IsOpenGenericType())
        {
            return InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        expression,
                        IdentifierName("MakeGenericType")
                    )
                )
               .WithArgumentList(
                    ArgumentList(
                        SeparatedList(
                            type.TypeArguments
                                .Select(t => Argument(GetTypeOfExpression(compilation, ( t as INamedTypeSymbol )!, null)))
                        )
                    )
                );
        }

        return expression;
    }

    private static readonly Regex SpecialCharacterRemover = new Regex("[^\\w\\d]", RegexOptions.Compiled);

    public static string AssemblyVariableName(IAssemblySymbol symbol)
    {
        return SpecialCharacterRemover.Replace(symbol.Identity.GetDisplayName(true), "");
    }

    public static bool IsOpenGenericType(this INamedTypeSymbol type)
    {
        return type.IsGenericType && ( type.IsUnboundGenericType || type.TypeArguments.All(z => z.TypeKind == TypeKind.TypeParameter) );
    }
}
