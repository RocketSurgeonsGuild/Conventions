﻿using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Rocket.Surgery.Conventions.Helpers;

namespace Rocket.Surgery.Conventions.Support;

internal static class ImportConventions
{
    public static void HandleConventionImports(
        SourceProductionContext context,
        Request request
    )
    {
        var references = getReferences(request.Compilation, request is { HasExports: true, ExportConfiguration.Assembly: true }, request.ExportConfiguration);

        var functionBody = references.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(references);

        addAssemblySource(context, functionBody, request.ImportConfiguration, request.IsTestProject);

        static void addAssemblySource(
            SourceProductionContext context,
            BlockSyntax syntax,
            ConventionConfigurationData configurationData,
            bool referencesXUnit
        )
        {
            var members =
                ClassDeclaration(configurationData.ClassName)
                   .WithAttributeLists(
                        SingletonList(
                            CompilerGeneratedAttributes
                               .WithLeadingTrivia(GetXmlSummary("The class defined for importing conventions into this assembly"))
                        )
                    )
                   .AddBaseListTypes(SimpleBaseType(IdentifierName("IConventionFactory")))
                   .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.InternalKeyword),
                            Token(SyntaxKind.SealedKeyword),
                            Token(SyntaxKind.PartialKeyword)
                        )
                    )
                   .AddMembers(
                        PropertyDeclaration(
                                IdentifierName("IConventionFactory"),
                                Identifier(configurationData.MethodName)
                            )
                           .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword)
                                )
                            )
                           .WithAccessorList(
                                AccessorList(
                                    SingletonList(
                                        AccessorDeclaration(
                                                SyntaxKind.GetAccessorDeclaration
                                            )
                                           .WithSemicolonToken(
                                                Token(SyntaxKind.SemicolonToken)
                                            )
                                    )
                                )
                            )
                           .WithInitializer(
                                EqualsValueClause(
                                    InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ObjectCreationExpression(IdentifierName(configurationData.ClassName)).WithArgumentList(ArgumentList()),
                                                IdentifierName("OrCallerConventions")
                                            )
                                        )
                                       .WithArgumentList(ArgumentList())
                                )
                            )
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        MethodDeclaration(
                                GenericName(Identifier("IEnumerable"))
                                   .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies"))
                                        )
                                    ),
                                Identifier("LoadConventions")
                            )
                           .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                           .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList(
                                        Parameter(Identifier("builder")).WithType(IdentifierName("ConventionContextBuilder"))
                                    )
                                )
                            )
                           .WithBody(syntax)
                           .WithLeadingTrivia(GetXmlSummary("The conventions imported into this assembly"))
                    );


            if (referencesXUnit)
            {
                members = members.AddMembers(
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
                       .WithModifiers(
                            TokenList(
                                [
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword),
                                ]
                            )
                        )
                       .WithBody(
                            Block(
                                SingletonList<StatementSyntax>(
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("ImportHelpers"),
                                                IdentifierName("ExternalConventions")
                                            ),
                                            IdentifierName(configurationData.MethodName)
                                        )
                                    )
                                )
                            )
                        )
                );
            }

            var cu = CompilationUnit()
                    .WithAttributeLists(configurationData.ToAttributes("Imports"))
                    .WithUsings(
                         List(
                             new[]
                             {
                                 UsingDirective(ParseName("System")),
                                 UsingDirective(ParseName("System.Collections.Generic")),
                                 UsingDirective(ParseName("System.Runtime.Loader")),
                                 UsingDirective(ParseName("Microsoft.Extensions.DependencyInjection")),
                                 UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                             }
                         )
                     );
            if (configurationData is { Assembly: true, })
            {
                cu = cu
                   .AddMembers(
                        configurationData is { Namespace: { Length: > 0, } relativeNamespace, }
                            ? NamespaceDeclaration(ParseName(relativeNamespace)).AddMembers(members)
                            : members
                    );
            }

            context.AddSource(
                "Imported_Assembly_Conventions.cs",
                cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
            );
        }

        static IReadOnlyCollection<string> getReferences(Compilation compilation, bool exports, ConventionConfigurationData configurationData)
        {
            return compilation
                  .References
                  .Select(compilation.GetAssemblyOrModuleSymbol)
                  .OfType<IAssemblySymbol>()
                  .Select(
                       symbol =>
                       {
                           try
                           {
                               var data = ConventionConfigurationData.FromAssemblyAttributes(symbol, ConventionConfigurationData.ExportsDefaults);
                               var configuredMetadata =
                                   string.IsNullOrWhiteSpace(data.Namespace)
                                       ? symbol.GetTypeByMetadataName(data.ClassName)
                                       : symbol.GetTypeByMetadataName($"{data.Namespace}.Conventions.{data.ClassName}")
                                    ?? symbol.GetTypeByMetadataName($"{data.Namespace}.{data.ClassName}");
                               if (configuredMetadata is { })
                               {
                                   return configuredMetadata.ToDisplayString() + $".{data.MethodName}";
                               }

                               // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                               return null!;
                           }
                           catch
                           {
                               // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                               return null!;
                           }
                       }
                   )
                  .Where(z => !string.IsNullOrWhiteSpace(z))
                  .Concat(
                       exports
                           ? new[]
                           {
                               ( string.IsNullOrWhiteSpace(configurationData.Namespace) ? "" : configurationData.Namespace + "." )
                             + configurationData.ClassName
                             + "."
                             + configurationData.MethodName,
                           }
                           : Enumerable.Empty<string>()
                   )
                  .OrderBy(z => z)
                  .ToArray();
        }

        static BlockSyntax addEnumerateExportStatements(IReadOnlyCollection<string> references)
        {
            var block = Block();
            foreach (var reference in references)
            {
                block = block.AddStatements(
                    ForEachStatement(
                            IdentifierName("var"),
                            Identifier("convention"),
                            InvocationExpression(ParseExpression(reference))
                               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("builder"))))),
                            YieldStatement(SyntaxKind.YieldReturnStatement, IdentifierName("convention"))
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
        bool HasExports,
        bool IsTestProject,
        ConventionConfigurationData ImportConfiguration,
        ConventionConfigurationData ExportConfiguration
    );
}
