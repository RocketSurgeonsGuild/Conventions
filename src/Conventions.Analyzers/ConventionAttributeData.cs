using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions;

internal record ConventionAttributeData(
    INamedTypeSymbol LiveConventionAttribute, INamedTypeSymbol ExportCandidates, INamedTypeSymbol UnitTestConventionAttribute,
    INamedTypeSymbol AfterConventionAttribute, INamedTypeSymbol DependsOnConventionAttribute, INamedTypeSymbol BeforeConventionAttribute,
    INamedTypeSymbol DependentOfConventionAttribute,
    ConventionConfigurationData Configuration
)
{
    public string Namespace => Configuration.Namespace;

    public static ConventionAttributeData Create(ConventionConfigurationData data, Compilation compilation)
    {
        var liveConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.LiveConventionAttribute")!;
        var exportConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.ExportConventionAttribute")!;
        var unitTestConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.UnitTestConventionAttribute")!;
        var afterConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.AfterConventionAttribute")!;
        var dependsOnConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.DependsOnConventionAttribute")!;
        var beforeConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.BeforeConventionAttribute")!;
        var dependentOfConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.DependentOfConventionAttribute")!;
        return new(
            liveConventionAttribute!,
            exportConventionAttribute!,
            unitTestConventionAttribute!,
            afterConventionAttribute!,
            dependsOnConventionAttribute!,
            beforeConventionAttribute!,
            dependentOfConventionAttribute!,
            data
        );
    }
}
