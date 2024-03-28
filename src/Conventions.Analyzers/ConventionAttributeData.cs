using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions;

internal record ConventionAttributeData
(
    INamedTypeSymbol LiveConventionAttribute,
    INamedTypeSymbol UnitTestConventionAttribute,
    ConventionConfigurationData Configuration
)
{
    public static ConventionAttributeData Create(ConventionConfigurationData data, Compilation compilation)
    {
        // ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var liveConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.LiveConventionAttribute")!;
        var unitTestConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.UnitTestConventionAttribute")!;
        // ReSharper enable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return new(liveConventionAttribute, unitTestConventionAttribute, data);
    }

    public string? Namespace => Configuration.Namespace;
}