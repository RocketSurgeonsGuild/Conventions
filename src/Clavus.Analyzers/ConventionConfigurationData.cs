using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Clavus;

internal record ClavusConfigurationData(string Property, string Namespace, string ClassName, string MethodName) : IComparable<ClavusConfigurationData>, IComparable
{
    public static IncrementalValueProvider<ClavusConfigurationData> Read(
        IncrementalGeneratorInitializationContext context,
        string propertyPrefix
    )
    {
        var prefix = $"Clavus{propertyPrefix}";
        return context.AnalyzerConfigOptionsProvider.Select((config, _) => new ClavusConfigurationData(
                                                                propertyPrefix,
                                                                config.GlobalOptions.GetBuildProperty(prefix + nameof(Namespace), s => s) ?? "##??NOT DEFINED??##",
                                                                config.GlobalOptions.GetBuildProperty(prefix + nameof(ClassName), s => s) ?? "##??NOT DEFINED??##",
                                                                config.GlobalOptions.GetBuildProperty(prefix + nameof(MethodName), s => s) ?? "##??NOT DEFINED??##"
                                                            )
        );
    }

    public static ClavusConfigurationData? FromAssemblyAttributes(IAssemblySymbol assemblySymbol, string propertyPrefix)
    {
        var prefix = $"Clavus.{propertyPrefix}.";
        var attributes = assemblySymbol.GetAssemblyMetadataAttributes(z => z.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        return attributes.Count == 0
            ? null
            : new(
            propertyPrefix,
            attributes.TryGetValue($"{prefix}{nameof(Namespace)}", out var ns) && ns is { Value: string namespaceValue } ? namespaceValue : "##??NOT DEFINED??##",
            attributes.TryGetValue($"{prefix}{nameof(ClassName)}", out var className) && className is { Value: string classValue } ? classValue : "##??NOT DEFINED??##",
            attributes.TryGetValue($"{prefix}{nameof(MethodName)}", out var methodName) && methodName is { Value: string methodNameValue } ? methodNameValue : "##??NOT DEFINED??##"
        );
    }

    public SyntaxList<AttributeListSyntax> ToAttributes()
    {
        return [
            .. GetType()
               .GetProperties()
               .Select(z => Helpers.AddAssemblyAttribute($"Clavus.{Property}.{z.Name}", z.GetValue(this) is string ? (string)z.GetValue(this) : null))
        ];
    }

    public override string ToString() => $"{( Namespace is { Length: > 0 } ? $"{Namespace}." : "" )}{ClassName}.{MethodName}";

    public int CompareTo(ClavusConfigurationData? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var propertyComparison = string.Compare(Property, other.Property, StringComparison.Ordinal);
        if (propertyComparison != 0) return propertyComparison;
        var namespaceComparison = string.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
        if (namespaceComparison != 0) return namespaceComparison;
        var classNameComparison = string.Compare(ClassName, other.ClassName, StringComparison.Ordinal);
        return  classNameComparison != 0  ? classNameComparison :  string.Compare(MethodName, other.MethodName, StringComparison.Ordinal);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return  ReferenceEquals(this, obj) 
            ?  0 
            :  obj is ClavusConfigurationData other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ClavusConfigurationData)}");
    }

    public static bool operator <(ClavusConfigurationData? left, ClavusConfigurationData? right) => Comparer<ClavusConfigurationData>.Default.Compare(left, right) < 0;

    public static bool operator >(ClavusConfigurationData? left, ClavusConfigurationData? right) => Comparer<ClavusConfigurationData>.Default.Compare(left, right) > 0;

    public static bool operator <=(ClavusConfigurationData? left, ClavusConfigurationData? right) => Comparer<ClavusConfigurationData>.Default.Compare(left, right) <= 0;

    public static bool operator >=(ClavusConfigurationData? left, ClavusConfigurationData? right) => Comparer<ClavusConfigurationData>.Default.Compare(left, right) >= 0;
}
