using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions;

internal static class Helpers
{
    public static string GetFullMetadataName(ISymbol? s)
    {
        if (s == null || isRootNamespace(s))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!isRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
            {
                sb.Insert(0, '+');
            }
            else
            {
                sb.Insert(0, '.');
            }

            sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            //sb.Insert(0, s.MetadataName);
            s = s.ContainingSymbol;
        }

        return sb.ToString();

        static bool isRootNamespace(ISymbol symbol)
        {
            INamespaceSymbol? s;
            return ( s = symbol as INamespaceSymbol ) != null && s.IsGlobalNamespace;
        }
    }

    internal static AttributeListSyntax AddAssemblyAttribute(string key, string? value)
    {
        return AttributeList(
                SingletonSeparatedList(
                    Attribute(QualifiedName(QualifiedName(IdentifierName("System"), IdentifierName("Reflection")), IdentifierName("AssemblyMetadata")))
                       .WithArgumentList(
                            AttributeArgumentList(
                                SeparatedList(
                                    [
                                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(key))),
                                        AttributeArgument(
                                            value is null
                                                ? LiteralExpression(SyntaxKind.NullLiteralExpression)
                                                : LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value))
                                        ),
                                    ]
                                )
                            )
                        )
                )
            )
           .WithTarget(
                AttributeTargetSpecifier(
                    Token(SyntaxKind.AssemblyKeyword)
                )
            );
    }

    internal static SyntaxTrivia GetXmlSummary(string text)
    {
        return Trivia(
            DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                List<XmlNodeSyntax>(
                    [
                        XmlText()
                           .WithTextTokens(
                                TokenList(XmlTextLiteral(TriviaList(DocumentationCommentExterior("///")), " ", " ", TriviaList()))
                            ),
                        XmlExampleElement(
                                SingletonList<XmlNodeSyntax>(
                                    XmlText()
                                       .WithTextTokens(
                                            TokenList(
                                                XmlNewLine,
                                                XmlTextLiteral(
                                                    TriviaList(DocumentationCommentExterior("    ///")),
                                                    $" {text}",
                                                    $" {text}",
                                                    TriviaList()
                                                ),
                                                XmlNewLine,
                                                XmlTextLiteral(
                                                    TriviaList(DocumentationCommentExterior("    ///")),
                                                    " ",
                                                    " ",
                                                    TriviaList()
                                                )
                                            )
                                        )
                                )
                            )
                           .WithStartTag(XmlElementStartTag(XmlName(Identifier("summary"))))
                           .WithEndTag(XmlElementEndTag(XmlName(Identifier("summary")))),
                        XmlText().WithTextTokens(TokenList(XmlNewLine)),
                    ]
                )
            )
        );
    }

    private static readonly SyntaxToken XmlNewLine = XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList());
}