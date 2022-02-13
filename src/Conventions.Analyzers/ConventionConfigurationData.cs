using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Conventions;

internal record ConventionConfigurationData(bool WasConfigured, bool Assembly, string? Namespace, string ClassName, string MethodName)
{
    private record InnerConventionConfigurationData(bool Assembly, string? Namespace, string ClassName, string MethodName)
    {
        public bool Postfix { get; init; } = true;
        public bool DefinedNamespace { get; init; }
        public bool WasConfigured { get; init; }

        public static InnerConventionConfigurationData FromDefaults(ConventionConfigurationData configurationData)
        {
            return new InnerConventionConfigurationData(configurationData.Assembly, null, configurationData.ClassName, configurationData.MethodName);
        }
    }

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
                        data = data with { Namespace = value, DefinedNamespace = true, WasConfigured = true };
                    }

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerConventionConfigurationData.ClassName)}", out value))
                    {
                        data = data with { ClassName = value, WasConfigured = true };
                    }

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerConventionConfigurationData.MethodName)}", out value))
                    {
                        data = data with { MethodName = value, WasConfigured = true };
                    }

                    if (config.GlobalOptions.TryGetValue($"build_property.{attributeName}{nameof(InnerConventionConfigurationData.Assembly)}", out value))
                    {
                        data = data with { Assembly = bool.TryParse(value, out var b) && b, WasConfigured = true };
                    }

                    if (config.GlobalOptions.TryGetValue(
                            $"build_property.{attributeName}{nameof(InnerConventionConfigurationData.Postfix)}", out value
                        ))
                    {
                        data = data with { Postfix = bool.TryParse(value, out var b) && b, WasConfigured = true };
                    }

                    return data;
                }
            );
        var assemblyConfiguration =
            context.SyntaxProvider
                   .CreateSyntaxProvider(
                        (node, token) => node is AttributeListSyntax attributeListSyntax
                                      && attributeListSyntax.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true
                                      && findAttribute(attributeListSyntax, attributeName) is not null,
                        (syntaxContext, token) =>
                            syntaxContext.Node is AttributeListSyntax attributeListSyntax
                                ? findAttribute(attributeListSyntax, attributeName)
                                : default
                    )
                   .Where(z => z is not null)
                   .Collect()
                   .Select(
                        (attributes, _) =>
                        {
                            var data = InnerConventionConfigurationData.FromDefaults(defaults);
                            if (!attributes.Any())
                            {
                                return data;
                            }

                            data = data with { WasConfigured = true };

                            var attribute = attributes.First();
                            if (attribute is null || attribute.ArgumentList is null or { Arguments.Count: 0 }) return data;
                            foreach (var arg in attribute.ArgumentList.Arguments)
                            {
                                if (arg is { NameEquals: null } or { Expression: null or not LiteralExpressionSyntax }) continue;
                                var syntax = (LiteralExpressionSyntax)arg.Expression;

                                data = arg.NameEquals.Name.Identifier.Text switch
                                {
                                    nameof(InnerConventionConfigurationData.Namespace) => data with
                                    {
                                        Namespace = (string)syntax.Token.Value!, DefinedNamespace = true
                                    },
                                    nameof(InnerConventionConfigurationData.ClassName) => data with
                                    {
                                        ClassName = (string)syntax.Token.Value!
                                    },
                                    nameof(InnerConventionConfigurationData.MethodName) => data with
                                    {
                                        MethodName = (string)syntax.Token.Value!
                                    },
                                    nameof(InnerConventionConfigurationData.Assembly) => data with
                                    {
                                        Assembly = (bool)syntax.Token.Value!
                                    },
                                    nameof(InnerConventionConfigurationData.Postfix) => data with
                                    {
                                        Postfix = (bool)syntax.Token.Value!
                                    },
                                    _ => data
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
                   (tuple, token) => new ConventionConfigurationData(
                       tuple.Left.WasConfigured,
                       tuple.Left.Assembly,
                       tuple.Left.DefinedNamespace ? tuple.Left.Namespace! : GetNamespaceForCompilation(tuple.Right, tuple.Left.Postfix),
                       tuple.Left.ClassName,
                       tuple.Left.MethodName
                   )
               );
    }

    public static ConventionConfigurationData FromAssemblyAttributes(IAssemblySymbol assemblySymbol, ConventionConfigurationData defaults)
    {
        var data = InnerConventionConfigurationData.FromDefaults(defaults);
        var prefix = $"Rocket.Surgery.ConventionConfigurationData.{defaults.ClassName}";
        foreach (var attribute in assemblySymbol.GetAttributes())
        {
            if (attribute is not { AttributeClass.MetadataName : "AssemblyMetadataAttribute", ConstructorArguments: { Length: 2 } }) continue;
            var key = (string)attribute.ConstructorArguments[0].Value!;
            if (!key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;

            var value = (string)attribute.ConstructorArguments[1].Value!;
            data = key.Split('.').Last() switch
            {
//                nameof(Assembly)   => data with { Assembly = bool.Parse(value) },
                nameof(Namespace)  => data with { Namespace = value },
                nameof(ClassName)  => data with { ClassName = value },
                nameof(MethodName) => data with { MethodName = value },
                _                  => data
            };
        }

        return new ConventionConfigurationData(false, data.Assembly, data.Namespace ?? assemblySymbol.Identity.Name, data.ClassName, data.MethodName);
    }

    public SyntaxList<AttributeListSyntax> ToAttributes(string type)
    {
        return SyntaxFactory.List(
            new[]
            {
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ConventionConfigurationData.{type}.{nameof(Namespace)}", Namespace ?? ""),
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ConventionConfigurationData.{type}.{nameof(ClassName)}", ClassName),
                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ConventionConfigurationData.{type}.{nameof(MethodName)}", MethodName),
//                Helpers.AddAssemblyAttribute($"Rocket.Surgery.ConventionConfigurationData.{type}.{nameof(Assembly)}", Assembly.ToString()),
            }
        );
    }

    private static AttributeSyntax? findAttribute(AttributeListSyntax list, string name)
    {
        return list.Attributes.FirstOrDefault(
            z => z.Name.ToFullString().TrimEnd().EndsWith(
                     name, StringComparison.OrdinalIgnoreCase
                 )
              || z.Name.ToFullString().TrimEnd().EndsWith(
                     $"{name}Attribute", StringComparison.OrdinalIgnoreCase
                 )
        );
    }

    private static string GetNamespaceForCompilation(Compilation compilation, bool postfix)
    {
        var @namespace = compilation.AssemblyName ?? "";
        if (postfix)
        {
            return ( @namespace.EndsWith(".Conventions", StringComparison.Ordinal) ? @namespace : @namespace + ".Conventions" ).TrimStart('.');
        }

        return @namespace;
    }
}
