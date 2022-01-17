using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions;

internal record ConventionAttributeData(
    string Namespace, INamedTypeSymbol LiveConventionAttribute, INamedTypeSymbol ExportCandidates, INamedTypeSymbol UnitTestConventionAttribute,
    INamedTypeSymbol AfterConventionAttribute, INamedTypeSymbol DependsOnConventionAttribute, INamedTypeSymbol BeforeConventionAttribute,
    INamedTypeSymbol DependentOfConventionAttribute
)
{
    public static ConventionAttributeData Create(string @namespace, Compilation compilation)
    {
        var liveConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.LiveConventionAttribute")!;
        var exportConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.ExportConventionAttribute")!;
        var unitTestConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.UnitTestConventionAttribute")!;
        var afterConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.AfterConventionAttribute")!;
        var dependsOnConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.DependsOnConventionAttribute")!;
        var beforeConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.BeforeConventionAttribute")!;
        var dependentOfConventionAttribute = compilation.GetTypeByMetadataName("Rocket.Surgery.Conventions.DependentOfConventionAttribute")!;
        return new(
            @namespace,
            liveConventionAttribute!,
            exportConventionAttribute!,
            unitTestConventionAttribute!,
            afterConventionAttribute!,
            dependsOnConventionAttribute!,
            beforeConventionAttribute!,
            dependentOfConventionAttribute!
        );
    }
}
