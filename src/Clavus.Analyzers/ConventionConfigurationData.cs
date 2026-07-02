using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.Clavus;

internal record ClavusConfigurationData(bool WasConfigured, bool Assembly, string? Namespace, string ClassName, string MethodName)
{
    public static ClavusConfigurationData ExportsDefaults { get; } = new(false, true, "", "Exports", "GetConventions") { Postfix = true, };
    public static ClavusConfigurationData ImportsDefaults { get; } = new(false, true, "", "Imports", "Instance") { Postfix = true, };

    public static IncrementalValueProvider<ClavusConfigurationData> Create(
        IncrementalGeneratorInitializationContext context,
        string attributeName,
        ClavusConfigurationData defaults
    )
    {
        var msBuildConfiguration =
            context.AnalyzerConfigOptionsProvider.Select(
                (config, _) =>
                {
                    var data = InnerClavusConfigurationData.FromDefaults(defaults);
                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerClavusConfigurationData.Namespace)}", out var value))
                        data = data with { Namespace = value, DefinedNamespace = true, WasConfigured = true, };
                    else if (config.GlobalOptions.TryGetValue("build_property.RootNamespace", out value))
                        data = data with { Namespace = value, DefinedNamespace = true, };

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerClavusConfigurationData.ClassName)}", out value))
                        data = data with { ClassName = value, WasConfigured = true, };

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerClavusConfigurationData.MethodName)}", out value))
                        data = data with { MethodName = value, WasConfigured = true, };

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerClavusConfigurationData.Assembly)}", out value))
                        data = data with { Assembly = bool.TryParse(value, out var b) && b, WasConfigured = true, };

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
                        var data = InnerClavusConfigurationData.FromDefaults(defaults);
                        if (!attributes.Any()) return data;

                        data = data with { WasConfigured = true, };

                        var attribute = attributes.First();
                        if (attribute is null || attribute.ArgumentList is null or { Arguments.Count: 0, }) return data;
                        foreach (var arg in attribute.ArgumentList.Arguments)
                        {
                            if (arg is { NameEquals: null, } or { Expression: null or not LiteralExpressionSyntax, }) continue;
                            var syntax = (LiteralExpressionSyntax)arg.Expression;

                            data = arg.NameEquals.Name.Identifier.Text switch
                            {
                                nameof(InnerClavusConfigurationData.Namespace) => data with
                                {
                                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                    Namespace = (string)syntax.Token.Value!,
                                    DefinedNamespace = true,
                                },
                                nameof(InnerClavusConfigurationData.ClassName) => data with
                                {
                                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                    ClassName = (string)syntax.Token.Value!,
                                },
                                nameof(InnerClavusConfigurationData.MethodName) => data with
                                {
                                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                    MethodName = (string)syntax.Token.Value!,
                                },
                                nameof(InnerClavusConfigurationData.Assembly) => data with
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
                   (tuple, _) => new ClavusConfigurationData(
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

    public static ClavusConfigurationData FromAssemblyAttributes(IAssemblySymbol assemblySymbol, ClavusConfigurationData defaults)
    {
        var data = InnerClavusConfigurationData.FromDefaults(defaults);
        var prefix = $"Rocket.Surgery.ClavusConfigurationData.{defaults.ClassName}";
        foreach (var attribute in assemblySymbol.GetAttributes().Where(z => z is { AttributeClass.MetadataName: "AssemblyMetadataAttribute", }))
        {
            if (attribute is not
                {
                    ConstructorArguments: [{ Value: string { Length: > 0, } key, }, var value,],
                }
             || !key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            data = key.Split('.').Last() switch
            {
                nameof(Namespace) => data with { Namespace = (string?)value.Value, },
                nameof(ClassName) => data with { ClassName = (string)value.Value!, },
                nameof(MethodName) => data with { MethodName = (string)value.Value!, },
                _ => data,
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
        return postfix
            ?  ( @namespace.EndsWith(".Conventions", StringComparison.Ordinal) ? @namespace : @namespace + ".Conventions" ).TrimStart('.') 
            : @namespace;
    }

    public bool Postfix { get; init; }

    public SyntaxList<AttributeListSyntax> ToAttributes(string type)
    {
        var list = List(
            new[]
            {
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ClavusConfigurationData.{type}.{nameof(Namespace)}", Namespace),
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ClavusConfigurationData.{type}.{nameof(ClassName)}", ClassName),
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ClavusConfigurationData.{type}.{nameof(MethodName)}", MethodName),
            }
        );
        if (type == "Import")
        {
            list = list.Add(
                AttributeList(
                        SingletonSeparatedList(
                            Attribute(ParseName("Rocket.Surgery.Clavus.ImportsType"))
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

    private record InnerClavusConfigurationData(bool Assembly, string? Namespace, string ClassName, string MethodName)
    {
        public static InnerClavusConfigurationData FromDefaults(ClavusConfigurationData configurationData) => new(configurationData.Assembly, null, configurationData.ClassName, configurationData.MethodName);

        public bool DefinedNamespace { get; init; }
        public bool WasConfigured { get; init; }
    }
}
