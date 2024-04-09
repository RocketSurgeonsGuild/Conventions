using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Rocket.Surgery.Conventions.Helpers;

namespace Rocket.Surgery.Conventions.Support;

internal static class ExportConventions
{
    public record Request(ConventionAttributeData Data, ImmutableArray<INamedTypeSymbol> Conventions, ImmutableArray<INamedTypeSymbol> ExportedConventions);

    public static void HandleConventionExports(SourceProductionContext context, Request request)
    {
        var (data, conventions, exportedConventions) = request;
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

            var createConvention = NewConventionOrActivate(convention);

            var attributes = convention.GetAttributes();
            var hostType = _hostTypeUndefined;
            var dependencies = new List<(MemberAccessExpressionSyntax direction, TypeSyntax type)>();
            foreach (var attributeData in attributes)
            {
                switch (attributeData)
                {
                    case
                    {
                        AttributeClass:
                        {
                            Name: "DependentOfConventionAttribute" or "BeforeConventionAttribute",
                            TypeArguments: [INamedTypeSymbol dependencyDirectionDependentOfSymbol,],
                        },
                    }:
                        dependencies.Add(( _dependencyDirectionDependentOf, ParseName(dependencyDirectionDependentOfSymbol.ToDisplayString()) ));
                        break;
                    case
                    {
                        AttributeClass.Name: "DependentOfConventionAttribute" or "BeforeConventionAttribute",
                        ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol dependencyDirectionDependentOfSymbol2, },],
                    }:
                        dependencies.Add(( _dependencyDirectionDependentOf, ParseName(dependencyDirectionDependentOfSymbol2.ToDisplayString()) ));
                        break;
                    case
                    {
                        AttributeClass:
                        {
                            Name: "DependsOnConventionAttribute" or "AfterConventionAttribute",
                            TypeArguments: [INamedTypeSymbol dependencyDirectionDependsOnSymbol,],
                        },
                    }:
                        dependencies.Add(( _dependencyDirectionDependsOn, ParseName(dependencyDirectionDependsOnSymbol.ToDisplayString()) ));
                        break;
                    case
                    {
                        AttributeClass.Name: "DependsOnConventionAttribute" or "AfterConventionAttribute",
                        ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol dependencyDirectionDependsOnSymbol2, },],
                    }:
                        dependencies.Add(( _dependencyDirectionDependsOn, ParseName(dependencyDirectionDependsOnSymbol2.ToDisplayString()) ));
                        break;
                }

                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, data.UnitTestConventionAttribute))
                {
                    hostType = _hostTypeUnitTestHost;
                }

                else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, data.LiveConventionAttribute))
                {
                    hostType = _hostTypeLive;
                }
            }

            ExpressionSyntax withDependencies = ObjectCreationExpression(IdentifierName("ConventionWithDependencies"))
                                                             .WithArgumentList(
                                                                  ArgumentList(
                                                                      SeparatedList(
                                                                          new[]
                                                                          {
                                                                              Argument(createConvention), Argument(hostType),
                                                                          }
                                                                      )
                                                                  )
                                                              );

            foreach (( var direction, var type ) in dependencies)
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
            ClassDeclaration(data.Configuration.ClassName)
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
                                                                      TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies")))
                                                                  ),
                                                    data.Configuration.MethodName
                                                )
                                               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                                               .WithParameterList(
                                                    ParameterList(
                                                        SingletonSeparatedList(
                                                            Parameter(Identifier("serviceProvider")).WithType(IdentifierName("IServiceProviderDictionary"))
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
                data is { Namespace.Length: > 0, } ? NamespaceDeclaration(ParseName(data.Namespace)).AddMembers(helperClass) : helperClass
            );
        }

        if (exportedConventions.Length > 0)
        {
            cu = cu.AddAttributeLists(
                exportedConventions
                   .Select(
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
                    )
                   .ToArray()
            );
        }

        context.AddSource(
            "Exported_Conventions.cs",
            cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
        );
    }

    private static ExpressionSyntax NewConventionOrActivate(INamedTypeSymbol convention)
    {
        if (convention.Constructors.Length is 0)
        {
            return ObjectCreationExpression(ParseName(convention.ToDisplayString()));
        }

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
                                IdentifierName("serviceProvider"),
                                GenericName("GetService")
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
    }
    private static readonly MemberAccessExpressionSyntax _hostTypeUndefined = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("HostType"),
        IdentifierName("Undefined")
    );

    private static readonly MemberAccessExpressionSyntax _hostTypeLive = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("HostType"),
        IdentifierName("Live")
    );

    private static readonly MemberAccessExpressionSyntax _hostTypeUnitTestHost = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("HostType"),
        IdentifierName("UnitTest")
    );

    private static readonly MemberAccessExpressionSyntax _dependencyDirectionDependsOn = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("DependencyDirection"),
        IdentifierName("DependsOn")
    );

    private static readonly MemberAccessExpressionSyntax _dependencyDirectionDependentOf = MemberAccessExpression(
        SyntaxKind.SimpleMemberAccessExpression,
        IdentifierName("DependencyDirection"),
        IdentifierName("DependentOf")
    );


}
