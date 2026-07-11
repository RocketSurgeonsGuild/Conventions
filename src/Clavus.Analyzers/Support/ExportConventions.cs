using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Clavus.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clavus.Support;

internal static class ExportConventions
{
    public static void HandleConventionExports(SourceProductionContext context, Request request)
    {
        (var msBuildConfig, var conventions) = request;

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

            var createConvention = NewConventionOrActivate(convention);

            var attributes = convention.GetAttributes();
            var hostType = _hostTypeUndefined;
            ExpressionSyntax conventionCategory = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("ClavusCategory"),
                IdentifierName("Application")
            );
            var dependencies = new List<(ExpressionSyntax direction, TypeSyntax type)>();
            foreach (var attributeData in attributes)
            {
                switch (attributeData)
                {
                    case
                    {
                        AttributeClass:
                        {
                            Name: "DependentOfPartAttribute" or "BeforePartAttribute",
                            TypeArguments: [INamedTypeSymbol dependencyDirectionDependentOfSymbol],
                        },
                    }:
                        dependencies.Add((_dependencyDirectionDependentOf, ParseName(dependencyDirectionDependentOfSymbol.ToDisplayString())));
                        break;
                    case
                    {
                        AttributeClass.Name: "DependentOfPartAttribute" or "BeforePartAttribute",
                        ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol dependencyDirectionDependentOfSymbol2 }],
                    }:
                        dependencies.Add((_dependencyDirectionDependentOf, ParseName(dependencyDirectionDependentOfSymbol2.ToDisplayString())));
                        break;
                    case
                    {
                        AttributeClass:
                        {
                            Name: "DependsOnPartAttribute" or "AfterPartAttribute",
                            TypeArguments: [INamedTypeSymbol dependencyDirectionDependsOnSymbol],
                        },
                    }:
                        dependencies.Add((_dependencyDirectionDependsOn, ParseName(dependencyDirectionDependsOnSymbol.ToDisplayString())));
                        break;
                    case
                    {
                        AttributeClass.Name: "DependsOnPartAttribute" or "AfterPartAttribute",
                        ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol dependencyDirectionDependsOnSymbol2 }],
                    }:
                        dependencies.Add((_dependencyDirectionDependsOn, ParseName(dependencyDirectionDependsOnSymbol2.ToDisplayString())));
                        break;
                    default:
                        break;
                }

                if (msBuildConfig.IsTestProject)
                {
                    hostType = _hostTypeUnitTestHost;
                }
            }

            ExpressionSyntax withDependencies = ObjectCreationExpression(IdentifierName("ClavusPartMetadata"))
               .WithArgumentList(
                    ArgumentList(
                        SeparatedList(
                            new[]
                            {
                                Argument(createConvention), Argument(hostType), Argument(conventionCategory),
                            }
                        )
                    )
                );

            foreach ((var direction, var type) in dependencies)
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
                                    Argument(TypeOfExpression(type)),
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
            ClassDeclaration(msBuildConfig.ExportConfiguration.ClassName)
               .WithAttributeLists(
                    SingletonList(
                        CompilerGeneratedAttributes
                           .WithLeadingTrivia(GetXmlSummary("The class defined for exporting conventions from this assembly"))
                    )
                )
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)))
               .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        MethodDeclaration(
                                GenericName(Identifier("IEnumerable"))
                                   .WithTypeArgumentList(
                                        TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IClavusPartMetadata")))
                                    ),
                                msBuildConfig.ExportConfiguration.MethodName
                            )
                           .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                           .WithParameterList(ParameterList())
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
                             UsingDirective(ParseName("Clavus")),
                             UsingDirective(ParseName("Clavus.Infrastructure")),
                         }
                     )
                 )
                .AddSharedTrivia()
                .WithAttributeLists(msBuildConfig.ExportConfiguration.ToAttributes());

        cu = cu.AddMembers(
            msBuildConfig.ExportConfiguration is { Namespace.Length: > 0 } data
             ? NamespaceDeclaration(ParseName(data.Namespace)).AddMembers(helperClass) : helperClass
        );

        context.AddSource(
            "Exported_Conventions.g.cs",
            cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
        );
    }

    private static ExpressionSyntax NewConventionOrActivate(INamedTypeSymbol convention)
    {
        if (convention.Constructors.Length is 0) return ObjectCreationExpression(ParseName(convention.ToDisplayString()));

        if (convention.Constructors.Count(z => z.DeclaredAccessibility is Accessibility.Internal or Accessibility.Public) == 1)
        {
            var constructor = convention.Constructors.First(z => z.DeclaredAccessibility is Accessibility.Internal or Accessibility.Public);
            var arguments = ArgumentList();
            foreach (var parameter in constructor.Parameters)
            {
                arguments = arguments.AddArguments(
                    Argument(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("builder"), IdentifierName("Properties")),
                                GenericName(Identifier("GetService"))
                                   .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                ParseName(parameter.Type.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString())
                                            )
                                        )
                                    )
                            )
                        )
                    )
                );
            }

            return ObjectCreationExpression(ParseName(convention.ToDisplayString()), arguments, null);
        }

        return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("ActivatorUtilities"),
                    GenericName(Identifier("CreateInstance"))
                       .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(ParseName(convention.ToDisplayString()))))
                )
            )
           .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("builder")))));
    }

    private static readonly ExpressionSyntax _hostTypeUndefined = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("HostType"),
        IdentifierName("Undefined")
    );
    private static readonly ExpressionSyntax _hostTypeUnitTestHost = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("HostType"),
        IdentifierName("UnitTest")
    );

    private static readonly ExpressionSyntax _dependencyDirectionDependsOn = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("DependencyDirection"),
        IdentifierName("DependsOn")
    );

    private static readonly ExpressionSyntax _dependencyDirectionDependentOf = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("DependencyDirection"),
        IdentifierName("DependentOf")
    );

    public record Request(
        MsBuildConfig BuildConfig,
        ImmutableArray<INamedTypeSymbol> ExportedConventions);
}
