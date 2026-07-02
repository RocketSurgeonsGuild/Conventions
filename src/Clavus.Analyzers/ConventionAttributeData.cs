using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Clavus;

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
        var liveConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Clavus.LivePartAttribute")!;
        var unitTestConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Clavus.UnitTestPartAttribute")!;
        var conventionCategoryAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Clavus.ClavusCategoryAttribute")!;
        // ReSharper enable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return new(liveConventionAttribute, unitTestConventionAttribute, conventionCategoryAttribute, data);
    }

    public string? Namespace => Configuration.Namespace;
}
