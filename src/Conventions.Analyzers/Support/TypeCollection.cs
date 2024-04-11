using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions.Support;

internal record SourceLocation
(
    [property: JsonPropertyName("l")]
    int LineNumber,
    [property: JsonPropertyName("f")]
    string FilePath,
    [property: JsonPropertyName("m")]
    string MemberName);

internal static class TypeCollection
{
    public record Request(Compilation Compilation, ImmutableArray<Item> Items, HashSet<IAssemblySymbol> PrivateAssemblies);

    public record Item(SourceLocation Location, CompiledAssemblyFilter AssemblyFilter, CompiledTypeFilter TypeFilter);

    public static MethodDeclarationSyntax Execute(Request request)
    {
        if (!request.Items.Any()) return TypesMethod;
        var compilation = request.Compilation;

        var results = new List<(SourceLocation location, BlockSyntax block)>();
        foreach (var item in request.Items)
        {
            var reducedTypes = TypeSymbolVisitor.GetTypes(compilation, item.AssemblyFilter, item.TypeFilter);
            var localBlock = GenerateDescriptors(compilation, reducedTypes, request.PrivateAssemblies);
            results.Add(( item.Location, localBlock ));
        }

        return TypesMethod.WithBody(Block(SwitchGenerator.GenerateSwitchStatement(results)));
    }

    private static BlockSyntax GenerateDescriptors(Compilation compilation, IEnumerable<INamedTypeSymbol> types, HashSet<IAssemblySymbol> privateAssemblies)
    {
        var block = Block();
        foreach (var type in types.OrderBy(z => z.ToDisplayString()))
        {
            block = block.AddStatements(YieldStatement(SyntaxKind.YieldReturnStatement, StatementGeneration.GetTypeOfExpression(compilation, type)));
            if (compilation.IsSymbolAccessibleWithin(type, compilation.Assembly)) continue;
            privateAssemblies.Add(type.ContainingAssembly);
        }

        return block;
    }

    private static MethodDeclarationSyntax TypesMethod = MethodDeclaration(
                                                             GenericName(Identifier("IEnumerable"))
                                                                .WithTypeArgumentList(
                                                                     TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("Type")))
                                                                 ),
                                                             Identifier("GetTypes")
                                                         )
                                                        .WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifier(IdentifierName("IAssemblyProvider")))
                                                        .AddParameterListParameters(
                                                             Parameter(Identifier("selector"))
                                                                .WithType(
                                                                     GenericName(Identifier("Func"))
                                                                        .AddTypeArgumentListArguments(
                                                                             IdentifierName("ITypeProviderAssemblySelector"),
                                                                             GenericName(Identifier("IEnumerable"))
                                                                                .WithTypeArgumentList(
                                                                                     TypeArgumentList(
                                                                                         SingletonSeparatedList<TypeSyntax>(IdentifierName("Type"))
                                                                                     )
                                                                                 )
                                                                         )
                                                                 ),
                                                             Parameter(Identifier("filePath")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                             Parameter(Identifier("memberName")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                             Parameter(Identifier("lineNumber")).WithType(PredefinedType(Token(SyntaxKind.IntKeyword)))
                                                         )
                                                        .WithBody(Block(SingletonList<StatementSyntax>(YieldStatement(SyntaxKind.YieldBreakStatement))));

    public static (InvocationExpressionSyntax method, ExpressionSyntax selector ) GetTypesMethod(SyntaxNode node) =>
        node is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name.Identifier.Text: "GetTypes",
                Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "AssemblyProvider" }
            },
            ArgumentList.Arguments: [.., { Expression: { } expression }]
        } invocationExpressionSyntax
            ? ( invocationExpressionSyntax, expression )
            : default;
}
