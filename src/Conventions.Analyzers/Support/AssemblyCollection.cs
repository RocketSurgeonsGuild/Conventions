using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions.Support;

internal static class AssemblyCollection
{
    public record Request(Compilation Compilation, ImmutableArray<Item> Items);

    public record Item(SourceLocation Location, CompiledAssemblyFilter AssemblyFilter);

    public static MethodDeclarationSyntax Execute(Request request)
    {
        if (!request.Items.Any()) return AssembliesMethod;
        var compilation = request.Compilation;
        var results = new List<(SourceLocation location, BlockSyntax block)>();
        foreach (var item in request.Items)
        {
            var allAssemblies = new List<IAssemblySymbol>();
            foreach (var symbol in compilation.References.Select(compilation.GetAssemblyOrModuleSymbol).Concat([compilation.Assembly]))
            {
                switch (symbol)
                {
                    case IAssemblySymbol assemblySymbol when item.AssemblyFilter.IsMatch(compilation, assemblySymbol):
                        allAssemblies.Add(assemblySymbol);
                        break;
                    case IModuleSymbol moduleSymbol when item.AssemblyFilter.IsMatch(compilation, moduleSymbol.ContainingAssembly):
                        allAssemblies.Add(moduleSymbol.ContainingAssembly);
                        break;
                }
            }

            results.Add(( item.Location, GenerateDescriptors(compilation, allAssemblies.ToImmutableHashSet<IAssemblySymbol>(SymbolEqualityComparer.Default)) ));
        }

        return AssembliesMethod.WithBody(Block(SwitchGenerator.GenerateSwitchStatement(results)));
    }

    private static BlockSyntax GenerateDescriptors(Compilation compilation, ImmutableHashSet<IAssemblySymbol> assemblies)
    {
        var block = Block();
        foreach (var type in assemblies.OrderBy(z => z.ToDisplayString()))
        {
            if (StatementGeneration.GetAssemblyExpression(compilation, type) is not { } assemblyExpression) continue;
            block = block.AddStatements(YieldStatement(SyntaxKind.YieldReturnStatement, assemblyExpression));
        }

        return block;
    }

    private static MethodDeclarationSyntax AssembliesMethod = MethodDeclaration(
                                                                  GenericName(Identifier("IEnumerable"))
                                                                     .WithTypeArgumentList(
                                                                          TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("Assembly")))
                                                                      ),
                                                                  Identifier("GetAssemblies")
                                                              )
                                                             .WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifier(IdentifierName("IAssemblyProvider")))
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
                                                                  Parameter(Identifier("filePath")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                                  Parameter(Identifier("memberName")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                                  Parameter(Identifier("lineNumber")).WithType(PredefinedType(Token(SyntaxKind.IntKeyword)))
                                                              )
                                                             .WithBody(Block(SingletonList<StatementSyntax>(YieldStatement(SyntaxKind.YieldBreakStatement))));

    public static (InvocationExpressionSyntax method, ExpressionSyntax selector ) GetAssembliesMethod(SyntaxNode node) =>
        node is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name.Identifier.Text: "GetAssemblies",
            },
            ArgumentList.Arguments: [{ Expression: { } expression }]
        } invocationExpressionSyntax
            ? ( invocationExpressionSyntax, expression )
            : default;
}
