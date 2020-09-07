using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions
{
    // TODO: analyzers
    //

    [Generator]
    public class ConventionAttributesGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ConventionAttributeSyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context)
        {
            if (!( context.SyntaxReceiver is ConventionAttributeSyntaxReceiver syntaxReceiver ))
            {
                return;
            }

            var compliation = ( context.Compilation as CSharpCompilation )!;
            var hasExports = false;

            if (syntaxReceiver.ExportCandidates.Count > 0)
            {
                hasExports = HandleConventionExports(context, compliation, syntaxReceiver.ExportCandidates);
            }

            if (syntaxReceiver.ImportCandidates.Count > 0)
            {
                HandleConventionImports(context, compliation, syntaxReceiver.ImportCandidates, hasExports);
            }
        }

        private static string GetNamespaceForCompilation(Compilation compilation) => ( ( compilation.AssemblyName ?? "" ) + ".__conventions__" ).TrimStart('.');

        private void HandleConventionImports(SourceGeneratorContext context, CSharpCompilation compilation, IReadOnlyCollection<SyntaxNode> importCandidates, bool hasExports)
        {
            if (importCandidates.OfType<AttributeListSyntax>().Any())
            {
                var references = getReferences(compilation, hasExports);
                var block = references.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(references);
                addAssemblySource(context, GetNamespaceForCompilation(compilation), block);
            }
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
                var references = getReferences(compilation, hasExports);
                var block = references.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(references);
                foreach (var declaration in importCandidates.OfType<ClassDeclarationSyntax>())
                {
                    var model = compilation.GetSemanticModel(declaration.SyntaxTree);
                    var symbol = model.GetDeclaredSymbol(declaration);
                    if (symbol == null)
                        continue; // TODO: Diagnostic
                    var @namespace = symbol.ContainingNamespace;

                    var derivedClass = ClassDeclaration(declaration.Identifier)
                           .WithModifiers(declaration.Modifiers)
                           .WithConstraintClauses(declaration.ConstraintClauses)
                           .WithTypeParameterList(declaration.TypeParameterList)
                           .WithMembers(createConventionsMemberDeclaration(block))
                           .NormalizeWhitespace()
                        ;
                    cu = cu.WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            NamespaceDeclaration(ParseName(@namespace.ToDisplayString()))
                               .WithMembers(SingletonList<MemberDeclarationSyntax>(derivedClass))
                        )
                    );
                }

                context.AddSource(
                    $"Imported_Class_Conventions.cs",
                    cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }

            static void addAssemblySource(SourceGeneratorContext context, string @namespace, BlockSyntax syntax)
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
                    )
                   .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            NamespaceDeclaration(ParseName(@namespace))
                               .WithMembers(
                                    SingletonList<MemberDeclarationSyntax>(
                                        ClassDeclaration("__imports__")
                                           .WithAttributeLists(
                                                SingletonList(AttributeList(SingletonSeparatedList(Attribute(ParseName("System.Runtime.CompilerServices.CompilerGenerated")))))
                                            )
                                           .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                                           .WithMembers(createConventionsMemberDeclaration(syntax))
                                    )
                                )
                        )
                    )
                   .NormalizeWhitespace();

                context.AddSource(
                    $"Imported_Assembly_Conventions.cs",
                    cu.SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }

            static IReadOnlyCollection<string> getReferences(CSharpCompilation compilation, bool exports) => compilation.References
               .Select(compilation.GetAssemblyOrModuleSymbol)
               .OfType<IAssemblySymbol>()
               .Where(z => z.TypeNames.Contains("__exports__"))
               .Select(symbol => symbol.GetTypeByMetadataName($"{symbol.Name}.__conventions__.__exports__")!.ToDisplayString())
               .Concat(exports ? new[] { GetNamespaceForCompilation(compilation) + ".__exports__" } : Enumerable.Empty<string>())
               .ToArray();

            static BlockSyntax addEnumerateExportStatements(IReadOnlyCollection<string> references)
            {
                var block = Block();
                foreach (var reference in references)
                {
                    block = block.AddStatements(
                        ForEachStatement(
                                IdentifierName("var"),
                                Identifier("convention"),
                                InvocationExpression(ParseExpression(reference + ".GetConventions"))
                                   .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("serviceProvider"))))),
                                YieldStatement(SyntaxKind.YieldReturnStatement, IdentifierName("convention"))
                            )
                           .NormalizeWhitespace()
                    );
                }

                return block;
            }

            static SyntaxList<MemberDeclarationSyntax> createConventionsMemberDeclaration(BlockSyntax syntax) => SingletonList<MemberDeclarationSyntax>(
                MethodDeclaration(
                        GenericName(Identifier("IEnumerable")).WithTypeArgumentList(
                            TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies")))
                        ),
                        Identifier("GetConventions")
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
            );
        }

        private bool HandleConventionExports(SourceGeneratorContext context, CSharpCompilation compilation, IReadOnlyCollection<AttributeListSyntax> exportCandidates)
        {
            var conventions = exportCandidates
               .SelectMany(candidate => GetExportedConventions(context, candidate))
               .ToArray();

            if (!conventions.Any())
                return false;

            var @namespace = context.Compilation.AssemblyName ?? "";
            @namespace = ( @namespace + ".__conventions__" ).TrimStart('.');


            var liveConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.LiveConventionAttribute")!;
            var unitTestConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.UnitTestConventionAttribute")!;
            var afterConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.AfterConventionAttribute")!;
            var dependsOnConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.DependsOnConventionAttribute")!;
            var beforeConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.BeforeConventionAttribute")!;
            var dependentOfConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.DependentOfConventionAttribute")!;


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
                    if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, beforeConventionAttribute)
                     || SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, dependentOfConventionAttribute))
                    {
                        // throw diagnostic if this is wrong?
                        if (attributeData.ConstructorArguments.Length is 1 && attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Type
                         && attributeData.ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                        {
                            dependencies.Add(( DependencyDirectionDependentOf, ParseName(namedTypeSymbol.ToDisplayString()) ));
                        }
                    }

                    if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, afterConventionAttribute)
                     || SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, dependsOnConventionAttribute))
                    {
                        // throw diagnostic if this is wrong?
                        if (attributeData.ConstructorArguments.Length is 1 && attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Type
                         && attributeData.ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                        {
                            dependencies.Add(( DependencyDirectionDependsOn, ParseName(namedTypeSymbol.ToDisplayString()) ));
                        }
                    }

                    if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, unitTestConventionAttribute))
                    {
                        hostType = HostTypeUnitTestHost;
                    }

                    if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, liveConventionAttribute))
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
                    NamespaceDeclaration(ParseName(@namespace)).WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            ClassDeclaration("__exports__")
                               .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(ParseName("System.Runtime.CompilerServices.CompilerGenerated"))))))
                               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                               .WithMembers(
                                    SingletonList<MemberDeclarationSyntax>(
                                        MethodDeclaration(
                                                GenericName(Identifier("IEnumerable")).WithTypeArgumentList(
                                                    TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies")))
                                                ),
                                                "GetConventions"
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
                                    )
                                )
                        )
                    )
                ;

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
               .WithAttributeLists(
                    SingletonList(
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
                    )
                )
               .WithMembers(SingletonList<MemberDeclarationSyntax>(helperClass))
               .NormalizeWhitespace();

            context.AddSource(
                $"Exported_Conventions.cs",
                cu.SyntaxTree.GetRoot().GetText(Encoding.UTF8)
            );

            return true;
        }

        private IEnumerable<INamedTypeSymbol> GetExportedConventions(SourceGeneratorContext context, AttributeListSyntax attributeListSyntax)
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

                yield return context.Compilation.GetTypeByMetadataName(symbol.Type.ToDisplayString())!;
            }
        }


        static MemberAccessExpressionSyntax HostTypeUndefined = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("HostType"),
            IdentifierName("Undefined")
        );

        static MemberAccessExpressionSyntax HostTypeLive = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("HostType"),
            IdentifierName("Live")
        );

        static MemberAccessExpressionSyntax HostTypeUnitTestHost = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("HostType"),
            IdentifierName("UnitTest")
        );

        static MemberAccessExpressionSyntax DependencyDirectionDependsOn = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("DependencyDirection"),
            IdentifierName("DependsOn")
        );

        static MemberAccessExpressionSyntax DependencyDirectionDependentOf = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("DependencyDirection"),
            IdentifierName("DependentOf")
        );
    }
}