namespace Clavus.Support;

internal record MsBuildConfig
(
    bool ClavusMetadata,
    bool AssignExternal,
    bool IsTestProject,
    string RootNamespace,
    string HostType,
    string Category,
    ClavusConfigurationData ExportConfiguration,
    ClavusConfigurationData ImportConfiguration
)
{
    public ClavusConfigurationData ImportConfiguration { get; init; } = ImportConfiguration with { Namespace = ImportConfiguration.Namespace is "" ? RootNamespace : ImportConfiguration.Namespace };
    public ClavusConfigurationData ExportConfiguration { get; init; } = ExportConfiguration with { Namespace = ExportConfiguration.Namespace is "" ? RootNamespace : ExportConfiguration.Namespace };
}
