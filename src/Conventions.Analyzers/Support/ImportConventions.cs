using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Rocket.Surgery.Conventions.Helpers;

namespace Rocket.Surgery.Conventions.Support;

internal static class ImportConventions
{
    public static void HandleConventionImports(
        SourceProductionContext context,
        Request request
    )
    {
        var references = getReferences(request.Compilation, request is { HasExports: true, ExportConfiguration.Assembly: true, }, request.ExportConfiguration);

        var functionBody = references.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(references);

        addAssemblySource(context, request.Compilation, functionBody, request.ImportConfiguration, request.IsTestProject);

        static void addAssemblySource(
            SourceProductionContext context,
            Compilation compilation,
            BlockSyntax syntax,
            ConventionConfigurationData configurationData,
            bool referencesXUnit
        )
        {
            var importsClass =
                ClassDeclaration(configurationData.ClassName)
                   .WithAttributeLists(
                        SingletonList(
                            CompilerGeneratedAttributes
                               .WithLeadingTrivia(GetXmlSummary("The class defined for importing conventions into this assembly"))
                        )
                    )
                   .AddBaseListTypes(SimpleBaseType(IdentifierName("IConventionFactory")))
                   .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.InternalKeyword),
                            Token(SyntaxKind.SealedKeyword),
                            Token(SyntaxKind.PartialKeyword)
                        )
                    )
                   .AddMembers(
                        PropertyDeclaration(
                                IdentifierName("IConventionFactory"),
                                Identifier(configurationData.MethodName)
                            )
                           .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword)
                                )
                            )
                           .WithAccessorList(
                                AccessorList(
                                    SingletonList(
                                        AccessorDeclaration(
                                                SyntaxKind.GetAccessorDeclaration
                                            )
                                           .WithSemicolonToken(
                                                Token(SyntaxKind.SemicolonToken)
                                            )
                                    )
                                )
                            )
                           .WithInitializer(
                                EqualsValueClause(
                                    InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ObjectCreationExpression(IdentifierName(configurationData.ClassName)).WithArgumentList(ArgumentList()),
                                                IdentifierName("OrCallerConventions")
                                            )
                                        )
                                       .WithArgumentList(ArgumentList())
                                )
                            )
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        MethodDeclaration(
                                GenericName(Identifier("IEnumerable"))
                                   .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionWithDependencies"))
                                        )
                                    ),
                                Identifier("LoadConventions")
                            )
                           .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                           .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList(
                                        Parameter(Identifier("builder")).WithType(IdentifierName("ConventionContextBuilder"))
                                    )
                                )
                            )
                           .WithBody(syntax)
                           .WithLeadingTrivia(GetXmlSummary("The conventions imported into this assembly"))
                    );


            if (referencesXUnit)
                importsClass = importsClass.AddMembers(
                    MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Init"))
                       .WithAttributeLists(
                            SingletonList(
                                AttributeList(
                                    SeparatedList(
                                        [
                                            Attribute(ParseName("System.Runtime.CompilerServices.ModuleInitializer")),
                                            Attribute(ParseName("System.ComponentModel.EditorBrowsable"))
                                               .WithArgumentList(
                                                    AttributeArgumentList(
                                                        SingletonSeparatedList(
                                                            AttributeArgument(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    ParseName("System.ComponentModel.EditorBrowsableState"),
                                                                    IdentifierName("Never")
                                                                )
                                                            )
                                                        )
                                                    )
                                                ),
                                        ]
                                    )
                                )
                            )
                        )
                       .WithModifiers(
                            TokenList(
                                [
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword),
                                ]
                            )
                        )
                       .WithBody(
                            Block(
                                SingletonList<StatementSyntax>(
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("ImportHelpers"),
                                                IdentifierName("ExternalConventions")
                                            ),
                                            IdentifierName(configurationData.MethodName)
                                        )
                                    )
                                )
                            )
                        )
                );

            var cu = CompilationUnit()
                    .WithAttributeLists(configurationData.ToAttributes("Imports"))
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
                    .WithUsings(
                         List(
                             new[]
                             {
                                 UsingDirective(ParseName("System")),
                                 UsingDirective(ParseName("System.Collections.Generic")),
                                 UsingDirective(ParseName("System.Runtime.Loader")),
                                 UsingDirective(ParseName("Microsoft.Extensions.DependencyInjection")),
                                 UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                             }
                         )
                     );
            var members = new List<MemberDeclarationSyntax>();
            members.Add(importsClass);
            if (configurationData is { Assembly: true, })
            {
                cu = cu
                   .AddMembers(
                        configurationData is { Namespace: { Length: > 0, } relativeNamespace, }
                            ? [NamespaceDeclaration(ParseName(relativeNamespace)).AddMembers(members.ToArray()),]
                            : members.ToArray()
                    );

                if (compilation.GetTypeByMetadataName("Rocket.Surgery.Hosting.RocketHostApplicationExtensions") is { })
                {
                    if (compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Builder.WebApplicationBuilder") is { })
                        context.AddSource(
                            "Generated_WebApplicationBuilder_Extensions.cs",
                            _configurationMethods
                               .Replace("{BuilderType}", "Microsoft.AspNetCore.Builder.WebApplicationBuilder")
                               .Replace("{ReturnType}", "Microsoft.AspNetCore.Builder.WebApplication")
                        );

                    if (compilation.GetTypeByMetadataName("Microsoft.Extensions.Hosting.HostApplicationBuilder") is { })
                        context.AddSource(
                            "Generated_HostApplicationBuilder_Extensions.cs",
                            _configurationMethods
                               .Replace("{BuilderType}", "Microsoft.Extensions.Hosting.HostApplicationBuilder")
                               .Replace("{ReturnType}", "Microsoft.Extensions.Hosting.IHost")
                        );
                }
            }

            context.AddSource(
                "Imported_Assembly_Conventions.cs",
                cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
            );
        }

        static IReadOnlyCollection<string> getReferences(Compilation compilation, bool exports, ConventionConfigurationData configurationData)
        {
            return compilation
                  .References
                  .Select(compilation.GetAssemblyOrModuleSymbol)
                  .OfType<IAssemblySymbol>()
                  .Select(
                       symbol =>
                       {
                           try
                           {
                               var data = ConventionConfigurationData.FromAssemblyAttributes(symbol, ConventionConfigurationData.ExportsDefaults);
                               var configuredMetadata =
                                   string.IsNullOrWhiteSpace(data.Namespace)
                                       ? symbol.GetTypeByMetadataName(data.ClassName)
                                       : symbol.GetTypeByMetadataName($"{data.Namespace}.{data.ClassName}");
                               if (configuredMetadata is { }) return configuredMetadata.ToDisplayString() + $".{data.MethodName}";

                               // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                               return null!;
                           }
                           catch
                           {
                               // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
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
                             + configurationData.ClassName
                             + "."
                             + configurationData.MethodName,
                           }
                           : Enumerable.Empty<string>()
                   )
                  .OrderBy(z => z)
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
                               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("builder"))))),
                            YieldStatement(SyntaxKind.YieldReturnStatement, IdentifierName("convention"))
                        )
                       .NormalizeWhitespace()
                );
            }

            return block;
        }
    }

    private static readonly string _configurationMethods = """"
        using Microsoft.Extensions.Configuration;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Hosting;
        using Microsoft.Extensions.Logging;
        using Rocket.Surgery.Conventions;
        using AppDelegate =
            System.Func<{BuilderType}, System.Threading.CancellationToken,
                System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

        namespace Rocket.Surgery.Hosting;

        internal static partial class GeneratedRocketHostApplicationExtensions
        {
            /// <summary>
            ///     Uses the rocket booster.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> UseRocketBooster(
                this {BuilderType} builder,
                AppDelegate func,
                Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
                CancellationToken cancellationToken = default
            )
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(func);
                var b = await func(builder, cancellationToken);
                await action.Invoke(b, cancellationToken);
                await RocketHostApplicationExtensions.Configure(builder, b, cancellationToken);
                return builder.Build();
            }
        
            /// <summary>
            ///     Uses the rocket booster.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> UseRocketBooster(
                this {BuilderType} builder,
                AppDelegate func,
                Func<ConventionContextBuilder, ValueTask> action,
                CancellationToken cancellationToken = default
            )
            {
                return UseRocketBooster(
                    builder,
                    func,
                    (b, _) => action.Invoke(b),
                    cancellationToken
                );
            }
        
            /// <summary>
            ///     Uses the rocket booster.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> UseRocketBooster(
                this {BuilderType} builder,
                AppDelegate func,
                Action<ConventionContextBuilder> action,
                CancellationToken cancellationToken = default
            )
            {
                return UseRocketBooster(
                    builder,
                    func,
                    (b, _) =>
                    {
                        action.Invoke(b);
                        return ValueTask.CompletedTask;
                    },
                    cancellationToken
                );
            }
        
            /// <summary>
            ///     Uses the rocket booster.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> UseRocketBooster(
                this {BuilderType} builder,
                AppDelegate func,
                CancellationToken cancellationToken = default
            )
            {
                return UseRocketBooster(builder, func, (_, _) => ValueTask.CompletedTask, cancellationToken);
            }
        
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> LaunchWith(
                this {BuilderType} builder,
                AppDelegate func,
                Action<ConventionContextBuilder> action,
                CancellationToken cancellationToken = default
            )
            {
                return UseRocketBooster(builder, func, action, cancellationToken);
            }
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> LaunchWith(
                this {BuilderType} builder,
                AppDelegate func,
                Func<ConventionContextBuilder, ValueTask> action,
                CancellationToken cancellationToken = default
            )
            {
                return UseRocketBooster(builder, func, action, cancellationToken);
            }
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> LaunchWith(
                this {BuilderType} builder,
                AppDelegate func,
                Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
                CancellationToken cancellationToken = default
            )
            {
                return UseRocketBooster(builder, func, action, cancellationToken);
            }
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> LaunchWith(this {BuilderType} builder, AppDelegate func, CancellationToken cancellationToken)
            {
                return UseRocketBooster(builder, func, cancellationToken);
            }
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            public static ValueTask<{ReturnType}> LaunchWith(this {BuilderType} builder, AppDelegate func)
            {
                return UseRocketBooster(builder, func, CancellationToken.None);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, CancellationToken cancellationToken = default)
            {
                return ConfigureRocketSurgery(builder, _ => { }, cancellationToken);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(
                this {BuilderType} builder,
                Action<ConventionContextBuilder> action,
                CancellationToken cancellationToken = default
            )
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = RocketHostApplicationExtensions.GetExisting(builder);
                action(contextBuilder);
                await RocketHostApplicationExtensions.Configure(builder, contextBuilder, cancellationToken);
                return builder.Build();
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(
                this {BuilderType} builder,
                Func<ConventionContextBuilder, ValueTask> action,
                CancellationToken cancellationToken = default
            )
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = RocketHostApplicationExtensions.GetExisting(builder);
                await action(contextBuilder);
                await RocketHostApplicationExtensions.Configure(builder, contextBuilder, cancellationToken);
                return builder.Build();
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(
                this {BuilderType} builder,
                Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
                CancellationToken cancellationToken = default
            )
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = RocketHostApplicationExtensions.GetExisting(builder);
                await action(contextBuilder, cancellationToken);
                await RocketHostApplicationExtensions.Configure(builder, contextBuilder, cancellationToken);
                return builder.Build();
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="getConventions">The method to get the conventions.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(
                this {BuilderType} builder,
                IConventionFactory getConventions,
                CancellationToken cancellationToken = default
            )
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(getConventions);
                var contextBuilder = RocketHostApplicationExtensions.GetExisting(builder).UseConventionFactory(getConventions);
                await RocketHostApplicationExtensions.Configure(builder, contextBuilder, cancellationToken);
                return builder.Build();
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="conventionContextBuilder">The convention context builder.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(
                this {BuilderType} builder,
                ConventionContextBuilder conventionContextBuilder,
                CancellationToken cancellationToken = default
            )
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(conventionContextBuilder);
                await RocketHostApplicationExtensions.Configure(builder, conventionContextBuilder, cancellationToken);
                return builder.Build();
            }
        }
        """";

    public record Request
    (
        Compilation Compilation,
        bool HasExports,
        bool IsTestProject,
        ConventionConfigurationData ImportConfiguration,
        ConventionConfigurationData ExportConfiguration
    );
}