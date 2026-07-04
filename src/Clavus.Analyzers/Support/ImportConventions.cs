using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Clavus.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clavus.Support;

internal static class ImportConventions
{
    public static void HandleConventionImports(
        SourceProductionContext context,
        Request request
    )
    {

        var exportConfig = request.BuildConfig.ExportConfiguration;
        var referenceMethods = request
                              .Compilation.GetClavusReferences()
                              .Select(refer => refer.ToString())
                              .Concat(request.ExportedConventions.Length > 0 ? [$"{( exportConfig.Namespace is { Length: > 0 } ? $"{exportConfig.Namespace}." : "" )}{exportConfig.ClassName}.{exportConfig.MethodName}"] : [])
                              .ToImmutableList();

        var functionBody = referenceMethods.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(referenceMethods);

        var importsClass =
            ClassDeclaration(request.BuildConfig.ImportConfiguration.ClassName)
               .WithAttributeLists(
                    SingletonList(
                        CompilerGeneratedAttributes
                           .WithLeadingTrivia(GetXmlSummary("The class defined for importing Clavus parts into this assembly"))
                    )
                )
               .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)))
               .AddMembers(
                    FieldDeclaration(
                            VariableDeclaration(IdentifierName("ClavusContextBuilderFactory"))
                               .WithVariables(
                                    SingletonSeparatedList(
                                        VariableDeclarator(Identifier(request.BuildConfig.ImportConfiguration.MethodName))
                                           .WithInitializer(EqualsValueClause(IdentifierName("CreateClavusContextBuilder")))
                                    )
                                )
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword))),
                    // The parameterless factory delegate assigned to ImportHelpers.Tectum. It seeds a builder to
                    // resolve any convention dependencies, then materializes the final builder from the imported parts.
                    MethodDeclaration(IdentifierName("ClavusContextBuilder"), Identifier("CreateClavusContextBuilder"))
                       .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
                       .WithParameterList(ParameterList([
                        // new dictionary<object, object>
                        Parameter(Identifier("properties"))
                            .WithType(                            NullableType(GenericName("IDictionary").WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>([PredefinedType(Token(SyntaxKind.ObjectKeyword)), PredefinedType(Token(SyntaxKind.ObjectKeyword))]))))                        )
                            .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                        Parameter(Identifier("categories"))
                            .WithType(                            NullableType(GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>([IdentifierName("ClavusCategory")]))))                        )
                            .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                       ]))
                       .WithExpressionBody(
            ArrowExpressionClause(
                                    InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("ClavusContextBuilder"), IdentifierName("Create")))
                                       .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList(
                                                    [
                                                        Argument(InvocationExpression(IdentifierName("LoadClavusPartsMethod")).WithArgumentList(ArgumentList())),
                                                        Argument(BinaryExpression(
                        SyntaxKind.CoalesceExpression,
                        IdentifierName("properties"),
                        ObjectCreationExpression(
                            GenericName(Identifier("Dictionary"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SeparatedList<TypeSyntax>([PredefinedType(Token(SyntaxKind.ObjectKeyword)), PredefinedType(Token(SyntaxKind.ObjectKeyword))])
                                    )
                                )
                        ).WithArgumentList(ArgumentList())
                    )),
                                                        Argument(BinaryExpression(                        SyntaxKind.CoalesceExpression,                        IdentifierName("categories"),                        CollectionExpression()                    ))
                                                    ]
                                                )
                                            )
                                        )
                                    )
                            )
                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                       .WithLeadingTrivia(GetXmlSummary("Creates the context builder populated with the Clavus parts imported into this assembly")),
                    MethodDeclaration(
                            GenericName(Identifier("IEnumerable"))
                               .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(IdentifierName("IClavusPartMetadata"))
                                    )
                                ),
                            Identifier("LoadClavusPartsMethod")
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
                       .WithParameterList(ParameterList())
                       .WithBody(functionBody)
                       .WithLeadingTrivia(GetXmlSummary("The Clavus parts imported into this assembly"))
                )
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        if (request.BuildConfig.IsTestProject || request.BuildConfig.AssignExternal)
        {
            importsClass = importsClass.AddMembers(
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Init"))
                   .WithAttributeLists(
                        SingletonList(
                            AttributeList(
                                SeparatedList(
                                    [
                                        Attribute(ParseName("System.Runtime.CompilerServices.ModuleInitializer")),
                                        Attribute(ParseName("System.ComponentModel.EditorBrowsable"))
                                           .WithArgumentList(
                                                AttributeArgumentList(
                                                    SingletonSeparatedList(
                                                        AttributeArgument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                ParseName("System.ComponentModel.EditorBrowsableState"),
                                                                IdentifierName("Never")
                                                            )
                                                        )
                                                    )
                                                )
                                            ),
                                    ]
                                )
                            )
                        )
                    )
                   .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)))
                   .WithBody(
                        Block(
                            List<StatementSyntax>(
                                [
                                    ExpressionStatement(
                                        InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("Environment"),
                                                    IdentifierName("SetEnvironmentVariable")
                                                )
                                            )
                                           .WithArgumentList(
                                                ArgumentList(
                                                    SeparatedList(
                                                        [
                                                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("CLAVUS__HOSTTYPE"))),
                                                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("UnitTest"))),
                                                        ]
                                                    )
                                                )
                                            )
                                    ),
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("ImportHelpers"),
                                                IdentifierName("Tectum")
                                            ),
                                            IdentifierName(request.BuildConfig.ImportConfiguration.MethodName)
                                        )
                                    ),
                                ]
                            )
                        )
                    )
            );
        }

        var cu = CompilationUnit()
                .WithAttributeLists(request.BuildConfig.ImportConfiguration.ToAttributes())
                .AddSharedTrivia()
                .WithUsings(
                     List(
                         [
                             UsingDirective(ParseName("System")),
                             UsingDirective(ParseName("System.Collections.Generic")),
                             UsingDirective(ParseName("System.Runtime.Loader")),
                             UsingDirective(ParseName("Microsoft.Extensions.DependencyInjection")),
                             UsingDirective(ParseName("Clavus")),
                             UsingDirective(ParseName("Clavus.Infrastructure")),
                         ]
                     )
                 );
        var members = new List<MemberDeclarationSyntax>
        {
            importsClass,
        };

        cu = cu
           .AddMembers(
                request.BuildConfig.ImportConfiguration is { Namespace: { Length: > 0 } relativeNamespace }
                    ? [NamespaceDeclaration(ParseName(relativeNamespace)).AddMembers(members.ToArray())]
                    : [.. members]
            );

        context.AddSource(
            "Imported_Assembly_Conventions.g.cs",
            cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
        );

        static BlockSyntax addEnumerateExportStatements(IReadOnlyCollection<string> references)
        {
            var block = Block();
            foreach (var reference in references)
            {
                block = block.AddStatements(
                    ForEachStatement(
                            IdentifierName("var"),
                            Identifier("part"),
                            InvocationExpression(ParseExpression(reference))
                               .WithArgumentList(ArgumentList()),
                            YieldStatement(SyntaxKind.YieldReturnStatement, IdentifierName("part"))
                        )
                       .NormalizeWhitespace()
                );
            }

            return block;
        }
    }

    public record Request
    (
        Compilation Compilation,
        MsBuildConfig BuildConfig,
        ImmutableArray<INamedTypeSymbol> ExportedConventions
    );

}
