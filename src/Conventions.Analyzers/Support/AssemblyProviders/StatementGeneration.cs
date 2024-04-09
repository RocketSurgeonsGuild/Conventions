using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal static class StatementGeneration
{
    private static readonly MemberAccessExpressionSyntax Describe = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("ServiceDescriptor"),
        IdentifierName("Describe")
    );

    public static InvocationExpressionSyntax GenerateServiceFactory(
        Compilation compilation,
        NameSyntax strategyName,
        NameSyntax serviceCollectionName,
        INamedTypeSymbol serviceType,
        INamedTypeSymbol implementationType
    )
    {
        var serviceTypeExpression = GetTypeOfExpression(compilation, serviceType, implementationType);
        var isAccessible = compilation.IsSymbolAccessibleWithin(implementationType, compilation.Assembly);

        if (isAccessible)
        {
            var implementationTypeExpression = SimpleLambdaExpression(Parameter(Identifier("_")))
               .WithExpressionBody(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("_"),
                            GenericName("GetRequiredService")
                               .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            IdentifierName(Helpers.GetGenericDisplayName(implementationType))
                                        )
                                    )
                                )
                        )
                    )
                );

            return GenerateServiceType(strategyName, serviceCollectionName, serviceTypeExpression, implementationTypeExpression);
        }
        else
        {
            var implementationTypeExpression = SimpleLambdaExpression(Parameter(Identifier("_")))
               .WithExpressionBody(
                    BinaryExpression(
                        SyntaxKind.AsExpression,
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("_"),
                                    IdentifierName("GetRequiredService")
                                )
                            )
                           .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(GetTypeOfExpression(compilation, implementationType, serviceType))
                                    )
                                )
                            ),
                        IdentifierName(Helpers.GetGenericDisplayName(serviceType))
                    )
                );
            return GenerateServiceType(strategyName, serviceCollectionName, serviceTypeExpression, implementationTypeExpression);
        }
    }

    public static InvocationExpressionSyntax GenerateServiceType(
        Compilation compilation,
        NameSyntax strategyName,
        NameSyntax serviceCollectionName,
        INamedTypeSymbol serviceType,
        INamedTypeSymbol implementationType
    )
    {
        var serviceTypeExpression = GetTypeOfExpression(compilation, serviceType, implementationType);
        var implementationTypeExpression = GetTypeOfExpression(compilation, implementationType, serviceType);
        return GenerateServiceType(strategyName, serviceCollectionName, serviceTypeExpression, implementationTypeExpression);
    }

    private static InvocationExpressionSyntax GenerateServiceType(
        NameSyntax strategyName,
        NameSyntax serviceCollectionName,
        ExpressionSyntax serviceTypeExpression,
        ExpressionSyntax implementationTypeExpression
    )
    {
        return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    strategyName,
                    IdentifierName("Apply")
                )
            )
           .WithArgumentList(
                ArgumentList(
                    SeparatedList(
                        new[]
                        {
                            Argument(serviceCollectionName),
                            Argument(
                                InvocationExpression(Describe)
                                   .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList(
                                                new[]
                                                {
                                                    Argument(serviceTypeExpression),
                                                    Argument(implementationTypeExpression)
                                                }
                                            )
                                        )
                                    )
                            )
                        }
                    )
                )
            );
    }

    public static IEnumerable<MemberDeclarationSyntax> AssemblyDeclaration(IAssemblySymbol symbol)
    {
        var name = AssemblyVariableName(symbol);
        var assemblyName = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(symbol.Identity.GetDisplayName(true)));

        yield return FieldDeclaration(
                VariableDeclaration(IdentifierName("AssemblyName"))
                   .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(Identifier($"_{name}"))
                        )
                    )
            )
           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)));
        yield return PropertyDeclaration(IdentifierName("AssemblyName"), Identifier(name))
                    .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
                    .WithExpressionBody(
                         ArrowExpressionClause(
                             AssignmentExpression(
                                 SyntaxKind.CoalesceAssignmentExpression,
                                 IdentifierName(Identifier($"_{name}")),
                                 ObjectCreationExpression(IdentifierName("AssemblyName"))
                                    .WithArgumentList(
                                         ArgumentList(SingletonSeparatedList(Argument(assemblyName)))
                                     )
                             )
                         )
                     )
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    public static ExpressionSyntax? GetAssemblyExpression(Compilation compilation, IAssemblySymbol assembly)
    {
        var types = TypeSymbolVisitor.GetTypes(
            compilation,
            new CompiledTypeFilter(ClassFilter.PublicOnly, ImmutableArray<ITypeFilterDescriptor>.Empty),
            new CompiledAssemblyFilter(ImmutableArray.Create<IAssemblyDescriptor>(new AssemblyDescriptor(assembly)))
        );
        var keyholdType = types.FirstOrDefault(z => !z.IsUnboundGenericType);
        if (keyholdType == null)
        {
            return null;
        }

        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            GetTypeOfExpression(compilation, keyholdType),
            IdentifierName("Assembly")
        );
    }

    public static ExpressionSyntax GetTypeOfExpression(Compilation compilation, INamedTypeSymbol type)
    {
        if (compilation.IsSymbolAccessibleWithin(type, compilation.Assembly))
        {
            return TypeOfExpression(ParseTypeName(Helpers.GetGenericDisplayName(type)));
        }

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
                                    .Select(t => Argument(GetTypeOfExpression(compilation, ( t as INamedTypeSymbol )!, null)))
                            )
                        )
                    );
            }
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
                var baseType = Helpers.GetBaseTypes(compilation, type).FirstOrDefault(z => z.IsGenericType && compilation.HasImplicitConversion(z, type));
                if (baseType == null)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    baseType = type.AllInterfaces.FirstOrDefault(z => z.IsGenericType && compilation.HasImplicitConversion(z, type));
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
