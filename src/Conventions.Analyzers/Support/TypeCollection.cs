﻿using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions.Support;

internal record SourceLocation(int LineNumber, string FilePath, string MemberName);

internal static class TypeCollection
{
    public record Request(Compilation Compilation, ImmutableArray<Item> Items);

    public record Item(SourceLocation Location, CompiledAssemblyFilter AssemblyFilter, CompiledTypeFilter TypeFilter);

    public static MethodDeclarationSyntax Execute(Request request)
    {
        var compilation = request.Compilation;
        if (request.Items.Any()) return TypesMethod;

        var privateAssemblies = new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default);
        var results = new List<(SourceLocation location, BlockSyntax block)>();
        foreach (var item in request.Items)
        {
            var types = TypeSymbolVisitor.GetTypes(compilation, item.TypeFilter, item.AssemblyFilter);
            var localBlock = GenerateDescriptors(compilation, types, privateAssemblies);
            results.Add(( item.Location, localBlock ));
        }

        return TypesMethod.WithBody(Block(SwitchGenerator.GenerateSwitchStatement(results)));
    }

    private static BlockSyntax GenerateDescriptors(Compilation compilation, ImmutableArray<INamedTypeSymbol> types, HashSet<IAssemblySymbol> privateAssemblies)
    {
        var block = Block();
        foreach (var type in types.OrderBy(z => z.ToDisplayString()))
        {
            block = block.AddStatements(ExpressionStatement(StatementGeneration.GetTypeOfExpression(compilation, type)));
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
            },
            ArgumentList.Arguments: [.., { Expression: { } expression }]
        } invocationExpressionSyntax
            ? ( invocationExpressionSyntax, expression )
            : default;
}