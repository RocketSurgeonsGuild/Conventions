using System.Collections.Immutable;
using System.Reflection;
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
        if (s == null || isRootNamespace(s)) return string.Empty;

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!isRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
                sb.Insert(0, '+');
            else
                sb.Insert(0, '.');

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

    public static IEnumerable<INamedTypeSymbol> GetBaseTypes(Compilation compilation, INamedTypeSymbol namedTypeSymbol)
    {
        while (namedTypeSymbol.BaseType != null)
        {
            if (SymbolEqualityComparer.Default.Equals(namedTypeSymbol.BaseType, compilation.ObjectType)) yield break;
            yield return namedTypeSymbol.BaseType;
            namedTypeSymbol = namedTypeSymbol.BaseType;
        }
    }

    public static CompilationUnitSyntax AddSharedTrivia(this CompilationUnitSyntax source)
    {
        return source
              .WithLeadingTrivia(
                   Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)),
                   Trivia(
                       PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)
                          .WithErrorCodes(SeparatedList(List(DisabledWarnings.Value)))
                   )
               )
              .WithTrailingTrivia(
                   Trivia(
                       PragmaWarningDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)
                          .WithErrorCodes(SeparatedList(List(DisabledWarnings.Value)))
                   ),
                   Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)),
                   CarriageReturnLineFeed
               );
    }

    public static bool IsOpenGenericType(this INamedTypeSymbol type)
    {
        return type.IsGenericType && ( type.IsUnboundGenericType || type.TypeArguments.All(z => z.TypeKind == TypeKind.TypeParameter) );
    }

    public static string GetGenericDisplayName(ISymbol? symbol)
    {
        if (symbol == null || IsRootNamespace(symbol)) return string.Empty;

        var sb = new StringBuilder(symbol.MetadataName);
        if (symbol is INamedTypeSymbol namedTypeSymbol && ( namedTypeSymbol.IsOpenGenericType() || namedTypeSymbol.IsGenericType ))
        {
            sb = new(symbol.Name);
            if (namedTypeSymbol.IsOpenGenericType())
            {
                sb.Append('<');
                for (var i = 1; i < namedTypeSymbol.Arity; i++)
                {
                    sb.Append(',');
                }

                sb.Append('>');
            }
            else
            {
                sb.Append('<');
                for (var index = 0; index < namedTypeSymbol.TypeArguments.Length; index++)
                {
                    var argument = namedTypeSymbol.TypeArguments[index];
                    sb.Append(GetGenericDisplayName(argument));
                    if (index < namedTypeSymbol.TypeArguments.Length - 1)
                        sb.Append(',');
                }

                sb.Append('>');
            }
        }

        var last = symbol;

        var workingSymbol = symbol.ContainingSymbol;

        while (!IsRootNamespace(workingSymbol))
        {
            if (workingSymbol is ITypeSymbol && last is ITypeSymbol)
                sb.Insert(0, '+');
            else
                sb.Insert(0, '.');

            sb.Insert(0, workingSymbol.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Trim());
            //sb.Insert(0, symbol.MetadataName);
            workingSymbol = workingSymbol.ContainingSymbol;
        }

        sb.Insert(0, "global::");
        return sb.ToString();

        static bool IsRootNamespace(ISymbol symbol)
        {
            INamespaceSymbol? s;
            return ( s = symbol as INamespaceSymbol ) != null && s.IsGlobalNamespace;
        }
    }

    public static string GetTypeOfName(ISymbol? symbol)
    {
        if (symbol == null || IsRootNamespace(symbol)) return string.Empty;

        var sb = new StringBuilder(symbol.MetadataName);
        if (symbol is INamedTypeSymbol namedTypeSymbol && ( namedTypeSymbol.IsOpenGenericType() || namedTypeSymbol.IsGenericType ))
        {
            sb = new(symbol.Name);
            if (namedTypeSymbol.IsOpenGenericType())
            {
                sb.Append('<');
                for (var i = 1; i < namedTypeSymbol.Arity; i++)
                {
                    sb.Append(',');
                }

                sb.Append('>');
            }
            else
            {
                sb.Append('<');
                for (var index = 0; index < namedTypeSymbol.TypeArguments.Length; index++)
                {
                    var argument = namedTypeSymbol.TypeArguments[index];
                    sb.Append(GetGenericDisplayName(argument));
                    if (index < namedTypeSymbol.TypeArguments.Length - 1)
                        sb.Append(',');
                }

                sb.Append('>');
            }
        }

        var last = symbol;

        var workingSymbol = symbol.ContainingSymbol;

        while (!IsRootNamespace(workingSymbol))
        {
            if (workingSymbol is ITypeSymbol && last is ITypeSymbol)
                sb.Insert(0, '.');
            else
                sb.Insert(0, '.');

            sb.Insert(0, workingSymbol.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Trim());
            //sb.Insert(0, symbol.MetadataName);
            workingSymbol = workingSymbol.ContainingSymbol;
        }

        sb.Insert(0, "global::");
        return sb.ToString();

        static bool IsRootNamespace(ISymbol symbol)
        {
            INamespaceSymbol? s;
            return ( s = symbol as INamespaceSymbol ) != null && s.IsGlobalNamespace;
        }
    }

    public static INamedTypeSymbol? GetUnboundGenericType(INamedTypeSymbol symbol)
    {
        return symbol switch
               {
                   { IsGenericType: true, IsUnboundGenericType: true, } => symbol,
                   { IsGenericType: true, }                             => symbol.ConstructUnboundGenericType(),
                   _                                                    => default,
               };
    }

    public static bool HasImplicitGenericConversion(
        Compilation compilation,
        INamedTypeSymbol assignableToType,
        INamedTypeSymbol assignableFromType
    )
    {
        if (SymbolEqualityComparer.Default.Equals(compilation.ObjectType, assignableToType)) return false;
        if (SymbolEqualityComparer.Default.Equals(compilation.ObjectType, assignableFromType)) return false;
        if (SymbolEqualityComparer.Default.Equals(assignableToType, assignableFromType))
            return true;
        if (compilation.HasImplicitConversion(assignableFromType, assignableToType)) return true;
        if (assignableToType is not { Arity: > 0, IsUnboundGenericType: true, }) return false;

        var matchingBaseTypes = GetBaseTypes(compilation, assignableFromType)
                               .Select(GetUnboundGenericType)
                               .Where(symbol => symbol is { } && compilation.HasImplicitConversion(symbol, assignableToType));
        if (matchingBaseTypes.Any()) return true;

        var matchingInterfaces = assignableFromType
                                .AllInterfaces
                                .Select(GetUnboundGenericType)
                                .Where(symbol => symbol is { } && compilation.HasImplicitConversion(symbol, assignableToType));
        return matchingInterfaces.Any();
    }


    internal static AttributeListSyntax CompilerGeneratedAttributes =
        AttributeList(
            SeparatedList(
                [
                    Attribute(ParseName("System.CodeDom.Compiler.GeneratedCode"))
                       .WithArgumentList(
                            AttributeArgumentList(
                                SeparatedList(
                                    [
                                        AttributeArgument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal(typeof(Helpers).Assembly.GetName().Name)
                                            )
                                        ),
                                        AttributeArgument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal(typeof(Helpers).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "generated")
                                            )
                                        ),
                                    ]
                                )
                            )
                        ),
                    Attribute(ParseName("System.Runtime.CompilerServices.CompilerGenerated")),
                    Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")),
                ]
            )
        );

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
           .WithTarget(AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword)));
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


    private static readonly string[] _disabledWarnings =
    [
        "CA1002",
        "CA1034",
        "CA1822",
        "CS0105",
        "CS1573",
        "CS8602",
        "CS8603",
        "CS8618",
        "CS8669",
    ];

    private static readonly Lazy<ImmutableArray<ExpressionSyntax>> DisabledWarnings = new(
        () => _disabledWarnings
             .Select(z => (ExpressionSyntax)IdentifierName(z))
             .ToImmutableArray()
    );

    private static readonly SyntaxToken XmlNewLine = XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList());
}
