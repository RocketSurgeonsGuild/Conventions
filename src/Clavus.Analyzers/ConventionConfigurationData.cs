using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Clavus;

internal record ClavusConfigurationData(string Property, string Namespace, string ClassName, string MethodName)
{
    public static IncrementalValueProvider<ClavusConfigurationData> Read(
        IncrementalGeneratorInitializationContext context,
        string propertyPrefix
    )
    {
        var prefix = $"Clavus{propertyPrefix}";
        return context.AnalyzerConfigOptionsProvider.Select((config, _) => new ClavusConfigurationData(
                                                                propertyPrefix,
                                                                config.GlobalOptions.GetBuildProperty($"{prefix}{nameof(Namespace)}", s => s) ?? "##??NOT DEFINED??##",
                                                                config.GlobalOptions.GetBuildProperty($"{prefix}{nameof(ClassName)}", s => s) ?? "##??NOT DEFINED??##",
                                                                config.GlobalOptions.GetBuildProperty($"{prefix}{nameof(MethodName)}", s => s) ?? "##??NOT DEFINED??##"
                                                            )
        );
    }

    public static ClavusConfigurationData? FromAssemblyAttributes(IAssemblySymbol assemblySymbol, string propertyPrefix)
    {
        var prefix = $"Clavus.{propertyPrefix}.";
        var attributes = assemblySymbol.GetAssemblyMetadataAttributes(z => z.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        return  attributes.Count == 0 
            ?   null  
            :   new(
            propertyPrefix,
            attributes.TryGetValue($"{prefix}{nameof(Namespace)}", out var ns) && ns is { Value: string namespaceValue } ? namespaceValue : "##??NOT DEFINED??##",
            attributes.TryGetValue($"{prefix}{nameof(ClassName)}", out var className) && className is { Value: string classValue } ? classValue : "##??NOT DEFINED??##",
            attributes.TryGetValue($"{prefix}{nameof(MethodName)}", out var methodName) && methodName is { Value: string methodNameValue } ? methodNameValue : "##??NOT DEFINED??##"
        );
    }

    public SyntaxList<AttributeListSyntax> ToAttributes()
    {
        return [with(
            GetType()
               .GetProperties()
               .Select(z => Helpers.AddAssemblyAttribute($"Clavus.{Property}.{z.Name}", z.GetValue(this) is string ? (string)z.GetValue(this) : null))
        )];
    }

    public override string ToString() => $"{Namespace}.{ClassName}.{MethodName}";
}
