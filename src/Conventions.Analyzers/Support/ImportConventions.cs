using System.Collections.Immutable;
using System.Text;
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
        ( var compilation, var importCandidates, var hasExports, var importConfiguration, var exportConfiguration ) = request;
        var references = getReferences(compilation, hasExports && exportConfiguration.Assembly, exportConfiguration);

        var functionBody = references.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(references);

        addAssemblySource(context, functionBody, importConfiguration);

        if (importCandidates.OfType<ClassDeclarationSyntax>().Any())
        {
            var cu = CompilationUnit()
               .WithUsings(
                    List(
                        new[]
                        {
                            UsingDirective(ParseName("System")),
                            UsingDirective(ParseName("System.Collections.Generic")),
                            UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                        }
                    )
                );
            if (importConfiguration is { Assembly: true, Namespace: { Length: > 0, } importNamespace, })
            {
                cu = cu.AddUsings(UsingDirective(ParseName(importNamespace)));
            }

            foreach (var declaration in importCandidates.OfType<ClassDeclarationSyntax>())
            {
                var model = compilation.GetSemanticModel(declaration.SyntaxTree);
                var symbol = ModelExtensions.GetDeclaredSymbol(model, declaration);
                if (symbol == null)
                    continue; // TODO: Diagnostic
                var @namespace = symbol.ContainingNamespace;

                var methodDeclaration = MethodDeclaration(
                                            GenericName(Identifier("IEnumerable"))
                                               .WithTypeArgumentList(
                                                    TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies")))
                                                ),
                                            Identifier(importConfiguration.MethodName)
                                        )
                                       .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                                       .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList(
                                                    Parameter(Identifier("serviceProvider")).WithType(IdentifierName("IServiceProviderDictionary"))
                                                )
                                            )
                                        )
                                       .WithLeadingTrivia(GetXmlSummary("The conventions imported into this assembly"));
                if (importConfiguration.Assembly)
                {
                    methodDeclaration = methodDeclaration
                                       .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName(importConfiguration.ClassName),
                                                            IdentifierName(importConfiguration.MethodName)
                                                        )
                                                    )
                                                   .WithArgumentList(
                                                        ArgumentList(SingletonSeparatedList(Argument(IdentifierName("serviceProvider"))))
                                                    )
                                            )
                                        )
                                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                }
                else
                {
                    methodDeclaration = methodDeclaration
                       .WithBody(functionBody);
                }

                var derivedClass = ClassDeclaration(declaration.Identifier)
                                  .WithModifiers(TokenList(declaration.Modifiers.Select(z => z.WithoutTrivia())))
                                  .WithConstraintClauses(declaration.ConstraintClauses)
                                  .WithTypeParameterList(declaration.TypeParameterList)
                                  .WithMembers(SingletonList<MemberDeclarationSyntax>(methodDeclaration))
                                  .NormalizeWhitespace();
                cu = cu
                   .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            NamespaceDeclaration(ParseName(@namespace.ToDisplayString()))
                               .WithMembers(SingletonList<MemberDeclarationSyntax>(derivedClass))
                        )
                    );
            }

            context.AddSource(
                "Imported_Class_Conventions.cs",
                cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
            );
        }

        static void addAssemblySource(SourceProductionContext context, BlockSyntax syntax, ConventionConfigurationData configurationData)
        {
            var members =
                ClassDeclaration(configurationData.ClassName)
                   .WithAttributeLists(
                        SingletonList(
                            CompilerGeneratedAttributes
                               .WithLeadingTrivia(GetXmlSummary("The class defined for importing conventions into this assembly"))
                        )
                    )
                   .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.InternalKeyword),
                            Token(SyntaxKind.StaticKeyword),
                            Token(SyntaxKind.PartialKeyword)
                        )
                    )
                   .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            MethodDeclaration(
                                    GenericName(Identifier("IEnumerable"))
                                       .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies"))
                                            )
                                        ),
                                    Identifier(configurationData.MethodName)
                                )
                               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                               .WithParameterList(
                                    ParameterList(
                                        SingletonSeparatedList(
                                            Parameter(Identifier("serviceProvider")).WithType(IdentifierName("IServiceProviderDictionary"))
                                        )
                                    )
                                )
                               .WithBody(syntax)
                               .WithLeadingTrivia(GetXmlSummary("The conventions imported into this assembly"))
                        )
                    );
            var cu = CompilationUnit()
                    .WithAttributeLists(configurationData.ToAttributes("Imports"))
                    .WithUsings(
                         List(
                             new[]
                             {
                                 UsingDirective(ParseName("System")),
                                 UsingDirective(ParseName("System.Collections.Generic")),
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

                               var legacyMetadata = symbol.GetTypeByMetadataName($"{symbol.Name}.Conventions.Exports")
                                ?? symbol.GetTypeByMetadataName($"{symbol.Name}.Exports")
                                ?? symbol.GetTypeByMetadataName($"{symbol.Name}.__conventions__.__exports__");
                               if (legacyMetadata is { })
                               {
                                   return legacyMetadata.ToDisplayString() + ".GetConventions";
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
                               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("serviceProvider"))))),
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
        ImmutableArray<SyntaxNode> ImportCandidates,
        bool HasExports,
        ConventionConfigurationData ImportConfiguration,
        ConventionConfigurationData ExportConfiguration
    );
}