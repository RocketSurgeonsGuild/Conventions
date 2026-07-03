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
        var references = getReferences(request.Compilation, request is { HasExports: true, ExportConfiguration.Assembly: true }, request.ExportConfiguration);

        var functionBody = references.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(references);

        var compilation = request.Compilation;
        var importsClass =
            ClassDeclaration(request.ImportConfiguration.ClassName)
               .WithAttributeLists(
                    SingletonList(
                        CompilerGeneratedAttributes
                           .WithLeadingTrivia(GetXmlSummary("The class defined for importing Clavus parts into this assembly"))
                    )
                )
               .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)))
               .AddMembers(
                    FieldDeclaration(
                            VariableDeclaration(IdentifierName("LoadClavusParts"))
                               .WithVariables(
                                    SingletonSeparatedList(
                                        VariableDeclarator(Identifier(request.ImportConfiguration.MethodName))
                                           .WithInitializer(EqualsValueClause(IdentifierName("LoadClavusPartsMethod")))
                                    )
                                )
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword))),
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
                       .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList(
                                    Parameter(Identifier("builder")).WithType(IdentifierName("ClavusContextBuilder"))
                                )
                            )
                        )
                       .WithBody(functionBody)
                       .WithLeadingTrivia(GetXmlSummary("The Clavus parts imported into this assembly"))
                )
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        var cu = CompilationUnit()
                .WithAttributeLists(request.ImportConfiguration.ToAttributes("Imports"))
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
                request.ImportConfiguration is { Namespace: { Length: > 0 } relativeNamespace }
                    ? [NamespaceDeclaration(ParseName(relativeNamespace)).AddMembers(members.ToArray())]
                    : [.. members]
            );

        context.AddSource(
            "Imported_Assembly_Conventions.g.cs",
            cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
        );

        static IReadOnlyCollection<string> getReferences(Compilation compilation, bool exports, ClavusConfigurationData configurationData) => [
            .. compilation
              .References
              .Select(compilation.GetAssemblyOrModuleSymbol)
              .OfType<IAssemblySymbol>()
              .Select(
                   symbol =>
                   {
                       try
                       {
                           var config = ClavusConfigurationData.FromAssemblyAttributes(symbol, ClavusConfigurationData.ExportsDefaults);
                           if (symbol.GetTypeByMetadataName(
                                   config switch
                                   {
                                       { Namespace.Length: > 0, Postfix: true }  => $"{config.Namespace}.Conventions.{config.ClassName}",
                                       { Postfix: true }                         => $"Conventions.{config.ClassName}",
                                       { Namespace.Length: > 0, Postfix: false } => $"{config.Namespace}.{config.ClassName}",
                                       _                                         => config.ClassName,
                                   }
                               ) is { } configuredMetadata) { return configuredMetadata.ToDisplayString() + $".{config.MethodName}";
                           } }
                       catch
                       {
                           //
                       }

                       // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                       return null!;
                   }
               )
              .Where(z => !string.IsNullOrWhiteSpace(z))
              .Concat(
                   exports
                       ?
                       [
                           ( string.IsNullOrWhiteSpace(configurationData.Namespace) ? "" : configurationData.Namespace + "." )
                         + configurationData.ClassName
                         + "."
                         + configurationData.MethodName,
                       ]
                       : []
               )
              .OrderBy(z => z),
            ];

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
                               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("builder"))))),
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
        bool HasExports,
        ClavusConfigurationData ImportConfiguration,
        ClavusConfigurationData ExportConfiguration
    );
}
