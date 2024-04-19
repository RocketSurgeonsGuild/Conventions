using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions.Support;

internal static class AssemblyCollection
{
    public static void Collect(
        SourceProductionContext context,
        CollectRequest request
    )
    {
        var getAssemblies = request.GetAssemblies;
        var getTypes = request.GetTypes;
        var compilation = request.Compilation;
        var configurationData = request.ImportConfiguration;
        ( var discoveredAssemblyRequests, var discoveredTypeRequests ) = AssemblyProviderConfiguration.FromAssemblyAttributes(compilation);
        var privateAssemblies = new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default);

        var cu = CompilationUnit()
           .WithUsings(
                List(
                    [
                        UsingDirective(ParseName("System")),
                        UsingDirective(ParseName("System.Collections.Generic")),
                        UsingDirective(ParseName("System.Reflection")),
                        UsingDirective(ParseName("Microsoft.Extensions.DependencyInjection")),
                        UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                        UsingDirective(ParseName("Rocket.Surgery.Conventions.Reflection")),
                    ]
                )
            );

        var assemblyRequests = GetAssemblyDetails(context, compilation, getAssemblies);
        var typeRequests = TypeCollection.GetTypeDetails(context, compilation, getTypes);
        var attributes = AssemblyProviderConfiguration.ToAssemblyAttributes(assemblyRequests, typeRequests).ToArray();
        cu = cu.AddAttributeLists(attributes);
        var assemblyProvider = GetAssemblyProvider(
            compilation,
            assemblyRequests.AddRange(discoveredAssemblyRequests),
            typeRequests.AddRange(discoveredTypeRequests),
            privateAssemblies
        );

        if (privateAssemblies.Any()) cu = cu.AddUsings(UsingDirective(ParseName("System.Runtime.Loader")));

        var members =
            ClassDeclaration(configurationData.ClassName)
               .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.InternalKeyword),
                        Token(SyntaxKind.SealedKeyword),
                        Token(SyntaxKind.PartialKeyword)
                    )
                )
               .AddMembers(GetAssembliesProviderMethod(privateAssemblies.Any(), request.IsTestProject), assemblyProvider);

        cu = cu
            .WithLeadingTrivia(
                 TriviaList(
                     Trivia(
                         PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)
                            .WithErrorCodes(SingletonSeparatedList<ExpressionSyntax>(IdentifierName("CA1822")))
                     ),
                     Trivia(
                         PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)
                            .WithErrorCodes(SingletonSeparatedList<ExpressionSyntax>(IdentifierName("CS8618")))
                     ),
                     Trivia(
                         PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)
                            .WithErrorCodes(SingletonSeparatedList<ExpressionSyntax>(IdentifierName("CS8603")))
                     )
                 )
             )
            .AddMembers(
                 configurationData is { Namespace: { Length: > 0, } relativeNamespace, }
                     ? NamespaceDeclaration(ParseName(relativeNamespace)).AddMembers(members)
                     : members
             );

        context.AddSource(
            "Compiled_AssemblyProvider.cs",
            cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
        );
    }

    public static MethodDeclarationSyntax Execute(
        Request request
    )
    {
        if (!request.Items.Any()) return AssembliesMethod;
        var compilation = request.Compilation;

        var assemblySymbols = compilation
                             .References.Select(compilation.GetAssemblyOrModuleSymbol)
                             .Concat([compilation.Assembly,])
                             .Select(
                                  symbol => symbol switch
                                            {
                                                IAssemblySymbol assemblySymbol => assemblySymbol,
                                                IModuleSymbol moduleSymbol     => moduleSymbol.ContainingAssembly,
                                                _                              => null!,
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

    public static (InvocationExpressionSyntax method, ExpressionSyntax selector, SemanticModel semanticModel ) GetAssembliesMethod(
        GeneratorSyntaxContext context
    )
    {
        var baseData = GetAssembliesMethod(context.Node);
        if (baseData.method is null
         || baseData.selector is null
         || context.SemanticModel.GetTypeInfo(baseData.selector).ConvertedType is not INamedTypeSymbol
            {
                TypeArguments: [{ Name: "IAssemblyProviderAssemblySelector", }, ..,],
            })
            return default;

        return ( baseData.method, baseData.selector, semanticModel: context.SemanticModel );
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

    private static readonly MethodDeclarationSyntax AssembliesMethod =
        MethodDeclaration(
                GenericName(Identifier("IEnumerable"))
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("Assembly")))),
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

    private static ImmutableArray<Item> GetAssemblyDetails(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<(InvocationExpressionSyntax expression, ExpressionSyntax selector, SemanticModel semanticModel)> results
    )
    {
        var items = ImmutableArray.CreateBuilder<Item>();
        foreach (var tuple in results)
        {
            ( var methodCallSyntax, var selector, var semanticModel ) = tuple;

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

            var source = Helpers.CreateSourceLocation(methodCallSyntax, context.CancellationToken);
            // disallow list?
            if (source.MemberName == "GetAssemblyConventions" && source.FilePath.EndsWith("ConventionContextHelpers.cs")) continue;

            var i = new Item(source, assemblyFilter);
            items.Add(i);
        }

        return items.ToImmutable();
    }

    private static TypeDeclarationSyntax GetAssemblyProvider(
        Compilation compilation,
        ImmutableArray<Item> getAssemblies,
        ImmutableArray<TypeCollection.Item> getTypes,
        HashSet<IAssemblySymbol> privateAssemblies
    )
    {
        var getAssembliesMethod = Execute(new(compilation, getAssemblies, privateAssemblies));
        var getTypesMethod = TypeCollection.Execute(new(compilation, getTypes, privateAssemblies));
        var parameters =
            ParameterList(
                SingletonSeparatedList(Parameter(Identifier("context")).WithType(IdentifierName("AssemblyLoadContext")))
            );
        if (!privateAssemblies.Any()) parameters = ParameterList();

        return ClassDeclaration("AssemblyProvider")
              .AddAttributeLists(Helpers.CompilerGeneratedAttributes)
              .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
              .WithParameterList(parameters)
              .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName("IAssemblyProvider")))))
              .AddMembers(getAssembliesMethod, getTypesMethod)
              .AddMembers(
                   privateAssemblies
                      .OrderBy(z => z.ToDisplayString())
                      .SelectMany(StatementGeneration.AssemblyDeclaration)
                      .ToArray()
               );
    }

    private static MethodDeclarationSyntax GetAssembliesProviderMethod(bool hasPrivateAssemblies, bool referencesXUnit)
    {
        ArgumentSyntax[] args = hasPrivateAssemblies
            ?
            [
                Argument(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("builder"), IdentifierName("Properties")),
                            GenericName(Identifier("GetRequiredService"))
                               .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("AssemblyLoadContext"))))
                        )
                    )
                ),
            ]
            : [];

        var method = MethodDeclaration(IdentifierName("IAssemblyProvider"), Identifier("CreateAssemblyProvider"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(
                         ParameterList(SingletonSeparatedList(Parameter(Identifier("builder")).WithType(IdentifierName("ConventionContextBuilder"))))
                     );

        if (!referencesXUnit)
            return method
                  .WithExpressionBody(ArrowExpressionClause(ObjectCreationExpression(IdentifierName("AssemblyProvider")).AddArgumentListArguments(args)))
                  .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        return method.WithBody(
            Block(
                ExpressionStatement(
                    InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("builder"), IdentifierName("Set")))
                       .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("HostType"),
                                            IdentifierName("UnitTest")
                                        )
                                    )
                                )
                            )
                        )
                ),
                ReturnStatement(ObjectCreationExpression(IdentifierName("AssemblyProvider")).AddArgumentListArguments(args))
            )
        );
    }

    public record CollectRequest
    (
        Compilation Compilation,
        ConventionConfigurationData ImportConfiguration,
        ImmutableArray<(InvocationExpressionSyntax method, ExpressionSyntax selector, SemanticModel semanticModel)> GetAssemblies,
        ImmutableArray<(InvocationExpressionSyntax method, ExpressionSyntax selector, SemanticModel semanticModel)> GetTypes,
        bool IsTestProject
    );

    public record Request(Compilation Compilation, ImmutableArray<Item> Items, HashSet<IAssemblySymbol> PrivateAssemblies);

    public record Item(SourceLocation Location, CompiledAssemblyFilter AssemblyFilter);
}