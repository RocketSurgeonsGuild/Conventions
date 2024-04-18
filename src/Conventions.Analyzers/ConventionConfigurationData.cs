using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Conventions;

internal record ConventionConfigurationData(bool WasConfigured, bool Assembly, string? Namespace, string ClassName, string MethodName)
{
    public static ConventionConfigurationData ExportsDefaults { get; } = new(false, true, "", "Exports", "GetConventions") { Postfix = true, };
    public static ConventionConfigurationData ImportsDefaults { get; } = new(false, true, "", "Imports", "Instance") { Postfix = true, };

    public static IncrementalValueProvider<ConventionConfigurationData> Create(
        IncrementalGeneratorInitializationContext context,
        string attributeName,
        ConventionConfigurationData defaults
    )
    {
        var msBuildConfiguration =
            context.AnalyzerConfigOptionsProvider.Select(
                (config, _) =>
                {
                    var data = InnerConventionConfigurationData.FromDefaults(defaults);
                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerConventionConfigurationData.Namespace)}", out var value))
                    {
                        data = data with { Namespace = value, DefinedNamespace = true, WasConfigured = true, };
                    }

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerConventionConfigurationData.ClassName)}", out value))
                    {
                        data = data with { ClassName = value, WasConfigured = true, };
                    }

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerConventionConfigurationData.MethodName)}", out value))
                    {
                        data = data with { MethodName = value, WasConfigured = true, };
                    }

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerConventionConfigurationData.Assembly)}", out value))
                    {
                        data = data with { Assembly = bool.TryParse(value, out var b) && b, WasConfigured = true, };
                    }

                    return data;
                }
            );
        var assemblyConfiguration =
            context
               .SyntaxProvider
               .CreateSyntaxProvider(
                    (node, _) => node is AttributeListSyntax attributeListSyntax
                     && attributeListSyntax.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true
                     && FindAttribute(attributeListSyntax, attributeName) is { },
                    (syntaxContext, _) =>
                        syntaxContext.Node is AttributeListSyntax attributeListSyntax
                            ? FindAttribute(attributeListSyntax, attributeName)
                            : default
                )
               .Where(z => z is { })
               .Collect()
               .Select(
                    (attributes, _) =>
                    {
                        var data = InnerConventionConfigurationData.FromDefaults(defaults);
                        if (!attributes.Any())
                        {
                            return data;
                        }

                        data = data with { WasConfigured = true, };

                        var attribute = attributes.First();
                        if (attribute is null || attribute.ArgumentList is null or { Arguments.Count: 0, }) return data;
                        foreach (var arg in attribute.ArgumentList.Arguments)
                        {
                            if (arg is { NameEquals: null, } or { Expression: null or not LiteralExpressionSyntax, }) continue;
                            var syntax = (LiteralExpressionSyntax)arg.Expression;

                            data = arg.NameEquals.Name.Identifier.Text switch
                                   {
                                       nameof(InnerConventionConfigurationData.Namespace) => data with
                                       {
                                           // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                           Namespace = (string)syntax.Token.Value!, DefinedNamespace = true,
                                       },
                                       nameof(InnerConventionConfigurationData.ClassName) => data with
                                       {
                                           // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                           ClassName = (string)syntax.Token.Value!,
                                       },
                                       nameof(InnerConventionConfigurationData.MethodName) => data with
                                       {
                                           // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                           MethodName = (string)syntax.Token.Value!,
                                       },
                                       nameof(InnerConventionConfigurationData.Assembly) => data with
                                       {
                                           // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                           Assembly = (bool)syntax.Token.Value!,
                                       },
                                       _ => data,
                                   };
                        }

                        return data;
                    }
                );

        return assemblyConfiguration
              .Combine(msBuildConfiguration)
              .Select((z, _) => z.Left.WasConfigured ? z.Left : z.Right)
              .Combine(context.CompilationProvider)
              .Select(
                   (tuple, _) => new ConventionConfigurationData(
                       tuple.Left.WasConfigured,
                       tuple.Left.Assembly,
                       // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                       tuple.Left.DefinedNamespace ? tuple.Left.Namespace! : GetNamespaceForCompilation(tuple.Right, defaults.Postfix),
                       tuple.Left.ClassName,
                       tuple.Left.MethodName
                   )
               )
              .Select((data, _) => data with { Namespace = data.Namespace == "global" ? "" : data.Namespace, });
    }

    public static ConventionConfigurationData FromAssemblyAttributes(IAssemblySymbol assemblySymbol, ConventionConfigurationData defaults)
    {
        var data = InnerConventionConfigurationData.FromDefaults(defaults);
        var prefix = $"Rocket.Surgery.ConventionConfigurationData.{defaults.ClassName}";
        foreach (var attribute in assemblySymbol.GetAttributes().Where(z => z is { AttributeClass.MetadataName : "AssemblyMetadataAttribute", }))
        {
            if (attribute is not
                {
                    ConstructorArguments: [{ Value: string { Length: > 0, } key, }, var value,],
                }
             || !key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
            data = key.Split('.').Last() switch
                   {
                       nameof(Namespace)  => data with { Namespace = (string?)value.Value, },
                       nameof(ClassName)  => data with { ClassName = (string)value.Value!, },
                       nameof(MethodName) => data with { MethodName = (string)value.Value!, },
                       _                  => data,
                   };
        }

        return new(false, data.Assembly, data.Namespace, data.ClassName, data.MethodName);
    }

    private static AttributeSyntax? FindAttribute(AttributeListSyntax list, string name)
    {
        return list.Attributes.FirstOrDefault(
            z => z.Name.ToFullString().TrimEnd().EndsWith(name, StringComparison.OrdinalIgnoreCase)
             || z.Name.ToFullString().TrimEnd().EndsWith($"{name}Attribute", StringComparison.OrdinalIgnoreCase)
        );
    }

    private static string GetNamespaceForCompilation(Compilation compilation, bool postfix = false)
    {
        var @namespace = compilation.AssemblyName ?? "";
        if (postfix)
        {
            return ( @namespace.EndsWith(".Conventions", StringComparison.Ordinal) ? @namespace : @namespace + ".Conventions" ).TrimStart('.');
        }

        return @namespace;
    }

    public bool Postfix { get; init; }

    public SyntaxList<AttributeListSyntax> ToAttributes(string type)
    {
        var list = List(
            new[]
            {
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ConventionConfigurationData.{type}.{nameof(Namespace)}", Namespace),
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ConventionConfigurationData.{type}.{nameof(ClassName)}", ClassName),
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ConventionConfigurationData.{type}.{nameof(MethodName)}", MethodName),
            }
        );
        if (type == "Import")
        {
            list = list.Add(
                AttributeList(
                        SingletonSeparatedList(
                            Attribute(ParseName("Rocket.Surgery.Conventions.ImportsType"))
                               .WithArgumentList(
                                    AttributeArgumentList(
                                        SingletonSeparatedList(
                                            AttributeArgument(
                                                TypeOfExpression(ParseTypeName(( Namespace is { Length: > 0, } ? Namespace + "." : "" ) + ClassName))
                                            )
                                        )
                                    )
                                )
                        )
                    )
                   .WithTarget(AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword)))
            );
        }

        return list;
    }

    private record InnerConventionConfigurationData(bool Assembly, string? Namespace, string ClassName, string MethodName)
    {
        public static InnerConventionConfigurationData FromDefaults(ConventionConfigurationData configurationData)
        {
            return new(configurationData.Assembly, null, configurationData.ClassName, configurationData.MethodName);
        }

        public bool DefinedNamespace { get; init; }
        public bool WasConfigured { get; init; }
    }
}