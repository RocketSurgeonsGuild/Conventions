using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    private static readonly ConventionConfigurationData _exportsDefaultConfiguration = new(false, true, "", "Exports", "GetConventions");
    private static readonly ConventionConfigurationData _importsDefaultConfiguration = new(false, true, "", "Imports", "GetConventions");

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var exportConfigurationCandidate = ConventionConfigurationData.Create(context, "ExportConventions", _exportsDefaultConfiguration);
        var exportCandidates = context
                              .SyntaxProvider
                              .CreateSyntaxProvider(
                                   static (node, token) =>
                                   {
                                       return node is AttributeListSyntax { Target.Identifier.RawKind: (int)SyntaxKind.AssemblyKeyword } attributeListSyntax
                                           && attributeListSyntax.Attributes.Any(
                                                  z => z.Name.ToFullString().TrimEnd().EndsWith("Convention", StringComparison.Ordinal) ||
                                                       z.Name.ToFullString().TrimEnd().EndsWith("ConventionAttribute", StringComparison.Ordinal)
                                              );
                                   },
                                   static (syntaxContext, token) =>
                                       syntaxContext.Node is AttributeListSyntax attributeListSyntax
                                           ? ( attributeListSyntax, syntaxContext.SemanticModel )
                                           : default
                               )
                              .Combine(context.CompilationProvider)
                              .Select(
                                   (z, token) =>
                                   {
                                       var (attributeListSyntax, model) = z.Left;
                                       var compilation = z.Right;
                                       var conventionAttribute = z.Right.GetTypeByMetadataName("Rocket.Surgery.Conventions.ConventionAttribute")!;
                                       return ( attributeListSyntax, model, compilation, conventionAttribute );
                                   }
                               )
                              .Where(z => z.conventionAttribute != null)
                              .SelectMany((z, _) => GetExportedConventions(z.attributeListSyntax, z.model, z.compilation, z.conventionAttribute!))
                              .WithComparer(SymbolEqualityComparer.Default);

        var exportedConventions = context
                                 .SyntaxProvider
                                 .CreateSyntaxProvider(
                                      static (node, token) =>
                                      {
                                          return node is TypeDeclarationSyntax baseType and (ClassDeclarationSyntax or RecordDeclarationSyntax)
                                              && baseType.AttributeLists.Any(
                                                     z => z.Target is null or { Identifier: { RawKind: (int)SyntaxKind.ClassKeyword } }
                                                       && z.Attributes.Any(
                                                              c => c.Name.ToFullString().TrimEnd().EndsWith("ExportConvention", StringComparison.Ordinal)
                                                                || c.Name.ToFullString().TrimEnd().EndsWith(
                                                                       "ExportConventionAttribute", StringComparison.Ordinal
                                                                   )
                                                          )
                                                 );
                                      },
                                      static (syntaxContext, token) => syntaxContext.Node is TypeDeclarationSyntax baseType
                                          ? ( baseType, syntaxContext.SemanticModel )
                                          : default
                                  )
                                 .SelectMany((z, _) => GetExportedConventions(z.baseType, z.SemanticModel))
                                 .WithComparer(SymbolEqualityComparer.Default);

        var combinedExports = exportCandidates.Collect()
                                              .Combine(exportedConventions.Collect())
                                              .SelectMany(
                                                   (tuple, token) =>
                                                       tuple.Left.AddRange(tuple.Right).Distinct(SymbolEqualityComparer.Default).OfType<INamedTypeSymbol>()
                                               )
                                              .WithComparer(SymbolEqualityComparer.Default);

        context.RegisterSourceOutput(
            context.CompilationProvider
                   .Combine(exportConfigurationCandidate)
                   .Select((z, token) => ConventionAttributeData.Create(z.Right, z.Left))
                   .Combine(combinedExports.Collect())
                   .Combine(exportedConventions.Collect())
                   .Select((z, token) => ( data: z.Left.Left, conventions: z.Left.Right, exportedConventions: z.Right )),
            static (productionContext, tuple) => HandleConventionExports(productionContext, tuple.data, tuple.conventions, tuple.exportedConventions)
        );

        var importConfigurationCandidate = ConventionConfigurationData.Create(context, "ImportConventions", _importsDefaultConfiguration)
                                                                      .Select((z, _) => !z.WasConfigured && z.Assembly ? z with { Assembly = false } : z);

        var importCandidates = context
                              .SyntaxProvider
                              .CreateSyntaxProvider(
                                   static (node, token) =>
                                   {
                                       return node is ClassDeclarationSyntax classDeclarationSyntax
                                           && classDeclarationSyntax.AttributeLists.SelectMany(z => z.Attributes)
                                                                    .Any(
                                                                         z => z.Name.ToFullString().TrimEnd().EndsWith(
                                                                                  "ImportConventions", StringComparison.OrdinalIgnoreCase
                                                                              )
                                                                           || z.Name.ToFullString().TrimEnd().EndsWith(
                                                                                  "ImportConventionsAttribute", StringComparison.OrdinalIgnoreCase
                                                                              )
                                                                     );
                                   },
                                   static (syntaxContext, token) => syntaxContext.Node
                               );
        context.RegisterSourceOutput(
            context.CompilationProvider
                   .Combine(importCandidates.Collect())
                   .Combine(combinedExports.Collect())
                   .Combine(importConfigurationCandidate)
                   .Combine(exportConfigurationCandidate)
                   .Select(
                        (z, token) => ( compilation: z.Left.Left.Left.Left, hasExports: z.Left.Left.Right.Any(), exportedCandidates: z.Left.Left.Left.Right,
                                        importConfiguration: z.Left.Right, exportConfiguration: z.Right )
                    ),
            static (productionContext, tuple) => HandleConventionImports(
                productionContext, tuple.compilation, tuple.exportedCandidates, tuple.hasExports, tuple.importConfiguration, tuple.exportConfiguration
            )
        );
    }

    private static void HandleConventionImports(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<SyntaxNode> importCandidates,
        bool hasExports,
        ConventionConfigurationData importConfiguration,
        ConventionConfigurationData exportConfiguration
    )
    {
        IReadOnlyCollection<string>? references = null;
        references = getReferences(compilation, hasExports && exportConfiguration.Assembly, exportConfiguration);

        var functionBody = references.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(references);

        addAssemblySource(context, functionBody, importConfiguration);

        if (importCandidates.OfType<ClassDeclarationSyntax>().Any())
        {
            var cu = CompilationUnit()
               .WithUsings(
                    List(
                        new[]
                        {
                            UsingDirective(ParseName("System")),
                            UsingDirective(ParseName("System.Collections.Generic")),
                            UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                        }
                    )
                );
            if (importConfiguration.Assembly && !string.IsNullOrWhiteSpace(importConfiguration.Namespace))
            {
                cu = cu.AddUsings(UsingDirective(ParseName(importConfiguration.Namespace!)));
            }

            foreach (var declaration in importCandidates.OfType<ClassDeclarationSyntax>())
            {
                var model = compilation.GetSemanticModel(declaration.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(declaration);
                if (symbol == null)
                    continue; // TODO: Diagnostic
                var @namespace = symbol.ContainingNamespace;

                var methodDeclaration = MethodDeclaration(
                                            GenericName(Identifier("IEnumerable")).WithTypeArgumentList(
                                                TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies")))
                                            ),
                                            Identifier(importConfiguration.MethodName)
                                        )
                                       .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                                       .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList(
                                                    Parameter(Identifier("serviceProvider")).WithType(IdentifierName("IServiceProvider"))
                                                )
                                            )
                                        )
                                       .WithLeadingTrivia(GetXmlSummary("The conventions imported into this assembly"));
                if (importConfiguration.Assembly)
                {
                    methodDeclaration = methodDeclaration
                                       .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName(importConfiguration.ClassName),
                                                            IdentifierName(importConfiguration.MethodName)
                                                        )
                                                    )
                                                   .WithArgumentList(
                                                        ArgumentList(SingletonSeparatedList(Argument(IdentifierName("serviceProvider"))))
                                                    )
                                            )
                                        )
                                       .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                }
                else
                {
                    methodDeclaration = methodDeclaration
                       .WithBody(functionBody);
                }

                var derivedClass = ClassDeclaration(declaration.Identifier)
                                  .WithModifiers(declaration.Modifiers)
                                  .WithConstraintClauses(declaration.ConstraintClauses)
                                  .WithTypeParameterList(declaration.TypeParameterList)
                                  .WithMembers(SingletonList<MemberDeclarationSyntax>(methodDeclaration))
                                  .NormalizeWhitespace();
                cu = cu
                   .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            NamespaceDeclaration(ParseName(@namespace.ToDisplayString()))
                               .WithMembers(SingletonList<MemberDeclarationSyntax>(derivedClass))
                        )
                    );
            }

            context.AddSource(
                "Imported_Class_Conventions.cs",
                cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
            );
        }

        static void addAssemblySource(SourceProductionContext context, BlockSyntax syntax, ConventionConfigurationData configurationData)
        {
            var members =
                ClassDeclaration(configurationData.ClassName)
                   .WithAttributeLists(
                        SingletonList(
                            AttributeList(
                                    SingletonSeparatedList(Attribute(ParseName("System.Runtime.CompilerServices.CompilerGenerated")))
                                )
                               .WithLeadingTrivia(GetXmlSummary("The class defined for importing conventions into this assembly"))
                        )
                    )
                   .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)
                        )
                    )
                   .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            MethodDeclaration(
                                    GenericName(Identifier("IEnumerable")).WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies"))
                                        )
                                    ),
                                    Identifier(configurationData.MethodName)
                                )
                               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                               .WithParameterList(
                                    ParameterList(
                                        SingletonSeparatedList(
                                            Parameter(Identifier("serviceProvider")).WithType(IdentifierName("IServiceProvider"))
                                        )
                                    )
                                )
                               .WithBody(syntax)
                               .WithLeadingTrivia(GetXmlSummary("The conventions imported into this assembly"))
                        )
                    );
            var cu = CompilationUnit()
                    .WithAttributeLists(configurationData.ToAttributes("Imports"))
                    .WithUsings(
                         List(
                             new[]
                             {
                                 UsingDirective(ParseName("System")),
                                 UsingDirective(ParseName("System.Collections.Generic")),
                                 UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                             }
                         )
                     );
            if (configurationData.Assembly)
            {
                cu = cu
                   .AddMembers(
                        string.IsNullOrWhiteSpace(configurationData.Namespace)
                            ? members
                            : NamespaceDeclaration(ParseName(configurationData.Namespace!)).AddMembers(members)
                    );
            }

            context.AddSource(
                "Imported_Assembly_Conventions.cs",
                cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
            );
        }

        static IReadOnlyCollection<string> getReferences(Compilation compilation, bool exports, ConventionConfigurationData configurationData)
        {
            return compilation.References
                              .Select(compilation.GetAssemblyOrModuleSymbol)
                              .OfType<IAssemblySymbol>()
                              .Select(
                                   symbol =>
                                   {
                                       try
                                       {
                                           var data = ConventionConfigurationData.FromAssemblyAttributes(symbol, _exportsDefaultConfiguration);
                                           var configuredMetadata =
                                               string.IsNullOrWhiteSpace(data.Namespace)
                                                   ? symbol.GetTypeByMetadataName(data.ClassName)
                                                   : symbol.GetTypeByMetadataName($"{data.Namespace}.Conventions.{data.ClassName}")
                                                  ?? symbol.GetTypeByMetadataName($"{data.Namespace}.{data.ClassName}");
                                           if (configuredMetadata is { })
                                           {
                                               return configuredMetadata.ToDisplayString() + $".{data.MethodName}";
                                           }

                                           var legacyMetadata = symbol.GetTypeByMetadataName($"{symbol.Name}.Conventions.Exports")
                                                             ?? symbol.GetTypeByMetadataName($"{symbol.Name}.Exports")
                                                             ?? symbol.GetTypeByMetadataName($"{symbol.Name}.__conventions__.__exports__");
                                           if (legacyMetadata is { })
                                           {
                                               return legacyMetadata.ToDisplayString() + ".GetConventions";
                                           }

                                           return null!;
                                       }
                                       catch
                                       {
                                           return null!;
                                       }
                                   }
                               )
                              .Where(z => !string.IsNullOrWhiteSpace(z))
                              .Concat(
                                   exports
                                       ? new[]
                                       {
                                           ( string.IsNullOrWhiteSpace(configurationData.Namespace) ? "" : configurationData.Namespace + "." )
                                         + configurationData.ClassName + "." + configurationData.MethodName
                                       }
                                       : Enumerable.Empty<string>()
                               )
                              .ToArray();
        }

        static BlockSyntax addEnumerateExportStatements(IReadOnlyCollection<string> references)
        {
            var block = Block();
            foreach (var reference in references)
            {
                block = block.AddStatements(
                    ForEachStatement(
                            IdentifierName("var"),
                            Identifier("convention"),
                            InvocationExpression(ParseExpression(reference))
                               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("serviceProvider"))))),
                            YieldStatement(SyntaxKind.YieldReturnStatement, IdentifierName("convention"))
                        )
                       .NormalizeWhitespace()
                );
            }

            return block;
        }
    }

    private static void HandleConventionExports(
        SourceProductionContext context,
        ConventionAttributeData data,
        ImmutableArray<INamedTypeSymbol> conventions,
        ImmutableArray<INamedTypeSymbol> exportedConventions
    )
    {
        if (!conventions.Any()) return;

        var helperClassBody = Block();

        foreach (var convention in conventions)
        {
            if (convention.Constructors.Length > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConventionHasToManyConstructors, convention.Locations.FirstOrDefault()));
                continue;
            }

            if (convention.IsGenericType)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConventionCannotBeGeneric, convention.Locations.FirstOrDefault()));
                continue;
            }

            var createConvention =
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("ActivatorUtilities"),
                            GenericName(Identifier("CreateInstance"))
                               .WithTypeArgumentList(
                                    TypeArgumentList(SingletonSeparatedList<TypeSyntax>(ParseName(convention.ToDisplayString())))
                                )
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(IdentifierName("serviceProvider"))
                            )
                        )
                    );

            var attributes = convention.GetAttributes();
            var hostType = HostTypeUndefined;
            var dependencies = new List<(MemberAccessExpressionSyntax direction, TypeSyntax type)>();
            foreach (var attributeData in attributes)
            {
                if (attributeData.AttributeClass == null)
                    continue;
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, data.BeforeConventionAttribute)
                 || SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, data.DependentOfConventionAttribute))
                {
                    // throw diagnostic if this is wrong?
                    if (attributeData.ConstructorArguments.Length is 1 && attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Type
                                                                       && attributeData.ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                    {
                        dependencies.Add(( DependencyDirectionDependentOf, ParseName(namedTypeSymbol.ToDisplayString()) ));
                    }
                }

                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, data.AfterConventionAttribute)
                 || SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, data.DependsOnConventionAttribute))
                {
                    // throw diagnostic if this is wrong?
                    if (attributeData.ConstructorArguments.Length is 1 && attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Type
                                                                       && attributeData.ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                    {
                        dependencies.Add(( DependencyDirectionDependsOn, ParseName(namedTypeSymbol.ToDisplayString()) ));
                    }
                }

                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, data.UnitTestConventionAttribute))
                {
                    hostType = HostTypeUnitTestHost;
                }

                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, data.LiveConventionAttribute))
                {
                    hostType = HostTypeLive;
                }
            }

            ExpressionSyntax withDependencies = ObjectCreationExpression(IdentifierName("ConventionWithDependencies"))
               .WithArgumentList(
                    ArgumentList(
                        SeparatedList(
                            new[]
                            {
                                Argument(createConvention), Argument(hostType)
                            }
                        )
                    )
                );

            foreach (var (direction, type) in dependencies)
            {
                withDependencies = InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            withDependencies,
                            IdentifierName("WithDependency")
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                new[]
                                {
                                    Argument(direction),
                                    Argument(TypeOfExpression(type))
                                }
                            )
                        )
                    );
            }


            helperClassBody = helperClassBody.AddStatements(
                YieldStatement(
                    SyntaxKind.YieldReturnStatement,
                    withDependencies
                )
            );
        }

        var helperClass =
            ClassDeclaration(data.Configuration.ClassName)
               .WithAttributeLists(
                    SingletonList(
                        AttributeList(SingletonSeparatedList(Attribute(ParseName("System.Runtime.CompilerServices.CompilerGenerated"))))
                           .WithLeadingTrivia(GetXmlSummary("The class defined for exporting conventions from this assembly"))
                    )
                )
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)))
               .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        MethodDeclaration(
                                GenericName(Identifier("IEnumerable")).WithTypeArgumentList(
                                    TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies")))
                                ),
                                data.Configuration.MethodName
                            )
                           .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                           .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList(
                                        Parameter(Identifier("serviceProvider")).WithType(IdentifierName("IServiceProvider"))
                                    )
                                )
                            )
                           .WithBody(helperClassBody)
                           .WithLeadingTrivia(GetXmlSummary("The conventions exports from this assembly"))
                    )
                );

        var cu = CompilationUnit()
                .WithUsings(
                     List(
                         new[]
                         {
                             UsingDirective(ParseName("System")),
                             UsingDirective(ParseName("System.Collections.Generic")),
                             UsingDirective(ParseName("Microsoft.Extensions.DependencyInjection")),
                             UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                         }
                     )
                 )
                .WithAttributeLists(data.Configuration.ToAttributes("Exports"))
                .AddAttributeLists(
                     AttributeList(
                             SingletonSeparatedList(
                                 Attribute(IdentifierName("ExportedConventions"))
                                    .WithArgumentList(
                                         AttributeArgumentList(
                                             SeparatedList(
                                                 conventions
                                                    .Select(symbol => AttributeArgument(TypeOfExpression(ParseName(symbol.ToDisplayString()))))
                                             )
                                         )
                                     )
                             )
                         )
                        .WithTarget(AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword)))
                 );

        if (data.Configuration.Assembly)
        {
            cu = cu.AddMembers(
                string.IsNullOrWhiteSpace(data.Namespace) ? helperClass : NamespaceDeclaration(ParseName(data.Namespace)).AddMembers(helperClass)
            );
        }

        if (exportedConventions.Length > 0)
        {
            cu = cu.AddAttributeLists(
                exportedConventions.Select(
                    candidate => AttributeList(
                            SingletonSeparatedList(
                                Attribute(IdentifierName("Convention"))
                                   .WithArgumentList(
                                        AttributeArgumentList(
                                            SingletonSeparatedList(
                                                AttributeArgument(
                                                    TypeOfExpression(
                                                        ParseName(candidate.ToDisplayString())
                                                    )
                                                )
                                            )
                                        )
                                    )
                            )
                        )
                       .WithTarget(
                            AttributeTargetSpecifier(
                                Token(SyntaxKind.AssemblyKeyword)
                            )
                        )
                ).ToArray()
            );
        }

        context.AddSource(
            "Exported_Conventions.cs",
            cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
        );
    }

    private static IEnumerable<INamedTypeSymbol> GetExportedConventions(GeneratorExecutionContext context, BaseTypeDeclarationSyntax declarationSyntax)
    {
        var model = context.Compilation.GetSemanticModel(declarationSyntax.SyntaxTree);
        if (model.GetDeclaredSymbol(declarationSyntax) is { } symbol)
            yield return symbol;
    }

    private static IEnumerable<INamedTypeSymbol> GetExportedConventions(GeneratorExecutionContext context, AttributeListSyntax attributeListSyntax)
    {
        var model = context.Compilation.GetSemanticModel(attributeListSyntax.SyntaxTree);
        var conventionAttribute = context.Compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.ConventionAttribute")!;
        foreach (var attribute in attributeListSyntax.Attributes)
        {
            if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count is 0 or > 1)
                continue;

            var nameInfo = model.GetTypeInfo(attribute.Name);
            if (nameInfo.Type?.ToDisplayString() != conventionAttribute.ToDisplayString())
                continue;

            var arg = attribute.ArgumentList.Arguments.Single();
            if (!( arg.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax ))
                continue;

            var symbol = model.GetTypeInfo(typeOfExpressionSyntax.Type);
            if (symbol.Type == null)
                continue;

            yield return context.Compilation.GetTypeByMetadataName(GetFullMetadataName(symbol.Type))!;
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetExportedConventions(
        AttributeListSyntax attributeListSyntax, SemanticModel model, Compilation compilation, INamedTypeSymbol conventionAttribute
    )
    {
        foreach (var attribute in attributeListSyntax.Attributes)
        {
            if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count is 0 or > 1)
                continue;

            var nameInfo = model.GetTypeInfo(attribute.Name);
            if (nameInfo.Type?.ToDisplayString() != conventionAttribute.ToDisplayString())
                continue;

            var arg = attribute.ArgumentList.Arguments.Single();
            if (!( arg.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax ))
                continue;

            var symbol = model.GetTypeInfo(typeOfExpressionSyntax.Type);
            if (symbol.Type == null)
                continue;

            yield return compilation.GetTypeByMetadataName(GetFullMetadataName(symbol.Type))!;
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetExportedConventions(BaseTypeDeclarationSyntax declarationSyntax, SemanticModel model)
    {
        if (model.GetDeclaredSymbol(declarationSyntax) is { } symbol)
            yield return symbol;
    }


    private static readonly MemberAccessExpressionSyntax HostTypeUndefined = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("HostType"),
        IdentifierName("Undefined")
    );

    private static readonly MemberAccessExpressionSyntax HostTypeLive = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("HostType"),
        IdentifierName("Live")
    );

    private static readonly MemberAccessExpressionSyntax HostTypeUnitTestHost = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("HostType"),
        IdentifierName("UnitTest")
    );

    private static readonly MemberAccessExpressionSyntax DependencyDirectionDependsOn = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("DependencyDirection"),
        IdentifierName("DependsOn")
    );

    private static readonly MemberAccessExpressionSyntax DependencyDirectionDependentOf = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("DependencyDirection"),
        IdentifierName("DependentOf")
    );

    internal static string GetFullMetadataName(ISymbol? s)
    {
        if (s == null || IsRootNamespace(s))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!IsRootNamespace(s))
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

        static bool IsRootNamespace(ISymbol symbol)
        {
            INamespaceSymbol? s;
            return ( s = symbol as INamespaceSymbol ) != null && s.IsGlobalNamespace;
        }
    }
}
