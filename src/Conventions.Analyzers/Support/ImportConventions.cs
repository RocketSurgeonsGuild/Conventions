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

        var compilation = request.Compilation;

        var importsClass =
            ClassDeclaration(request.ImportConfiguration.ClassName)
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
                            Identifier(request.ImportConfiguration.MethodName)
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
                                            ObjectCreationExpression(IdentifierName(request.ImportConfiguration.ClassName)).WithArgumentList(ArgumentList()),
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
                       .WithBody(functionBody)
                       .WithLeadingTrivia(GetXmlSummary("The conventions imported into this assembly"))
                );


        if (request.MsBuildConfig.isTestProject)
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
                                        IdentifierName(request.ImportConfiguration.MethodName)
                                    )
                                )
                            )
                        )
                    )
            );

        var cu = CompilationUnit()
                .WithAttributeLists(request.ImportConfiguration.ToAttributes("Imports"))
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
        if (request.ImportConfiguration is { Assembly: true, })
        {
            cu = cu
               .AddMembers(
                    request.ImportConfiguration is { Namespace: { Length: > 0, } relativeNamespace, }
                        ? [NamespaceDeclaration(ParseName(relativeNamespace)).AddMembers(members.ToArray()),]
                        : members.ToArray()
                );

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.Hosting.RocketHostApplicationExtensions") is { })
            {
                if (compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Builder.WebApplicationBuilder") is { })
                    context.AddSource(
                        "Generated_WebApplicationBuilder_Extensions.cs",
                        _configurationMethods
                           .Replace("{BuilderName}", "WebApplicationBuilder")
                           .Replace("{BuilderType}", "Microsoft.AspNetCore.Builder.WebApplicationBuilder")
                           .Replace("{ReturnType}", "Microsoft.AspNetCore.Builder.WebApplication")
                           .Replace(
                                "{HostBuiltEvent}",
                                "Rocket.Surgery.Hosting.RocketHostApplicationExtensions.HostBuiltEvent<Microsoft.AspNetCore.Builder.WebApplication>"
                            )
                           .Replace("{ExtensionsType}", "Rocket.Surgery.Hosting.RocketHostApplicationExtensions")
                           .Replace("{HostingUsing}", "Microsoft.Extensions.Hosting")
                           .Replace("{RocketUsing}", "Rocket.Surgery.Hosting")
                    );

                if (compilation.GetTypeByMetadataName("Microsoft.Extensions.Hosting.HostApplicationBuilder") is { })
                    context.AddSource(
                        "Generated_HostApplicationBuilder_Extensions.cs",
                        _configurationMethods
                           .Replace("{BuilderName}", "HostApplicationBuilder")
                           .Replace("{BuilderType}", "Microsoft.Extensions.Hosting.HostApplicationBuilder")
                           .Replace("{ReturnType}", "Microsoft.Extensions.Hosting.IHost")
                           .Replace(
                                "{HostBuiltEvent}",
                                "Rocket.Surgery.Hosting.RocketHostApplicationExtensions.HostBuiltEvent<Microsoft.Extensions.Hosting.IHost>"
                            )
                           .Replace("{ExtensionsType}", "Rocket.Surgery.Hosting.RocketHostApplicationExtensions")
                           .Replace("{HostingUsing}", "Microsoft.Extensions.Hosting")
                           .Replace("{RocketUsing}", "Rocket.Surgery.Hosting")
                    );
            }

            if (compilation.GetTypeByMetadataName("Rocket.Surgery.WebAssembly.Hosting.RocketWebAssemblyExtensions") is { }
             && compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder") is { })
            {
                context.AddSource(
                    "Generated_WebAssemblyBuilder_Extensions.cs",
                    _configurationMethods
                       .Replace("{BuilderName}", "WebAssemblyHostBuilder")
                       .Replace("{BuilderType}", "Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder")
                       .Replace("{ReturnType}", "Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost")
                       .Replace("{HostBuiltEvent}", "Rocket.Surgery.WebAssembly.Hosting.RocketWebAssemblyExtensions.HostBuiltEvent")
                       .Replace("{ExtensionsType}", "Rocket.Surgery.WebAssembly.Hosting.RocketWebAssemblyExtensions")
                       .Replace("{HostingUsing}", "Microsoft.AspNetCore.Components.WebAssembly.Hosting")
                       .Replace("{RocketUsing}", "Rocket.Surgery.WebAssembly.Hosting")
                );
            }
        }

        context.AddSource(
            "Imported_Assembly_Conventions.cs",
            cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
        );

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
                               var config = ConventionConfigurationData.FromAssemblyAttributes(symbol, ConventionConfigurationData.ExportsDefaults);
                               if (symbol.GetTypeByMetadataName(
                                       config switch
                                       {
                                           { Namespace.Length: > 0, Postfix: true, }  => $"{config.Namespace}.Conventions.{config.ClassName}",
                                           { Postfix: true, }                         => $"Conventions.{config.ClassName}",
                                           { Namespace.Length: > 0, Postfix: false, } => $"{config.Namespace}.{config.ClassName}",
                                           _                                          => config.ClassName,
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
        #pragma warning disable CS0105
        #pragma warning disable CS8602
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
            ///     Uses the rocket booster.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> UseRocketBooster(this {BuilderType} builder, AppDelegate func, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(func);
                var b = await func(builder, cancellationToken);
                return await ConfigureRocketSurgery(builder, b, cancellationToken);
            }
        
            /// <summary>
            ///     Uses the rocket booster.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> UseRocketBooster(this {BuilderType} builder, AppDelegate func, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(func);
                ArgumentNullException.ThrowIfNull(action);
                var b = await func(builder, cancellationToken);
                await action(b, cancellationToken);
                return await ConfigureRocketSurgery(builder, b, cancellationToken);
            }
        
            /// <summary>
            ///     Uses the rocket booster.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> UseRocketBooster(this {BuilderType} builder, AppDelegate func, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(func);
                ArgumentNullException.ThrowIfNull(action);
                var b = await func(builder, cancellationToken);
                await action(b);
                return await ConfigureRocketSurgery(builder, b, cancellationToken);
            }
        
            /// <summary>
            ///     Uses the rocket booster.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> UseRocketBooster(this {BuilderType} builder, AppDelegate func, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(func);
                ArgumentNullException.ThrowIfNull(action);
                var b = await func(builder, cancellationToken);
                action(b);
                return await ConfigureRocketSurgery(builder, b, cancellationToken);
            }
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> LaunchWith(this {BuilderType} builder, AppDelegate func, CancellationToken cancellationToken = default) => UseRocketBooster(builder, func, cancellationToken);
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> LaunchWith(this {BuilderType} builder, AppDelegate func, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default) => UseRocketBooster(builder, func, action, cancellationToken);
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> LaunchWith(this {BuilderType} builder, AppDelegate func, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default) => UseRocketBooster(builder, func, action, cancellationToken);
        
            /// <summary>
            ///     Launches the with.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="func">The function.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> LaunchWith(this {BuilderType} builder, AppDelegate func, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default) => UseRocketBooster(builder, func, action, cancellationToken);
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                var contextBuilder = {ExtensionsType}.GetExisting(builder);
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
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
                var contextBuilder = {ExtensionsType}.GetExisting(builder);
                await action(contextBuilder, cancellationToken);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
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
                var contextBuilder = {ExtensionsType}.GetExisting(builder);
                await action(contextBuilder);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
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
                var contextBuilder = {ExtensionsType}.GetExisting(builder);
                action(contextBuilder);
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="getConventions">The method to get the conventions.</param>
            /// <param name="action">The configuration action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, IConventionFactory getConventions, Func<ConventionContextBuilder, CancellationToken, ValueTask> action ,CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(getConventions);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = {ExtensionsType}.GetExisting(builder).UseConventionFactory(getConventions);
                await action(contextBuilder, cancellationToken);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="getConventions">The method to get the conventions.</param>
            /// <param name="action">The configuration action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder,IConventionFactory getConventions, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(getConventions);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = {ExtensionsType}.GetExisting(builder).UseConventionFactory(getConventions);
                await action(contextBuilder);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="getConventions">The method to get the conventions.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, IConventionFactory getConventions, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(getConventions);
                var contextBuilder = {ExtensionsType}.GetExisting(builder).UseConventionFactory(getConventions);
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="getConventions">The method to get the conventions.</param>
            /// <param name="action">The configuration action.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, IConventionFactory getConventions, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(getConventions);
                ArgumentNullException.ThrowIfNull(action);
                var contextBuilder = {ExtensionsType}.GetExisting(builder).UseConventionFactory(getConventions);
                action(contextBuilder);
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="contextBuilder">The convention context builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, ConventionContextBuilder contextBuilder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(contextBuilder);
                ArgumentNullException.ThrowIfNull(action);
                await action(contextBuilder, cancellationToken);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="contextBuilder">The convention context builder.</param>
            /// <param name="action">The action.</param>
            /// <param name="cancellationToken"></param>
            public static async ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, ConventionContextBuilder contextBuilder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(contextBuilder);
                ArgumentNullException.ThrowIfNull(action);
                await action(contextBuilder);
                return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }
        
            /// <summary>
            ///     Configures the rocket Surgery.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <param name="contextBuilder">The convention context builder.</param>
            /// <param name="cancellationToken"></param>
            public static ValueTask<{ReturnType}> ConfigureRocketSurgery(this {BuilderType} builder, ConventionContextBuilder contextBuilder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
            {
                ArgumentNullException.ThrowIfNull(builder);
                ArgumentNullException.ThrowIfNull(contextBuilder);
                ArgumentNullException.ThrowIfNull(action);
                action(contextBuilder);
                return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
            }
        
            /// <summary>
            /// Run a simple action when the host has been built
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="action"></param>
            /// <returns></returns>
            public static {BuilderType} OnHostBuilt(this {BuilderType} builder, Action<{ReturnType}> action)
            {
                {ExtensionsType}.GetExisting(builder).GetOrAdd(() => new List<{HostBuiltEvent}>()).Add(new((provider, _) =>
                        {
                            action(provider);
                            return ValueTask.CompletedTask;
                        }
                    )
                );
                return builder;
            }
        
            /// <summary>
            /// Run a simple action when the host has been built
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="action"></param>
            /// <returns></returns>
            public static {BuilderType} OnHostBuilt(this {BuilderType} builder, Func<{ReturnType}, ValueTask> action)
            {
                {ExtensionsType}.GetExisting(builder).GetOrAdd(() => new List<{HostBuiltEvent}>()).Add(new((provider, _) => action(provider)));
                return builder;
            }
        
            /// <summary>
            /// Run a simple action when the host has been built
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="action"></param>
            /// <returns></returns>
            public static {BuilderType} OnHostBuilt(this {BuilderType} builder, Func<{ReturnType}, CancellationToken, ValueTask> action)
            {
                {ExtensionsType}.GetExisting(builder).GetOrAdd(() => new List<{HostBuiltEvent}>()).Add(new(action));
                return builder;
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