using System.Collections.Immutable;

namespace Clavus.Support.Configuration;

/// <summary>
///     The on-disk format of a discovered <c>ClavusConfiguration</c> file, used to select the generator-side
///     shape reader and the runtime <c>IConfigurationSource</c> to register.
/// </summary>
internal enum ConfigurationFileFormat
{
    Json,
    Yaml,
    Toml,
}

/// <summary>
///     A single discovered <c>ClavusConfiguration</c> layer file (base/environment/local), as surfaced to the
///     generator via <c>AdditionalFiles</c>.
/// </summary>
/// <remarks>
///     <b>Assumption (Parker's MSBuild surface, task 1.4, not yet visible in this worktree):</b> each
///     <c>ClavusConfiguration</c> item is surfaced as an <see cref="Microsoft.CodeAnalysis.AdditionalText" /> whose
///     per-file <c>AnalyzerConfigOptions</c> carry:
///     <list type="bullet">
///         <item><c>build_metadata.AdditionalFiles.ClavusConfiguration</c> = <c>"true"</c> (marks the item)</item>
///         <item>
///             <c>build_metadata.AdditionalFiles.ClavusConfigurationBaseName</c> = the layering group key (e.g.
///             <c>appsettings</c>); falls back to the file name without extension/environment segment when absent
///         </item>
///     </list>
///     Format is derived from the file extension rather than a separate metadata key. The base/environment/local
///     layer isn't tracked as metadata at all — <see cref="ConfigurationDiscovery" /> derives it on demand from
///     the file name, so there's nothing extra to keep in sync on the MSBuild side. If Parker's actual plumbing
///     differs, only <see cref="ConfigurationDiscovery" /> needs to change.
/// </remarks>
internal readonly record struct ConfigurationSourceFile(
    string BaseName,
    ConfigurationFileFormat Format,
    string FilePath,
    string Content);

/// <summary>
///     All layers (base/environment/local) discovered for a single configuration base name, within one project.
/// </summary>
internal sealed record ConfigurationFileGroup(
    string BaseName,
    ConfigurationFileFormat Format,
    string RelativePath,
    ImmutableArray<KeyValuePair<string, string?>> FlatValues);
