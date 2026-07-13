using System.Collections.Frozen;
using System.Reflection;
using System.Xml.Linq;
using DiffEngine;
using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.Build.Utilities.ProjectCreation;

// MSBuild.ProjectCreation shares a global ProjectRootElementCache; concurrent tests that create
// projects with the same relative file name (e.g. "Directory.Build.props") race on that cache and
// throw KeyNotFoundException. Serialize the whole assembly.
[assembly: NotInParallel]

namespace Clavus.Sdk.Tests;

/// <summary>
/// Pairs a build's <see cref="ProjectEvaluation"/> (evaluation-time snapshot: properties, static
/// items, imports) with two sets of filenames <see cref="SdkTestProject"/> reads straight off disk
/// after a successful build, because neither is visible in the evaluation snapshot itself:
/// <list type="bullet">
/// <item><see cref="GeneratedFiles"/> - the <c>*.g.cs</c> Compile items the SDK's targets wrote to
/// <c>obj/</c>, added by <c>BeforeTargets="CoreCompile"</c> targets at execution time.</item>
/// <item><see cref="OutputFiles"/> - the renamed/copied config files (e.g. <c>appsettings.yaml</c>
/// -&gt; <c>{ProjectName}.yaml</c>) that <c>Clavus.PackConfiguration.targets</c> copies to
/// <c>bin/</c>. That rename is driven by <c>&lt;None Update="..."&gt;</c> metadata against items the
/// .NET SDK's own default item globs (evaluated in <c>Microsoft.NET.Sdk.DefaultItems.props</c>,
/// before this project's own evaluation) add - MSBuild.StructuredLogger's evaluation-time Items
/// snapshot only records items added by the project's own body/imports, so those never show up
/// there even though the rename/copy genuinely happens (verified independently via
/// <c>dotnet build -getItem:None</c>).</item>
/// </list>
/// </summary>
internal sealed record ProjectEvaluationResult(ProjectEvaluation Evaluation, IReadOnlyList<string> GeneratedFiles, IReadOnlyList<string> OutputFiles);

internal static class Config
{
    [Before(HookType.Assembly)]
    public static void Setup(AssemblyHookContext context)
    {
        MSBuildAssemblyResolver.Register();
        VerifierSettings.InitializePlugins();

        DiffRunner.Disabled = true;
        VerifierSettings.AddExtraSettings(z => z.Converters.Add(new ProjectEvaluationSerializer()));
    }

    public static string TUnitVersion => field ??= typeof(TestAttribute).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion.Split('+')[0];

    private static readonly OrderedDictionary<string, string> _namedVersions = [];

    private class ProjectEvaluationSerializer : WriteOnlyJsonConverter<ProjectEvaluationResult>
    {
        private static readonly IReadOnlyCollection<string> PropertiesToWrite =
        [
            nameof(ProjectEvaluation.Configuration),
            nameof(ProjectEvaluation.Platform),
            nameof(ProjectEvaluation.TargetFramework),
        ];

        public override void Write(VerifyJsonWriter writer, ProjectEvaluationResult result)
        {
            var value = result.Evaluation;
            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            writer.WriteValue(value.ShortenedName);
            foreach (var item in PropertiesToWrite)
            {
                writer.WritePropertyName(item);
                writer.WriteValue(value.GetType().GetProperty(item)?.GetValue(value));
            }

            writer.WritePropertyName("Properties");
            writer.WriteStartObject();
            foreach (var property in value.GetProperties()
                         .Where(z => AllPropertyNames.Contains(z.Key))
                         .OrderBy(z => z.Key)
                    )
            {
                writer.WritePropertyName(property.Key);
                if (property.Key.StartsWith("RsgSdk_") && property.Key.EndsWith("_Version"))
                {
                    if (_namedVersions.TryGetValue(property.Value, out _, out var index))
                    {
                        writer.WriteValue($"Version_{index}");
                    }
                    else
                    {
                        _namedVersions.TryAdd(property.Value, property.Value, out index);
                        writer.WriteValue($"Version_{index}");
                    }
                    continue;
                }
                writer.WriteValue(property.Value);
            }

            writer.WriteEndObject();

            WriteAdditionalFiles(writer, value, "AdditionalFiles");
            WriteItems(writer, value, "PackageReference");
            WriteItems(writer, value, "ClavusPart");
            WriteItems(writer, value, "ClavusHost");
            WriteUsings(writer, value, "Using");
            WriteImports(writer, value, "Imports");

            writer.WritePropertyName("GeneratedFiles");
            writer.WriteStartArray();
            foreach (var fileName in result.GeneratedFiles.OrderBy(z => z, StringComparer.Ordinal))
            {
                writer.WriteValue(fileName);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("OutputFiles");
            writer.WriteStartArray();
            foreach (var fileName in result.OutputFiles.OrderBy(z => z, StringComparer.Ordinal))
            {
                writer.WriteValue(fileName);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private static void WriteAdditionalFiles(VerifyJsonWriter writer, ProjectEvaluation project, string name)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var item in GetItemGroup(project, name).OrderBy(z => z.Name))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Name");
                writer.WriteValue(item.Name);
                foreach (var value in item.Children.OfType<Metadata>().OrderBy(z => z.Name))
                {
                    // MSBuild synthesizes %(Link) for out-of-cone items based on a string-prefix
                    // comparison between the item's full path and the project directory; macOS's
                    // /var,/tmp -> /private/var,/private/tmp symlink makes that comparison behave
                    // differently than on Linux, so the same evaluation yields Link on one platform
                    // and not the other. Not meaningful test signal - skip it.
                    if (value.Name == "Link")
                    {
                        continue;
                    }

                    writer.WritePropertyName(value.Name);
                    if (value.Name == "Version")
                    {
                        if (_namedVersions.TryGetValue(value.Value, out _, out var index))
                        {
                            writer.WriteValue($"Version_{index}");
                        }
                        else
                        {
                            _namedVersions.TryAdd(value.Value, $"Version_{_namedVersions.Count + 1}", out index);
                            writer.WriteValue($"Version_{index}");
                        }
                        continue;
                    }

                    writer.WriteValue(value.Value);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private static void WriteImports(VerifyJsonWriter writer, ProjectEvaluation project, string name)
        {
            var (imports, noImports) = GetImportGroup(project);
            writer.WritePropertyName("Imports");

            writer.WriteStartArray();
            foreach (var item in imports)
            {
                writer.WriteValue(item.ProjectFilePath);
            }

            writer.WriteEndArray();

            writer.WritePropertyName("NoImports");
            writer.WriteStartArray();
            foreach (var item in noImports.GroupBy(z => z.ProjectFilePath))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("ProjectFilePath");
                writer.WriteValue(item.Key);
                writer.WritePropertyName("Imports");
                writer.WriteStartArray();
                foreach (var value in item)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("Path");
                    writer.WriteValue(value.Text);
                    writer.WritePropertyName("Reason");
                    writer.WriteValue(value.Reason);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        private static void WriteUsings(VerifyJsonWriter writer, ProjectEvaluation project, string name)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var item in GetItemGroup(project, name).OrderBy(z => z.Name))
            {
                writer.WriteValue(item.Name);
            }
            writer.WriteEndArray();
        }

        private static void WriteItems(VerifyJsonWriter writer, ProjectEvaluation project, string name)
        {
            writer.WritePropertyName(name);
            writer.WriteStartObject();
            foreach (var item in GetItemGroup(project, name).OrderBy(z => z.Name))
            {
                writer.WritePropertyName(item.Name);
                writer.WriteStartObject();
                foreach (var value in item.Children.OfType<Metadata>().OrderBy(z => z.Name))
                {
                    // MSBuild synthesizes %(Link) for out-of-cone items based on a string-prefix
                    // comparison between the item's full path and the project directory; macOS's
                    // /var,/tmp -> /private/var,/private/tmp symlink makes that comparison behave
                    // differently than on Linux, so the same evaluation yields Link on one platform
                    // and not the other. Not meaningful test signal - skip it.
                    if (value.Name == "Link")
                    {
                        continue;
                    }

                    writer.WritePropertyName(value.Name);
                    if (value.Name == "Version")
                    {
                        if (_namedVersions.TryGetValue(value.Value, out _, out var index))
                        {
                            writer.WriteValue($"Version_{index}");
                        }
                        else
                        {
                            _namedVersions.TryAdd(value.Value, $"Version_{_namedVersions.Count + 1}", out index);
                            writer.WriteValue($"Version_{index}");
                        }
                        continue;
                    }

                    writer.WriteValue(value.Value);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }

    private static IEnumerable<Item> GetItemGroup(ProjectEvaluation project, string name)
    {
        var items = project.Children.OfType<Folder>().Single(z => z.Name == "Items");
        return items.Children.OfType<AddItem>().SingleOrDefault(z => z.Name == name)?.Children.OfType<Item>() ?? [];
    }

    private static (IEnumerable<Import>, IEnumerable<NoImport>) GetImportGroup(ProjectEvaluation project)
    {
        var dotnetRoot = project.GetProperties()["DOTNET_ROOT"];
        var items = project.Children.OfType<TimedNode>().Single(z => z.Name == "Imports");
        var allImports = items.Children
            .Expand(z => z is TreeNode tn ? tn.Children : [])
            .ToArray();
        return (
            allImports.OfType<Import>().DistinctBy(z => z.ProjectFilePath).Where(z => !z.ProjectFilePath.StartsWith(dotnetRoot)).OrderBy(z => z.Text),
            allImports.OfType<NoImport>().DistinctBy(z => z.ProjectFilePath).Where(z => !z.ProjectFilePath.StartsWith(dotnetRoot)).OrderBy(z => z.Text)
        );
    }

    public static FrozenSet<string> AllPropertyNames => field ??= Directory.EnumerateFiles(Path.Combine(RootDirectory, "src"), "*.props", SearchOption.AllDirectories)
        .Concat(Directory.EnumerateFiles(Path.Combine(RootDirectory, "src"), "*.targets", SearchOption.AllDirectories))
        .SelectMany(z => XDocument.Parse(File.ReadAllText(z)).Document!.Descendants("PropertyGroup")
            .SelectMany(z => z.Elements().Select(e => e.Name.LocalName))
            .Distinct()
        )
        .ToFrozenSet();

    public static string RootDirectory => field ??= FindRootDirectory();
    public static string NugetArtifactsDirectory => field ??= Path.Combine(FindRootDirectory(), "artifacts", "nuget-local");

    /// <summary>
    /// Resolves the version a Clavus package was just packed with (see
    /// <c>PackClavusSourcePackages</c> in Clavus.Sdk.Tests.csproj, which repacks every src/*.csproj
    /// before each build), for use as an explicit <c>Version</c> on a <see cref="ProjectCreator"/>
    /// PackageReference item - scaffolded consumer projects have no central package management, so
    /// NuGet needs one.
    /// </summary>
    public static string ClavusPackageVersion(string packageId)
    {
        var prefix = packageId + ".";
        var match = Directory.EnumerateFiles(NugetArtifactsDirectory, $"{packageId}.*.nupkg")
            .Select(Path.GetFileName)
            .Where(f => f!.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && char.IsDigit(f[prefix.Length]))
            .OrderByDescending(f => f, StringComparer.Ordinal)
            .FirstOrDefault();
        return match is null
            ? throw new InvalidOperationException($"No packed nupkg found for '{packageId}' in {NugetArtifactsDirectory}.")
            : match[prefix.Length..^".nupkg".Length];
    }

    private static string FindRootDirectory()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "artifacts")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException("Could not locate the repo root directory. This test must be run from within the repo.");
    }
}
