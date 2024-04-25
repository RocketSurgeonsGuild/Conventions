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
    public static MethodDeclarationSyntax Execute(Request request)
    {
        if (!request.Items.Any()) return TypesMethod;
        var compilation = request.Compilation;

        var results = new List<(SourceLocation location, BlockSyntax block)>();
        foreach (var item in request.Items)
        {
            var reducedTypes = TypeSymbolVisitor.GetTypes(compilation, item.AssemblyFilter, item.TypeFilter);
            if (reducedTypes.Length == 0) continue;
            var localBlock = GenerateDescriptors(compilation, reducedTypes, request.PrivateAssemblies);
            results.Add(( item.Location, localBlock ));
        }

        return results.Count == 0 ? TypesMethod : TypesMethod.WithBody(Block(SwitchGenerator.GenerateSwitchStatement(results)));
    }

    public static (InvocationExpressionSyntax method, ExpressionSyntax selector, SemanticModel semanticModel ) GetTypesMethod(GeneratorSyntaxContext context)
    {
        var baseData = GetTypesMethod(context.Node);
        if (baseData.method is null
         || baseData.selector is null
         || context.SemanticModel.GetTypeInfo(baseData.selector).ConvertedType is not INamedTypeSymbol
            {
                TypeArguments: [{ Name: "ITypeProviderAssemblySelector", }, ..,],
            })
            return default;

        return ( baseData.method, baseData.selector, semanticModel: context.SemanticModel );
    }

    public static (InvocationExpressionSyntax method, ExpressionSyntax selector ) GetTypesMethod(SyntaxNode node)
    {
        return node is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name.Identifier.Text: "GetTypes",
            },
            ArgumentList.Arguments: [.., { Expression: { } expression, },],
        } invocationExpressionSyntax
            ? ( invocationExpressionSyntax, expression )
            : default;
    }

    internal static ImmutableArray<Item> GetTypeDetails(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<(InvocationExpressionSyntax expression, ExpressionSyntax selector, SemanticModel semanticModel)> results
    )
    {
        var items = ImmutableArray.CreateBuilder<Item>();
        foreach (var tuple in results)
        {
            ( var methodCallSyntax, var selector, _ ) = tuple;

            var assemblies = new List<IAssemblyDescriptor>();
            var typeFilters = new List<ITypeFilterDescriptor>();
            var classFilter = ClassFilter.All;

            DataHelpers.HandleInvocationExpressionSyntax(
                context,
                compilation.GetSemanticModel(tuple.expression.SyntaxTree),
                selector,
                assemblies,
                typeFilters,
                compilation.ObjectType,
                ref classFilter,
                context.CancellationToken
            );

            var assemblyFilter = new CompiledAssemblyFilter(assemblies.ToImmutableArray());
            var typeFilter = new CompiledTypeFilter(classFilter, typeFilters.ToImmutableArray());

            var source = Helpers.CreateSourceLocation(methodCallSyntax, context.CancellationToken);

            var i = new Item(source, assemblyFilter, typeFilter);
            items.Add(i);
        }

        return items.ToImmutable();
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


    private static readonly MethodDeclarationSyntax TypesMethod = MethodDeclaration(
                                                                      GenericName(Identifier("IEnumerable"))
                                                                         .WithTypeArgumentList(
                                                                              TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("Type")))
                                                                          ),
                                                                      Identifier("GetTypes")
                                                                  )
                                                                 .WithExplicitInterfaceSpecifier(
                                                                      ExplicitInterfaceSpecifier(IdentifierName("IAssemblyProvider"))
                                                                  )
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
                                                                      Parameter(Identifier("filePath"))
                                                                         .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                                      Parameter(Identifier("memberName"))
                                                                         .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                                      Parameter(Identifier("lineNumber")).WithType(PredefinedType(Token(SyntaxKind.IntKeyword)))
                                                                  )
                                                                 .WithBody(
                                                                      Block(SingletonList<StatementSyntax>(YieldStatement(SyntaxKind.YieldBreakStatement)))
                                                                  );

    public record Request(Compilation Compilation, ImmutableArray<Item> Items, HashSet<IAssemblySymbol> PrivateAssemblies);

    public record Item(SourceLocation Location, CompiledAssemblyFilter AssemblyFilter, CompiledTypeFilter TypeFilter);
}