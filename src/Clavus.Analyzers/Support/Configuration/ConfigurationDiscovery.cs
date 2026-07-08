using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Clavus.Support.Configuration;

/// <summary>
///     Wires up the incremental pipeline stage that discovers <c>ClavusConfiguration</c> items from
///     <c>AdditionalFiles</c> (task 1.4's consumer side) and groups their layers (base/environment/local) into one
///     <see cref="ConfigurationFileGroup" /> per base name.
/// </summary>
/// <remarks>
///     See the "Assumption" remarks on <see cref="ConfigurationSourceFile" /> for the exact per-file metadata
///     shape this expects from Parker's MSBuild surface (task 1.1-1.4), which is not yet present in this worktree.
/// </remarks>
internal static class ConfigurationDiscovery
{
    public static IncrementalValuesProvider<ConfigurationSourceFile> GetSourceFiles(IncrementalGeneratorInitializationContext context) =>
        context
           .AdditionalTextsProvider
           .Combine(context.AnalyzerConfigOptionsProvider)
           .Select(static (pair, ct) => TryRead(pair.Left, pair.Right, ct))
           .Where(static x => x.HasValue)
           .Select(static (x, _) => x!.Value)
           .WithTrackingName("clavus:configuration_files");

    private static ConfigurationSourceFile? TryRead(AdditionalText text, AnalyzerConfigOptionsProvider optionsProvider, CancellationToken cancellationToken)
    {
        var options = optionsProvider.GetOptions(text);
        if (!options.TryGetValue("build_metadata.AdditionalFiles.ClavusConfigFormat", out var formatMetadata) || formatMetadata is not { Length: > 0, })
        {
            return null;
        }

        var format = formatMetadata.ToUpperInvariant() switch
        {
            "JSON" => ConfigurationFileFormat.Json,
            "YAML" => ConfigurationFileFormat.Yaml,
            "TOML" => ConfigurationFileFormat.Toml,
            _ => InferFormat(text.Path),
        };
        if (format is null) return null;

        options.TryGetValue("build_metadata.AdditionalFiles.ClavusConfigBaseName", out var baseName);

        var resolvedBaseName = baseName is { Length: > 0, } ? baseName : DefaultBaseName(text.Path);
        var content = text.GetText(cancellationToken)?.ToString() ?? "";

        return new ConfigurationSourceFile(resolvedBaseName, format.Value, text.Path, content);
    }

    /// <summary>
    ///     Groups discovered source files by base name. The <c>Base</c> layer's own file name is used as the
    ///     generated part's relative path (task 4.2); flattened key/value pairs are unioned across every layer
    ///     (Base, then Environment, then Local) so a key introduced only in an environment/local override still
    ///     contributes to shape inference, without any one layer's absence breaking generation.
    /// </summary>
    public static ImmutableArray<ConfigurationFileGroup> GroupJsonFiles(ImmutableArray<ConfigurationSourceFile> files)
    {
        var groups = ImmutableArray.CreateBuilder<ConfigurationFileGroup>();
        foreach (var byName in files.GroupBy(f => f.BaseName, StringComparer.OrdinalIgnoreCase).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            var ordered = byName.OrderBy(LayerOrder).ToImmutableArray();
            var format = ordered[0].Format;
            if (format != ConfigurationFileFormat.Json) continue; // JSON-only shape inference for now (task 3.1 scope).

            var flatValues = ImmutableArray.CreateBuilder<KeyValuePair<string, string?>>();
            foreach (var file in ordered)
            {
                if (!JsonFlatConfigurationReader.TryRead(file.Content, out var values)) continue;
                flatValues.AddRange(values);
            }

            var relativePath = ordered.FirstOrDefault(f => LayerOrder(f) == 0).FilePath is { Length: > 0, } basePath
                ? Path.GetFileName(basePath)
                : Path.GetFileName(ordered[0].FilePath);

            groups.Add(new(byName.Key, format, relativePath, flatValues.ToImmutable()));
        }

        return groups.ToImmutable();
    }

    /// <summary>All discovered groups regardless of format, for marker-attribute emission (task 2.6, format-agnostic).</summary>
    public static ImmutableArray<ConfigurationFileGroup> GroupAllFiles(ImmutableArray<ConfigurationSourceFile> files)
    {
        var groups = ImmutableArray.CreateBuilder<ConfigurationFileGroup>();
        foreach (var byName in files.GroupBy(f => f.BaseName, StringComparer.OrdinalIgnoreCase).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            var ordered = byName.OrderBy(LayerOrder).ToImmutableArray();
            var relativePath = ordered.FirstOrDefault(f => LayerOrder(f) == 0).FilePath is { Length: > 0, } basePath
                ? Path.GetFileName(basePath)
                : Path.GetFileName(ordered[0].FilePath);

            groups.Add(new(byName.Key, ordered[0].Format, relativePath, []));
        }

        return groups.ToImmutable();
    }

    /// <summary>
    ///     Derives the base (0) / environment (1) / local (2) resolution order for a file from its name rather
    ///     than a tracked "Layer" value — e.g. for BaseName "appsettings": "appsettings.json" is Base,
    ///     "appsettings.local.json" is Local, and anything else matching "appsettings.*" is Environment.
    /// </summary>
    private static int LayerOrder(ConfigurationSourceFile file)
    {
        var name = Path.GetFileNameWithoutExtension(file.FilePath);
        if (!name.StartsWith(file.BaseName, StringComparison.OrdinalIgnoreCase) || name.Length == file.BaseName.Length) return 0;

        var suffix = name[( file.BaseName.Length + 1 )..];
        return string.Equals(suffix, "local", StringComparison.OrdinalIgnoreCase) ? 2 : 1;
    }

    private static ConfigurationFileFormat? InferFormat(string path) =>
        Path.GetExtension(path).ToUpperInvariant() switch
        {
            ".JSON" => ConfigurationFileFormat.Json,
            ".YAML" or ".YML" => ConfigurationFileFormat.Yaml,
            ".TOML" => ConfigurationFileFormat.Toml,
            _ => null,
        };

    private static string DefaultBaseName(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        // Strip a trailing ".local" or ".<Environment>" segment (e.g. "appsettings.local" -> "appsettings").
        var dot = name.IndexOf('.');
        return dot > 0 ? name[..dot] : name;
    }
}
