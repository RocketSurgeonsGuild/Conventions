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
        var references = getReferences(request.Compilation, request is { HasExports: true, ExportConfiguration.Assembly: true }, request.ExportConfiguration);

        var functionBody = references.Count == 0 ? Block(YieldStatement(SyntaxKind.YieldBreakStatement)) : addEnumerateExportStatements(references);

        var compilation = request.Compilation;
        var importsClass =
            ClassDeclaration(request.ImportConfiguration.ClassName)
               .WithAttributeLists(
                    SingletonList(
                        CompilerGeneratedAttributes
                           .WithLeadingTrivia(GetXmlSummary("The class defined for importing conventions into this assembly"))
                    )
                )
               .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)))
               .AddMembers(
                    FieldDeclaration(
                            VariableDeclaration(IdentifierName("LoadConventions"))
                               .WithVariables(
                                    SingletonSeparatedList(
                                        VariableDeclarator(Identifier(request.ImportConfiguration.MethodName))
                                           .WithInitializer(EqualsValueClause(IdentifierName("LoadConventionsMethod")))
                                    )
                                )
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword))),
                    MethodDeclaration(
                            GenericName(Identifier("IEnumerable"))
                               .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(IdentifierName("IConventionMetadata"))
                                    )
                                ),
                            Identifier("LoadConventionsMethod")
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
                       .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList(
                                    Parameter(Identifier("builder")).WithType(IdentifierName("ConventionContextBuilder"))
                                )
                            )
                        )
                       .WithBody(functionBody)
                       .WithLeadingTrivia(GetXmlSummary("The conventions imported into this assembly"))
                )
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        if (request.MsBuildConfig.isTestProject)
        {
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
                   .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                   .WithBody(
                        Block(
                            List<StatementSyntax>(
                                [
                                    ExpressionStatement(
                                        InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("Environment"),
                                                    IdentifierName("SetEnvironmentVariable")
                                                )
                                            )
                                           .WithArgumentList(
                                                ArgumentList(
                                                    SeparatedList<ArgumentSyntax>(
                                                        [
                                                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("RSG__HOSTTYPE"))),
                                                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("UnitTest"))),
                                                        ]
                                                    )
                                                )
                                            )
                                    ),
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("ImportHelpers"),
                                                IdentifierName("ExternalConventions")
                                            ),
                                            IdentifierName(request.ImportConfiguration.MethodName)
                                        )
                                    ),
                                ]
                            )
                        )
                    )
            );
        }

        var cu = CompilationUnit()
                .WithAttributeLists(request.ImportConfiguration.ToAttributes("Imports"))
                .AddSharedTrivia()
                .WithUsings(
                     List(
                         [
                             UsingDirective(ParseName("System")),
                             UsingDirective(ParseName("System.Collections.Generic")),
                             UsingDirective(ParseName("System.Runtime.Loader")),
                             UsingDirective(ParseName("Microsoft.Extensions.DependencyInjection")),
                             UsingDirective(ParseName("Rocket.Surgery.Conventions")),
                         ]
                     )
                 );
        var members = new List<MemberDeclarationSyntax>
        {
            importsClass,
        };

        cu = cu
           .AddMembers(
                request.ImportConfiguration is { Namespace: { Length: > 0 } relativeNamespace }
                    ? [NamespaceDeclaration(ParseName(relativeNamespace)).AddMembers(members.ToArray())]
                    : [.. members]
            );

        context.AddSource(
            "Imported_Assembly_Conventions.g.cs",
            cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
        );

        if (request.ImportConfiguration is { Assembly: true })
        {
            var loadConventionsMethod = request.ImportConfiguration.Namespace is { Length: > 0 }
                ? $"global::{request.ImportConfiguration.Namespace}.{request.ImportConfiguration.ClassName}.{request.ImportConfiguration.MethodName}"
                : $"global::{request.ImportConfiguration.ClassName}.{request.ImportConfiguration.MethodName}";

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.Hosting.RocketHostApplicationExtensions") is { })
            {
                if (compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Builder.WebApplicationBuilder") is { })
                {
                    context.AddSource(
                        "Generated_WebApplicationBuilder_Extensions.g.cs",
                        _configurationMethods
                           .Replace("{BuilderName}", "WebApplicationBuilder")
                           .Replace("{BuilderType}", "global::Microsoft.AspNetCore.Builder.WebApplicationBuilder")
                           .Replace("{ReturnType}", "global::Microsoft.AspNetCore.Builder.WebApplication")
                           .Replace("{ExtensionsType}", "global::Rocket.Surgery.Hosting.RocketHostApplicationExtensions")
                           .Replace("{LoadConventions}", loadConventionsMethod)
                           .Replace("{HostingUsing}", "global::Microsoft.Extensions.Hosting")
                           .Replace("{RocketUsing}", "Rocket.Surgery.Hosting")
                    );
                }

                if (compilation.GetTypeByMetadataName("Microsoft.Extensions.Hosting.HostApplicationBuilder") is { })
                {
                    context.AddSource(
                        "Generated_HostApplicationBuilder_Extensions.g.cs",
                        _configurationMethods
                           .Replace("{BuilderName}", "HostApplicationBuilder")
                           .Replace("{BuilderType}", "global::Microsoft.Extensions.Hosting.HostApplicationBuilder")
                           .Replace("{ReturnType}", "global::Microsoft.Extensions.Hosting.IHost")
                           .Replace("{ExtensionsType}", "global::Rocket.Surgery.Hosting.RocketHostApplicationExtensions")
                           .Replace("{LoadConventions}", loadConventionsMethod)
                           .Replace("{HostingUsing}", "global::Microsoft.Extensions.Hosting")
                           .Replace("{RocketUsing}", "Rocket.Surgery.Hosting")
                    );
                }
            }

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.WebAssembly.Hosting.RocketWebAssemblyExtensions") is { }
             && compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder") is { })
            {
                context.AddSource(
                    "Generated_WebAssemblyBuilder_Extensions.g.cs",
                    _configurationMethods
                       .Replace("{BuilderName}", "WebAssemblyHostBuilder")
                       .Replace("{BuilderType}", "global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder")
                       .Replace("{ReturnType}", "global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost")
                       .Replace("{ExtensionsType}", "global::Rocket.Surgery.WebAssembly.Hosting.RocketWebAssemblyExtensions")
                       .Replace("{LoadConventions}", loadConventionsMethod)
                       .Replace("{HostingUsing}", "global::Microsoft.AspNetCore.Components.WebAssembly.Hosting")
                       .Replace("{RocketUsing}", "Rocket.Surgery.WebAssembly.Hosting")
                );
            }

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.Aspire.Hosting.RocketDistributedApplicationExtensions") is { }
             && compilation.GetTypeByMetadataName("Aspire.Hosting.IDistributedApplicationBuilder") is { })
            {
                context.AddSource(
                    "Generated_DistributedApplicationBuilder_Extensions.g.cs",
                    _configurationMethods
                       .Replace("{BuilderName}", "DistributedApplicationBuilder")
                       .Replace("{BuilderType}", "global::Aspire.Hosting.IDistributedApplicationBuilder")
                       .Replace("{ReturnType}", "global::Aspire.Hosting.DistributedApplication")
                       .Replace("{ExtensionsType}", "global::Rocket.Surgery.Aspire.Hosting.RocketDistributedApplicationExtensions")
                       .Replace("{LoadConventions}", loadConventionsMethod)
                       .Replace("{HostingUsing}", "global::Aspire.Hosting")
                       .Replace("{RocketUsing}", "Rocket.Surgery.Aspire.Hosting")
                       .Replace(", static b => b.Build()", "")
                );
            }

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.Aspire.Hosting.Testing.RocketDistributedApplicationTestingExtensions") is { }
             && compilation.GetTypeByMetadataName("Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder") is { })
            {
                context.AddSource(
                    "Generated_DistributedApplicationTestingBuilder_Extensions.g.cs",
                    _configurationMethods
                       .Replace("{BuilderName}", "DistributedApplicationTestingBuilder")
                       .Replace("{BuilderType}", "global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder")
                       .Replace("{ReturnType}", "global::Aspire.Hosting.DistributedApplication")
                       .Replace("{ExtensionsType}", "global::Rocket.Surgery.Aspire.Hosting.Testing.RocketDistributedApplicationTestingExtensions")
                       .Replace("{LoadConventions}", loadConventionsMethod)
                       .Replace("{HostingUsing}", "global::Aspire.Hosting.Testing")
                       .Replace("{RocketUsing}", "Rocket.Surgery.Aspire.Hosting.Testing")
                       .Replace(", static b => b.Build()", "")
                );
            }
        }

        static IReadOnlyCollection<string> getReferences(Compilation compilation, bool exports, ConventionConfigurationData configurationData)
        {
            return
            [
                .. compilation
                  .References
                  .Select(compilation.GetAssemblyOrModuleSymbol)
                  .OfType<IAssemblySymbol>()
                  .Select(
                       symbol =>
                       {
                           try
                           {
                               var config = ConventionConfigurationData.FromAssemblyAttributes(symbol, ConventionConfigurationData.ExportsDefaults);
                               if (symbol.GetTypeByMetadataName(
                                       config switch
                                       {
                                           { Namespace.Length: > 0, Postfix: true }  => $"{config.Namespace}.Conventions.{config.ClassName}",
                                           { Postfix: true }                         => $"Conventions.{config.ClassName}",
                                           { Namespace.Length: > 0, Postfix: false } => $"{config.Namespace}.{config.ClassName}",
                                           _                                         => config.ClassName,
                                       }
                                   ) is { } configuredMetadata)
                               {
                                   return configuredMetadata.ToDisplayString() + $".{config.MethodName}";
                               }
                           }
                           catch
                           {
                               //
                           }

                           // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                           return null!;
                       }
                   )
                  .Where(z => !string.IsNullOrWhiteSpace(z))
                  .Concat(
                       exports
                           ?
                           [
                               ( string.IsNullOrWhiteSpace(configurationData.Namespace) ? "" : configurationData.Namespace + "." )
                             + configurationData.ClassName
                             + "."
                             + configurationData.MethodName,
                           ]
                           : []
                   )
                  .OrderBy(z => z),
            ];
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

    private const string _configurationMethods = """"
        #pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8603, CS8602, CS8618
        using System.Threading.Tasks;
        using Microsoft.Extensions.Configuration;
        using Microsoft.Extensions.DependencyInjection;
        using {HostingUsing};
        using Microsoft.Extensions.Logging;
        using Rocket.Surgery.Conventions;
        using AppDelegate =
            System.Func<{BuilderType}, System.Threading.CancellationToken,
                System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

        namespace {RocketUsing};

        internal static partial class GeneratedRocket{BuilderName}Extensions
        {
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions());
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions());
                return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions());
                await action(contextBuilder, cancellationToken);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions());
                await action(contextBuilder, cancellationToken);
                return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions());
                await action(contextBuilder);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions());
                await action(contextBuilder);
                return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions());
                action(contextBuilder);
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions());
                action(contextBuilder);
                return await ConfigureRocketSurgery(await builder, action, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="contextBuilder">The convention context builder.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, ConventionContextBuilder contextBuilder, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(contextBuilder);
                return await {ExtensionsType}.Configure(builder, static b => b.Build(), contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="contextBuilder">The convention context builder.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, ConventionContextBuilder contextBuilder, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
        }
        """";

    public record Request
    (
        Compilation Compilation,
        bool HasExports,
        (bool isTestProject, string? rootNamespace) MsBuildConfig,
        ConventionConfigurationData ImportConfiguration,
        ConventionConfigurationData ExportConfiguration
    );
}
