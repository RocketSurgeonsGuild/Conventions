using Microsoft.CodeAnalysis;

namespace Clavus;

internal record ConventionAttributeData
(
    INamedTypeSymbol LivePartAttribute,
    INamedTypeSymbol UnitTestPartAttribute,
    INamedTypeSymbol ClavusCategoryAttribute,
    ClavusConfigurationData Configuration
)
{
    public static ConventionAttributeData Create(ClavusConfigurationData data, Compilation compilation)
    {
        // ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var liveConventionAttribute = compilation.GetTypeByMetadataName("Clavus.LivePartAttribute")!;
        var unitTestConventionAttribute = compilation.GetTypeByMetadataName("Clavus.UnitTestPartAttribute")!;
        var conventionCategoryAttribute = compilation.GetTypeByMetadataName("Clavus.ClavusCategoryAttribute")!;
        // ReSharper enable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return new(liveConventionAttribute, unitTestConventionAttribute, conventionCategoryAttribute, data);
    }

    public string? Namespace => Configuration.Namespace;
}
