using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;
using Rocket.Surgery.Conventions.Support;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Rocket.Surgery.Conventions.Helpers;

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
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var exportConfigurationCandidate = ConventionConfigurationData.Create(context, "ExportConventions", ConventionConfigurationData.ExportsDefaults);

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
               .Combine(exportConfigurationCandidate)
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

        var importConfigurationCandidate = ConventionConfigurationData
                                          .Create(context, "ImportConventions", ConventionConfigurationData.ImportsDefaults)
                                          .Select((z, _) => !z.WasConfigured && z.Assembly ? z with { Assembly = false, } : z);

        var importCandidates = context
                              .SyntaxProvider
                              .ForAttributeWithMetadataName(
                                   "Rocket.Surgery.Conventions.ImportConventionsAttribute",
                                   (node, _) => node is TypeDeclarationSyntax,
                                   (syntaxContext, _) => syntaxContext.TargetNode
                               );

        context.RegisterSourceOutput(
            context
               .CompilationProvider
               .Combine(importCandidates.Collect())
               .Combine(combinedExports.Collect())
               .Combine(importConfigurationCandidate)
               .Combine(exportConfigurationCandidate)
               .Select(
                    (z, _) => ( compilation: z.Left.Left.Left.Left, hasExports: z.Left.Left.Right.Any(), exportedCandidates: z.Left.Left.Left.Right,
                                importConfiguration: z.Left.Right, exportConfiguration: z.Right )
                ),
            static (productionContext, tuple) => ImportConventions.HandleConventionImports(
                productionContext,
                new(
                    tuple.compilation,
                    tuple.exportedCandidates,
                    tuple.hasExports,
                    tuple.importConfiguration,
                    tuple.exportConfiguration
                )
            )
        );

        var hasAssemblyLoadContext = context.CompilationProvider
                                            .Select((compilation, _) => compilation.GetTypeByMetadataName("System.Runtime.Loader.AssemblyLoadContext") is { });
        var getAssembliesSyntaxProvider = context
                                         .SyntaxProvider.CreateSyntaxProvider(
                                              (node, _) => AssemblyCollection.GetAssembliesMethod(node) is { method: { }, selector: { } },
                                              (syntaxContext, _) => (
                                                  node: syntaxContext.Node,
                                                  symbol: syntaxContext.SemanticModel.GetSymbolInfo(syntaxContext.Node).Symbol!,
                                                  semanticModel: syntaxContext.SemanticModel
                                              )
                                          )
                                         .Where(z => z is { symbol: { } })
                                         .Select(
                                              (z, _) =>
                                              {
                                                  var (expression, selector) = AssemblyCollection.GetAssembliesMethod(z.node);
                                                  return ( expression, selector, z.symbol, z.semanticModel );
                                              }
                                          )
                                         .Combine(hasAssemblyLoadContext)
                                         .Where(z => z.Right)
                                         .Select((tuple, _) => ( tuple.Left.expression, tuple.Left.selector, tuple.Left.semanticModel, tuple.Left.symbol ))
                                         .Collect();
        var getTypesSyntaxProvider = context
                                    .SyntaxProvider.CreateSyntaxProvider(
                                         (node, _) => TypeCollection.GetTypesMethod(node) is { method: { }, selector: { } },
                                         (syntaxContext, _) => (
                                             node: syntaxContext.Node,
                                             symbol: syntaxContext.SemanticModel.GetSymbolInfo(syntaxContext.Node).Symbol!,
                                             semanticModel: syntaxContext.SemanticModel
                                         )
                                     )
                                    .Where(z => z is { symbol: { } })
                                    .Select(
                                         (z, _) =>
                                         {
                                             var (expression, selector) = TypeCollection.GetTypesMethod(z.node);
                                             return ( expression, selector, z.symbol, z.semanticModel );
                                         }
                                     )
                                    .Combine(hasAssemblyLoadContext)
                                    .Where(z => z.Right)
                                    .Select((tuple, _) => ( tuple.Left.expression, tuple.Left.selector, tuple.Left.semanticModel, tuple.Left.symbol ))
                                    .Collect();
        context.RegisterImplementationSourceOutput(
            getAssembliesSyntaxProvider
               .Combine(getTypesSyntaxProvider)
               .Combine(importConfigurationCandidate)
               .Combine(context.CompilationProvider),
            static (context, results) =>
            {
                var (getAssemblies, getTypes) = results.Left.Left;
                var configurationData = results.Left.Right;
                var compilation = results.Right;

                var getAssembliesMethod = GetAssemblyDetails(context, compilation, getAssemblies);
                var getTypesMethod = GetTypeDetails(context, compilation, getTypes);
                var assemblyProvider = ClassDeclaration("AssemblyProvider")
                                      .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
                                      .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName("IAssemblyProvider")))))
                                      .AddMembers(getAssembliesMethod, getTypesMethod);
                var members =
                    ClassDeclaration(configurationData.ClassName)
                       .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.InternalKeyword),
                                Token(SyntaxKind.StaticKeyword),
                                Token(SyntaxKind.PartialKeyword)
                            )
                        )
                       .AddMembers(assemblyProvider);
                var cu = CompilationUnit()
                        .WithUsings(
                             List(
                                 new[]
                                 {
                                     UsingDirective(ParseName("System")),
                                     UsingDirective(ParseName("System.Reflection")),
                                     UsingDirective(ParseName("System.Collections.Generic")),
                                     UsingDirective(ParseName("Microsoft.Extensions.DependencyInjection")),
                                     UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                                     UsingDirective(ParseName("Rocket.Surgery.Conventions.Reflection")),
                                 }
                             )
                         );
                if (configurationData is { Assembly: true, })
                {
                    cu = cu
                       .AddMembers(
                            configurationData is { Namespace: { Length: > 0, } relativeNamespace, }
                                ? NamespaceDeclaration(ParseName(relativeNamespace)).AddMembers(members)
                                : members
                        );
                }

                context.AddSource(
                    "Compiled_AssemblyProvider.cs",
                    cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }
        );
    }

    private static MemberDeclarationSyntax GetAssemblyDetails(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<(InvocationExpressionSyntax expression, ExpressionSyntax selector, SemanticModel semanticModel, ISymbol symbol)> results
    )
    {
        var items = new List<AssemblyCollection.Item>();
        foreach (var tuple in results)
        {
            var (methodCallSyntax, selector, semanticModel, symbol) = tuple;

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

            var containingMethod = methodCallSyntax.Ancestors().OfType<MethodDeclarationSyntax>().First();

            var i = new AssemblyCollection.Item(
                new(
                    methodCallSyntax
                       .SyntaxTree.GetText(context.CancellationToken)
                       .Lines.First(z => z.Span.IntersectsWith(methodCallSyntax.Span))
                       .LineNumber,
                    methodCallSyntax.SyntaxTree.FilePath,
                    containingMethod.Identifier.Text
                ),
                assemblyFilter
            );
            items.Add(i);
        }

        return AssemblyCollection.Execute(new(compilation, items.ToImmutableArray()));
    }

    private static MemberDeclarationSyntax GetTypeDetails(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<(InvocationExpressionSyntax expression, ExpressionSyntax selector, SemanticModel semanticModel, ISymbol symbol)> results
    )
    {
        var items = new List<TypeCollection.Item>();
        foreach (var tuple in results)
        {
            var (methodCallSyntax, selector, semanticModel, symbol) = tuple;

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
            var containingMethod = methodCallSyntax.Ancestors().OfType<MethodDeclarationSyntax>().First();

            var i = new TypeCollection.Item(
                new(
                    methodCallSyntax
                       .SyntaxTree.GetText(context.CancellationToken)
                       .Lines.First(z => z.Span.IntersectsWith(methodCallSyntax.Span))
                       .LineNumber,
                    methodCallSyntax.SyntaxTree.FilePath,
                    containingMethod.Identifier.Text
                ),
                assemblyFilter,
                typeFilter
            );
            items.Add(i);
        }

        return TypeCollection.Execute(new(compilation, items.ToImmutableArray()));
    }

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
}
