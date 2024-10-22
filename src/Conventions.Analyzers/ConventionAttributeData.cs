using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions;

internal record ConventionAttributeData
(
    INamedTypeSymbol LiveConventionAttribute,
    INamedTypeSymbol UnitTestConventionAttribute,
    INamedTypeSymbol ConventionCategoryAttribute,
    ConventionConfigurationData Configuration
)
{
    public static ConventionAttributeData Create(ConventionConfigurationData data, Compilation compilation)
    {
        // ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var liveConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.LiveConventionAttribute")!;
        var unitTestConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.UnitTestConventionAttribute")!;
        var conventionCategoryAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.ConventionCategoryAttribute")!;
        // ReSharper enable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return new(liveConventionAttribute, unitTestConventionAttribute, conventionCategoryAttribute, data);
    }

    public string? Namespace => Configuration.Namespace;
}
