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

        var hasSerilog = request.Compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.ConventionSerilogExtensions") is { };

        if (request.ImportConfiguration is { Assembly: true })
        {
            var loadConventionsMethod = request.ImportConfiguration.Namespace is { Length: > 0 }
                ? $"global::{request.ImportConfiguration.Namespace}.{request.ImportConfiguration.ClassName}.{request.ImportConfiguration.MethodName}"
                : $"global::{request.ImportConfiguration.ClassName}.{request.ImportConfiguration.MethodName}";

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.Hosting.RocketHostApplicationExtensions") is { })
            {
                if (compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Builder.WebApplicationBuilder") is { })
                {
                    var builderName = "WebApplicationBuilder";
                    var builderType = "global::Microsoft.AspNetCore.Builder.WebApplicationBuilder";
                    var returnType = "global::Microsoft.AspNetCore.Builder.WebApplication";
                    var extensionsType = "global::Rocket.Surgery.Hosting.RocketHostApplicationExtensions";
                    var hostingUsing = "global::Microsoft.Extensions.Hosting";
                    var rocketUsing = "Rocket.Surgery.Hosting";
                    context.AddSource(
                        "Generated_WebApplicationBuilder_Extensions.g.cs",
                        transformTemplate(
                            _configurationMethods,
                            builderName,
                            builderType,
                            returnType,
                            extensionsType,
                            loadConventionsMethod,
                            hostingUsing,
                            rocketUsing
                        )
                    );
                    if (hasSerilog)
                        context.AddSource(
                            "Generated_WebApplicationBuilder_Extensions_Serilog.g.cs",
                            transformTemplate(
                                _serilogConfigurationMethods,
                                builderName,
                                builderType,
                                returnType,
                                extensionsType,
                                loadConventionsMethod,
                                hostingUsing,
                                rocketUsing
                            )
                        );
                }

                if (compilation.GetTypeByMetadataName("Microsoft.Extensions.Hosting.HostApplicationBuilder") is { })
                {
                    var builderName = "HostApplicationBuilder";
                    var builderType = "global::Microsoft.Extensions.Hosting.HostApplicationBuilder";
                    var returnType = "global::Microsoft.Extensions.Hosting.IHost";
                    var extensionsType = "global::Rocket.Surgery.Hosting.RocketHostApplicationExtensions";
                    var hostingUsing = "global::Microsoft.Extensions.Hosting";
                    var rocketUsing = "Rocket.Surgery.Hosting";

                    context.AddSource(
                        "Generated_HostApplicationBuilder_Extensions.g.cs",
                        transformTemplate(
                            _configurationMethods,
                            builderName,
                            builderType,
                            returnType,
                            extensionsType,
                            loadConventionsMethod,
                            hostingUsing,
                            rocketUsing
                        )
                    );

                    if (hasSerilog)
                        context.AddSource(
                            "Generated_HostApplicationBuilder_Extensions_Serilog.g.cs",
                            transformTemplate(
                                _serilogConfigurationMethods,
                                builderName,
                                builderType,
                                returnType,
                                extensionsType,
                                loadConventionsMethod,
                                hostingUsing,
                                rocketUsing
                            )
                        );
                }
            }

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.WebAssembly.Hosting.RocketWebAssemblyExtensions") is { }
             && compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder") is { })
            {
                var builderName = "WebAssemblyHostBuilder";
                var builderType = "global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder";
                var returnType = "global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost";
                var extensionsType = "global::Rocket.Surgery.WebAssembly.Hosting.RocketWebAssemblyExtensions";
                var hostingUsing = "global::Microsoft.AspNetCore.Components.WebAssembly.Hosting";
                var rocketUsing = "Rocket.Surgery.WebAssembly.Hosting";

                context.AddSource(
                    "Generated_WebAssemblyBuilder_Extensions.g.cs",
                    transformTemplate(
                        _configurationMethods,
                        builderName,
                        builderType,
                        returnType,
                        extensionsType,
                        loadConventionsMethod,
                        hostingUsing,
                        rocketUsing
                    )
                );
                if (hasSerilog)
                    context.AddSource(
                        "Generated_WebAssemblyBuilder_Extensions_Serilog.g.cs",
                        transformTemplate(
                            _serilogConfigurationMethods,
                            builderName,
                            builderType,
                            returnType,
                            extensionsType,
                            loadConventionsMethod,
                            hostingUsing,
                            rocketUsing
                        )
                    );
            }

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.Aspire.Hosting.RocketDistributedApplicationExtensions") is { }
                && compilation.GetTypeByMetadataName("Aspire.Hosting.IDistributedApplicationBuilder") is { })
            {
                var builderName = "DistributedApplicationBuilder";
                var builderType = "global::Aspire.Hosting.IDistributedApplicationBuilder";
                var returnType = "global::Aspire.Hosting.DistributedApplication";
                var extensionsType = "global::Rocket.Surgery.Aspire.Hosting.RocketDistributedApplicationExtensions";
                var hostingUsing = "global::Aspire.Hosting";
                var rocketUsing = "Rocket.Surgery.Aspire.Hosting";

                context.AddSource(
                    "Generated_DistributedApplicationBuilder_Extensions.g.cs",
                    transformTemplate(
                        _configurationMethods,
                        builderName,
                        builderType,
                        returnType,
                        extensionsType,
                        loadConventionsMethod,
                        hostingUsing,
                        rocketUsing
                    )
                   .Replace(", static b => b.Build()", "")
                );
                if (hasSerilog)
                    context.AddSource(
                        "Generated_DistributedApplicationBuilder_Extensions_Serilog.g.cs",
                        transformTemplate(
                            _serilogConfigurationMethods,
                            builderName,
                            builderType,
                            returnType,
                            extensionsType,
                            loadConventionsMethod,
                            hostingUsing,
                            rocketUsing
                        )
                       .Replace(", static b => b.Build()", "")
                    );
            }

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.Aspire.Hosting.Testing.RocketDistributedApplicationTestingExtensions") is { }
             && compilation.GetTypeByMetadataName("Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder") is { })
            {
                var builderName = "DistributedApplicationTestingBuilder";
                var builderType = "global::Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder";
                var returnType = "global::Aspire.Hosting.DistributedApplication";
                var extensionsType = "global::Rocket.Surgery.Aspire.Hosting.Testing.RocketDistributedApplicationTestingExtensions";
                var hostingUsing = "global::Aspire.Hosting.Testing";
                var rocketUsing = "Rocket.Surgery.Aspire.Hosting.Testing";
                context.AddSource(
                    "Generated_DistributedApplicationTestingBuilder_Extensions.g.cs",
                    transformTemplate(
                        _configurationMethods,
                        builderName,
                        builderType,
                        returnType,
                        extensionsType,
                        loadConventionsMethod,
                        hostingUsing,
                        rocketUsing
                    )
                   .Replace(", static b => b.Build()", "")
                );
                if (hasSerilog)
                    context.AddSource(
                        "Generated_DistributedApplicationTestingBuilder_Extensions_Serilog.g.cs",
                        transformTemplate(
                            _serilogConfigurationMethods,
                            builderName,
                            builderType,
                            returnType,
                            extensionsType,
                            loadConventionsMethod,
                            hostingUsing,
                            rocketUsing
                        )
                       .Replace(", static b => b.Build()", "")
                    );
            }
        }

        static string transformTemplate(
            string template,
            string builderName,
            string builderType,
            string returnType,
            string extensionsType,
            string loadConventionsMethod,
            string hostingUsing,
            string rocketUsing
        )
            => template
              .Replace("{BuilderName}", builderName)
              .Replace("{BuilderType}", builderType)
              .Replace("{ReturnType}", returnType)
              .Replace("{ExtensionsType}", extensionsType)
              .Replace("{LoadConventions}", loadConventionsMethod)
              .Replace("{HostingUsing}", hostingUsing)
              .Replace("{RocketUsing}", rocketUsing);

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

    private const string _serilogConfigurationMethods = """"
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
        using ILogger = Serilog.ILogger;

        namespace {RocketUsing};

        internal static partial class GeneratedRocket{BuilderName}Extensions
        {
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, ILogger logger, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions()).UseLogger(logger);
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, ILogger logger, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions()).UseLogger(logger);
                return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, ILogger logger, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions()).UseLogger(logger);
                await action(contextBuilder, cancellationToken);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, ILogger logger, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions()).UseLogger(logger);
                await action(contextBuilder, cancellationToken);
                return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, ILogger logger, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions()).UseLogger(logger);
                await action(contextBuilder);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, ILogger logger, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions()).UseLogger(logger);
                await action(contextBuilder);
                return await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, ILogger logger, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions()).UseLogger(logger);
                action(contextBuilder);
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }

            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this Task<{BuilderType}> builder, ILogger logger, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = ConventionContextBuilder.Create({LoadConventions}.OrCallerConventions()).UseLogger(logger);
                action(contextBuilder);
                return await ConfigureRocketSurgery(await builder, action, cancellationToken);
            }
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
