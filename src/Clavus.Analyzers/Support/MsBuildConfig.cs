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
);
