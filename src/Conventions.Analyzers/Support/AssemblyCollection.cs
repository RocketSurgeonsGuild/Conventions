using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions.Support;

internal static class AssemblyCollection
{
    public static MethodDeclarationSyntax Execute(Request request)
    {
        if (!request.Items.Any()) return AssembliesMethod;
        var compilation = request.Compilation;

        var assemblySymbols = compilation
                             .References.Select(compilation.GetAssemblyOrModuleSymbol)
                             .Concat([compilation.Assembly,])
                             .Select(
                                  symbol =>
                                  {
                                      if (symbol is IAssemblySymbol assemblySymbol)
                                          return assemblySymbol;
                                      if (symbol is IModuleSymbol moduleSymbol) return moduleSymbol.ContainingAssembly;
                                      return null!;
                                  }
                              )
                             .Where(z => z is { })
                             .ToImmutableHashSet<IAssemblySymbol>(SymbolEqualityComparer.Default);

        var results = new List<(SourceLocation location, BlockSyntax block)>();
        foreach (var item in request.Items)
        {
            var filterAssemblies = assemblySymbols
                                  .Where(z => item.AssemblyFilter.IsMatch(compilation, z))
                                  .ToArray();

            if (filterAssemblies.Length == 0) continue;
            results.Add(( item.Location, GenerateDescriptors(compilation, filterAssemblies, request.PrivateAssemblies) ));
        }

        return results.Count == 0 ? AssembliesMethod : AssembliesMethod.WithBody(Block(SwitchGenerator.GenerateSwitchStatement(results)));
    }

    public static (InvocationExpressionSyntax method, ExpressionSyntax selector ) GetAssembliesMethod(SyntaxNode node)
    {
        return node is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name.Identifier.Text: "GetAssemblies",
            },
            ArgumentList.Arguments: [{ Expression: { } expression, },],
        } invocationExpressionSyntax
            ? ( invocationExpressionSyntax, expression )
            : default;
    }

    private static BlockSyntax GenerateDescriptors(Compilation compilation, IEnumerable<IAssemblySymbol> assemblies, HashSet<IAssemblySymbol> privateAssemblies)
    {
        var block = Block();
        foreach (var assembly in assemblies.OrderBy(z => z.ToDisplayString()))
        {
            // TODO: Make this always use the load context?
            if (StatementGeneration.GetAssemblyExpression(compilation, assembly) is not { } assemblyExpression)
            {
                privateAssemblies.Add(assembly);
                block = block.AddStatements(YieldStatement(SyntaxKind.YieldReturnStatement, StatementGeneration.GetPrivateAssembly(assembly)));
                continue;
            }

            block = block.AddStatements(YieldStatement(SyntaxKind.YieldReturnStatement, assemblyExpression));
        }

        return block;
    }

    private static readonly MethodDeclarationSyntax AssembliesMethod = MethodDeclaration(
                                                                           GenericName(Identifier("IEnumerable"))
                                                                              .WithTypeArgumentList(
                                                                                   TypeArgumentList(
                                                                                       SingletonSeparatedList<TypeSyntax>(IdentifierName("Assembly"))
                                                                                   )
                                                                               ),
                                                                           Identifier("GetAssemblies")
                                                                       )
                                                                      .WithExplicitInterfaceSpecifier(
                                                                           ExplicitInterfaceSpecifier(IdentifierName("IAssemblyProvider"))
                                                                       )
                                                                      .AddParameterListParameters(
                                                                           Parameter(Identifier("action"))
                                                                              .WithType(
                                                                                   GenericName(Identifier("Action"))
                                                                                      .WithTypeArgumentList(
                                                                                           TypeArgumentList(
                                                                                               SingletonSeparatedList<TypeSyntax>(
                                                                                                   IdentifierName("IAssemblyProviderAssemblySelector")
                                                                                               )
                                                                                           )
                                                                                       )
                                                                               ),
                                                                           Parameter(Identifier("filePath"))
                                                                              .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                                           Parameter(Identifier("memberName"))
                                                                              .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                                           Parameter(Identifier("lineNumber"))
                                                                              .WithType(PredefinedType(Token(SyntaxKind.IntKeyword)))
                                                                       )
                                                                      .WithBody(
                                                                           Block(SingletonList<StatementSyntax>(YieldStatement(SyntaxKind.YieldBreakStatement)))
                                                                       );

    public record Request(Compilation Compilation, ImmutableArray<Item> Items, HashSet<IAssemblySymbol> PrivateAssemblies);

    public record Item(SourceLocation Location, CompiledAssemblyFilter AssemblyFilter);
}