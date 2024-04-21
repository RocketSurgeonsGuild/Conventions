using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Support;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

// ReSharper disable UnusedVariable
namespace Rocket.Surgery.Conventions;
// TODO: analyzers
//

/// <summary>
///     Generator to handle materializing conventions as code instead of loading them at runtime
/// </summary>
[Generator]
public class ConventionAttributesGenerator : IIncrementalGenerator
{
    private static IEnumerable<INamedTypeSymbol> GetExportedConventions(GeneratorAttributeSyntaxContext context)
    {
        foreach (var attribute in context.Attributes)
        {
            if (attribute is { AttributeClass.TypeArguments: [INamedTypeSymbol ta,], })
                yield return ta;
            if (attribute is { ConstructorArguments: [{ Value: INamedTypeSymbol sv, },], })
                yield return sv;
        }
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var exportConfiguration = ConventionConfigurationData.Create(context, "ExportConventions", ConventionConfigurationData.ExportsDefaults);

        var exportCandidates = context
                              .SyntaxProvider
                              .ForAttributeWithMetadataName(
                                   "Rocket.Surgery.Conventions.ConventionAttribute",
                                   (_, _) => true,
                                   (syntaxContext, _) => GetExportedConventions(syntaxContext)
                               )
                              .SelectMany((z, _) => z)
                              .WithComparer(SymbolEqualityComparer.Default);
        var exportCandidates2 = context
                               .SyntaxProvider
                               .ForAttributeWithMetadataName(
                                    "Rocket.Surgery.Conventions.ConventionAttribute`1",
                                    (_, _) => true,
                                    (syntaxContext, _) => GetExportedConventions(syntaxContext)
                                )
                               .SelectMany((z, _) => z)
                               .WithComparer(SymbolEqualityComparer.Default);

        var exportedConventions = context
                                 .SyntaxProvider
                                 .ForAttributeWithMetadataName(
                                      "Rocket.Surgery.Conventions.ExportConventionAttribute",
                                      (node, _) => node is TypeDeclarationSyntax,
                                      (syntaxContext, _) => (INamedTypeSymbol)syntaxContext.TargetSymbol
                                  )
                                 .WithComparer(SymbolEqualityComparer.Default);

        var combinedExports = exportCandidates
                             .Collect()
                             .Combine(exportCandidates2.Collect())
                             .SelectMany((tuple, _) => tuple.Left.AddRange(tuple.Right))
                             .Collect()
                             .Combine(exportedConventions.Collect())
                             .SelectMany((tuple, _) => tuple.Left.AddRange(tuple.Right))
                             .WithComparer(SymbolEqualityComparer.Default);

        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(exportConfiguration)
               .Select((z, _) => ConventionAttributeData.Create(z.Right, z.Left))
               .Combine(combinedExports.Collect().Select(static (z, _) => z.Distinct(SymbolEqualityComparer.Default).OfType<INamedTypeSymbol>()))
               .Combine(exportedConventions.Collect()),
            static (productionContext, tuple) => ExportConventions.HandleConventionExports(
                productionContext,
                new(
                    tuple.Left.Left,
                    tuple.Left.Right.OrderBy(z => z.ToDisplayString()).ToImmutableArray(),
                    tuple.Right.OrderBy(z => z.ToDisplayString()).ToImmutableArray()
                )
            )
        );

        var importConfiguration = ConventionConfigurationData
                                 .Create(context, "ImportConventions", ConventionConfigurationData.ImportsDefaults)
                                 .Select((z, _) => z with { Assembly = z is not { WasConfigured: false, Assembly: true, } && z.Assembly, });

        var hasAssemblyLoadContext = context.CompilationProvider
                                            .Select((compilation, _) => compilation.GetTypeByMetadataName("System.Runtime.Loader.AssemblyLoadContext") is { });
        var msBuildConfig = context.AnalyzerConfigOptionsProvider
                                   .Select(
                                        (provider, _) => ( isTestProject: provider.GlobalOptions.TryGetValue(
                                                               "build_property.IsTestProject",
                                                               out var isTestProjectString
                                                           )
                                                        && bool.TryParse(isTestProjectString, out var isTestProject)
                                                        && isTestProject,
                                                           rootNamespace: provider.GlobalOptions.TryGetValue(
                                                               "build_property.RootNamespace",
                                                               out var rootNamespace
                                                           )
                                                               ? rootNamespace
                                                               : null )
                                    );
        var rootNamespace = context.AnalyzerConfigOptionsProvider
                                   .Select(
                                        (provider, _) => provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var value) ? value : ""
                                    );

        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(combinedExports.Collect())
               .Combine(importConfiguration)
               .Combine(exportConfiguration)
               .Combine(hasAssemblyLoadContext)
               .Combine(msBuildConfig)
               .Select(
                    (z, _) => (
                        compilation: z.Left.Left.Left.Left.Left,
                        hasExports: z.Left.Left.Left.Left.Right.Any(),
                        exportedCandidates: z.Left.Left.Left.Left.Right,
                        importConfiguration: z.Left.Left.Left.Right, exportConfiguration: z.Left.Left.Right, hasAssemblyLoadContext: z.Left.Right,
                        msBuildConfig: z.Right
                    )
                ),
            static (productionContext, tuple) =>
            {
                if (!tuple.hasAssemblyLoadContext) return;
                ImportConventions.HandleConventionImports(
                    productionContext,
                    new(
                        tuple.compilation,
                        tuple.hasExports,
                        tuple.msBuildConfig,
                        tuple.importConfiguration,
                        tuple.exportConfiguration
                    )
                );
            }
        );

        var getAssembliesSyntaxProvider = context
                                         .SyntaxProvider.CreateSyntaxProvider(
                                              (node, _) => AssemblyCollection.GetAssembliesMethod(node) is { method: { }, selector: { }, },
                                              (syntaxContext, _) => AssemblyCollection.GetAssembliesMethod(syntaxContext)
                                          )
                                         .Combine(hasAssemblyLoadContext)
                                         .Where(z => z is { Right: true, Left: { method: { }, selector: { }, }, })
                                         .Select((z, _) => z.Left)
                                         .Collect();
        var getTypesSyntaxProvider = context
                                    .SyntaxProvider.CreateSyntaxProvider(
                                         (node, _) => TypeCollection.GetTypesMethod(node) is { method: { }, selector: { }, },
                                         (syntaxContext, _) => TypeCollection.GetTypesMethod(syntaxContext)
                                     )
                                    .Combine(hasAssemblyLoadContext)
                                    .Where(z => z is { Right: true, Left: { method: { }, selector: { }, }, })
                                    .Select((tuple, _) => tuple.Left)
                                    .Collect();
        context.RegisterImplementationSourceOutput(
            getAssembliesSyntaxProvider
               .Combine(getTypesSyntaxProvider)
               .Combine(importConfiguration)
               .Combine(msBuildConfig)
               .Combine(context.CompilationProvider),
            static (context, results) =>
            {
                AssemblyCollection.Collect(
                    context,
                    new(
                        results.Right,
                        results.Left.Left.Right,
                        results.Left.Left.Left.Left,
                        results.Left.Left.Left.Right,
                        results.Left.Right
                    )
                );
            }
        );

        var topLevelClass = context
                           .SyntaxProvider
                           .CreateSyntaxProvider(
                                (node, _) => node is CompilationUnitSyntax compilationUnitSyntax
                                 && compilationUnitSyntax.Members.OfType<GlobalStatementSyntax>().Any(),
                                (syntaxContext, _) => ( node: (CompilationUnitSyntax)syntaxContext.Node, semanticModel: syntaxContext.SemanticModel )
                            )
                           .Combine(importConfiguration)
                           .Where(z => z.Right.Assembly);
        context.RegisterImplementationSourceOutput(
            topLevelClass,
            static (context, input) =>
            {
                ( var compilation, var semanticModel ) = input.Left;
                var importConfiguration = input.Right;


                var hasReturn = compilation
                               .Members
                               .OfType<GlobalStatementSyntax>()
                               .FirstOrDefault(z => z.Statement is ReturnStatementSyntax { Expression: { }, });

                // UseRocketBooster
                // LaunchWith
                // ConfigureRocketSurgery
                var method = MethodDeclaration(
                                 hasReturn is { }
                                     ? GenericName(Identifier("Task"))
                                        .WithTypeArgumentList(
                                             TypeArgumentList(SingletonSeparatedList<TypeSyntax>(PredefinedType(Token(SyntaxKind.IntKeyword))))
                                         )
                                     : IdentifierName("Task"),
                                 Identifier("RunAsync")
                             )
                            .WithAttributeLists(SingletonList(Helpers.CompilerGeneratedAttributes))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.AsyncKeyword)))
                            .WithParameterList(
                                 ParameterList(
                                     SeparatedList(
                                         [
                                             Parameter(Identifier("args"))
                                                .WithType(
                                                     ArrayType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                                                        .WithRankSpecifiers(
                                                             SingletonList(
                                                                 ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))
                                                             )
                                                         )
                                                 ),
                                             Parameter(Identifier("factory"))
                                                .WithType(NullableType(IdentifierName("IConventionFactory")))
                                                .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                                             Parameter(Identifier("action"))
                                                .WithType(
                                                     NullableType(
                                                         GenericName(Identifier("Func"))
                                                            .WithTypeArgumentList(
                                                                 TypeArgumentList(
                                                                     SeparatedList<TypeSyntax>(
                                                                         new[]
                                                                         {
                                                                             IdentifierName("ConventionContextBuilder"),
                                                                             IdentifierName("CancellationToken"),
                                                                             IdentifierName("ValueTask"),
                                                                         }
                                                                     )
                                                                 )
                                                             )
                                                     )
                                                 )
                                                .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                                         ]
                                     )
                                 )
                             );
                var methodBlock = Block();
                foreach (var i in compilation.Members.OfType<GlobalStatementSyntax>())
                {
                    var nodes = i.Statement.DescendantNodesAndSelf();
                    var statement = i.Statement;
                    foreach (( var node, var memberAccessExpression ) in nodes
                                                                        .OfType<InvocationExpressionSyntax>()
                                                                        .Select(
                                                                             static z => z is
                                                                             {
                                                                                 Expression: MemberAccessExpressionSyntax
                                                                                 {
                                                                                     Name.Identifier.Text: "LaunchWith"
                                                                                  or "UseRocketBooster"
                                                                                  or "ConfigureRocketSurgery",
                                                                                 } memberAccessExpression,
                                                                             }
                                                                                 ? ( z, memberAccessExpression )
                                                                                 : ( z, null! )
                                                                         )
                                                                        .Where(z => z.memberAccessExpression is { })
                                                                        .Reverse())
                    {
                        var newNode = node;
                        if (node.ArgumentList.Arguments.Count > 1
                         && node is
                            {
                                ArgumentList.Arguments:
                                [{ Expression: var factoryExpression, }, { Expression: LambdaExpressionSyntax lambdaExpressionSyntax, }, ..,],
                            })
                        {
                            TypeSyntax? sourceActionType = null;
                            var newLambdaExpressionSyntax = lambdaExpressionSyntax;
                            NameSyntax factoryNameExpression = IdentifierName("factory");
                            if (factoryExpression is InvocationExpressionSyntax { ArgumentList.Arguments: [{ Expression: var otherFactoryExpresion, },], })
                            {
                                factoryExpression = otherFactoryExpresion;
                            }

                            if (lambdaExpressionSyntax is SimpleLambdaExpressionSyntax slex)
                            {
                                sourceActionType =
                                    GenericName(Identifier("Func"))
                                       .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SeparatedList<TypeSyntax>(
                                                    new[]
                                                    {
                                                        IdentifierName("ConventionContextBuilder"),
                                                        IdentifierName("ValueTask"),
                                                    }
                                                )
                                            )
                                        );
                                if (!slex.Modifiers.Any(z => z.IsKind(SyntaxKind.AsyncKeyword))
                                 && slex.ExpressionBody is { }
                                                       and not MemberAccessExpressionSyntax
                                                           {
                                                               Name.Identifier.Text: "CompletedTask",
                                                               Expression: IdentifierNameSyntax { Identifier.Text: "ValueTask", },
                                                           }
                                   )
                                {
                                    newLambdaExpressionSyntax = lambdaExpressionSyntax
                                                               .WithExpressionBody(null)
                                                               .WithBlock(
                                                                    Block(
                                                                        ExpressionStatement(slex.ExpressionBody),
                                                                        ReturnStatement(
                                                                            MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                IdentifierName("ValueTask"),
                                                                                IdentifierName("CompletedTask")
                                                                            )
                                                                        )
                                                                    )
                                                                );
                                }
                                else if (lambdaExpressionSyntax.Block is { })
                                {
                                    newLambdaExpressionSyntax = lambdaExpressionSyntax
                                       .WithBody(
                                            lambdaExpressionSyntax.Block.AddStatements(
                                                ReturnStatement(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("ValueTask"),
                                                        IdentifierName("CompletedTask")
                                                    )
                                                )
                                            )
                                        );
                                }
                            }
                            else if (lambdaExpressionSyntax is ParenthesizedLambdaExpressionSyntax plex)
                            {
                                sourceActionType =
                                    GenericName(Identifier("Func"))
                                       .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SeparatedList<TypeSyntax>(
                                                    new[]
                                                    {
                                                        IdentifierName("ConventionContextBuilder"),
                                                        IdentifierName("CancellationToken"),
                                                        IdentifierName("ValueTask"),
                                                    }
                                                )
                                            )
                                        );
                            }

                            if (sourceActionType is { })
                            {
                                var variable = LocalDeclarationStatement(
                                    VariableDeclaration(sourceActionType)
                                       .WithVariables(
                                            SingletonSeparatedList(
                                                VariableDeclarator(Identifier("sourceAction"))
                                                   .WithInitializer(EqualsValueClause(newLambdaExpressionSyntax))
                                            )
                                        )
                                );

                                var beforeSpot = methodBlock.Statements.FirstOrDefault();
                                methodBlock = beforeSpot is { } ? methodBlock.InsertNodesBefore(beforeSpot, [variable,]) : methodBlock.AddStatements(variable);
                            }

                            newNode = newNode.ReplaceNodes(
                                [factoryExpression, lambdaExpressionSyntax,],
                                (original, _) =>
                                {
                                    return original == factoryExpression
                                        ? BinaryExpression(SyntaxKind.CoalesceExpression, factoryNameExpression, factoryExpression)
                                        : ParenthesizedLambdaExpression()
                                         .WithAsyncKeyword(Token(SyntaxKind.AsyncKeyword))
                                         .WithParameterList(
                                              ParameterList(SeparatedList(new[] { Parameter(Identifier("builder")), Parameter(Identifier("token")), }))
                                          )
                                         .WithBlock(
                                              Block(
                                                  ExpressionStatement(
                                                      AwaitExpression(
                                                          InvocationExpression(IdentifierName("sourceAction"))
                                                             .WithArgumentList(
                                                                  ArgumentList(
                                                                      sourceActionType is GenericNameSyntax { TypeArgumentList.Arguments.Count: 3, }
                                                                          ? SeparatedList(
                                                                              new[] { Argument(IdentifierName("builder")), Argument(IdentifierName("token")), }
                                                                          )
                                                                          : SeparatedList(
                                                                              new[] { Argument(IdentifierName("builder")), }
                                                                          )
                                                                  )
                                                              )
                                                      )
                                                  ),
                                                  ExpressionStatement(
                                                      AwaitExpression(
                                                          InvocationExpression(IdentifierName("action"))
                                                             .WithArgumentList(
                                                                  ArgumentList(
                                                                      SeparatedList(
                                                                          new[]
                                                                          {
                                                                              Argument(
                                                                                  IdentifierName("builder")
                                                                              ),
                                                                              Argument(
                                                                                  IdentifierName("token")
                                                                              ),
                                                                          }
                                                                      )
                                                                  )
                                                              )
                                                      )
                                                  )
                                              )
                                          );
                                }
                            );
                        }

                        else
                        {
                            newNode = newNode.AddArgumentListArguments(Argument(IdentifierName("action")));
                        }

                        statement = statement.ReplaceNode(node, newNode);
                    }

                    methodBlock = methodBlock.AddStatements(statement);
                }

                var program = ClassDeclaration("Program")
                             .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword),]))
                             .AddMembers(method.WithBody(methodBlock));


                var cu = CompilationUnit()
                   .AddUsings(
                        compilation
                           .Usings.AddRange(
                                [
                                    UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                                    UsingDirective(ParseName("System.Threading")),
                                    UsingDirective(ParseName("System.Threading.Tasks")),
                                ]
                            )
                           .ToArray()
                    );
                if (importConfiguration is { Namespace.Length: > 0, })
                {
                    cu = cu.AddMembers(FileScopedNamespaceDeclaration(ParseName(importConfiguration.Namespace)));
                }

                cu = cu.AddMembers(program);

                context.AddSource(
                    "Program.cs",
                    cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }
        );
    }
}