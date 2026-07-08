using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Clavus.Analyzers.Tests;

/// <summary>
///     Task 7.2: confirm incrementality - unrelated source edits do not invalidate the
///     configuration generator pipeline stage, per design.md's Risks section ("config discovery
///     reuses the existing `AdditionalFiles`/`AnalyzerConfigOptionsProvider` machinery already
///     read for `ClavusMetadata` etc., and is keyed so unrelated source edits don't invalidate the
///     configuration pipeline stage").
///
///     NOTE: `test/Analyzers.Tests` has no existing incrementality-snapshot test to mirror at the
///     time this was written (grep across the repo turned up none), so "per existing
///     incrementality snapshot conventions" in tasks.md 7.2 is aspirational rather than an
///     established pattern. This test instead drives a raw `CSharpGeneratorDriver` directly
///     (bypassing the `GeneratorTestContextBuilder` convenience layer, which does not currently
///     expose `GeneratorDriverRunResult`/tracked steps) so it can assert on
///     `IncrementalGeneratorRunStep.Reasons` (`Cached` vs `New`/`Modified`) the standard Roslyn
///     way. It assumes the real generator pipeline calls `.WithTrackingName(...)` on its
///     configuration-discovery stage(s) - update `ConfigDiscoveryTrackingName` below to match
///     whatever name Ripley's implementation actually uses.
/// </summary>
public class ConfigurationIncrementalityTests() : ConfigGeneratorTest()
{
    /// <summary>ASSUMPTION: placeholder tracking name for the config-discovery incremental stage.</summary>
    private const string ConfigDiscoveryTrackingName = "clavus:configuration_source_files";

    [Test]
    public void Unrelated_Source_Edit_Should_Not_Re_Execute_The_Configuration_Discovery_Stage()
    {
        var references = AppDomain
                         .CurrentDomain
                         .GetAssemblies()
                         .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                         .Select(a => (MetadataReference)MetadataReference.CreateFromFile(a.Location))
                         .ToImmutableArray();

        const string configJson = /*lang=json,strict*/ """{ "Sample": { "Name": "value" } }""";

        var additionalText = new InMemoryAdditionalText("appsettings.json", configJson);

        var optionsProvider = new SingleFileAnalyzerConfigOptionsProvider(
            "appsettings.json",
            new Dictionary<string, string> { ["build_metadata.AdditionalFiles.ClavusConfigFormat"] = "Json", },
            new Dictionary<string, string> { ["build_property.EnableClavusConfiguration"] = "true", }
        );

        var generator = new ClavusAttributesGenerator();
        var driver = CSharpGeneratorDriver
                    .Create([generator.AsSourceGenerator(),], [additionalText,], optionsProvider: optionsProvider)
                    .WithUpdatedAnalyzerConfigOptions(optionsProvider);

        var unrelatedSource1 = CSharpSyntaxTree.ParseText("namespace Sample; public class Unrelated1;");
        var compilation1 = CSharpCompilation.Create("IncrementalityCheck", [unrelatedSource1,], references);

        var runResult1 = driver.RunGenerators(compilation1).GetRunResult();
        var step1 = runResult1
                   .Results
                   .SelectMany(r => r.TrackedSteps)
                   .Where(kvp => kvp.Key == ConfigDiscoveryTrackingName)
                   .SelectMany(kvp => kvp.Value)
                   .ToImmutableArray();

        // First run: everything is necessarily "New".
        step1.ShouldNotBeEmpty();

        // Now apply an entirely unrelated source edit (a new, unrelated class) and re-run the
        // same driver instance so Roslyn's incremental caching applies.
        var unrelatedSource2 = CSharpSyntaxTree.ParseText("namespace Sample; public class Unrelated1; public class Unrelated2;");
        var compilation2 = compilation1.ReplaceSyntaxTree(unrelatedSource1, unrelatedSource2);

        var driverAfterFirstRun = driver.RunGenerators(compilation1);
        var driver2 = driverAfterFirstRun.RunGenerators(compilation2);
        var runResult2 = driver2.GetRunResult();

        var step2 = runResult2
                   .Results
                   .SelectMany(r => r.TrackedSteps)
                   .Where(kvp => kvp.Key == ConfigDiscoveryTrackingName)
                   .SelectMany(kvp => kvp.Value)
                   .ToImmutableArray();

        // The configuration-discovery stage's outputs must all be reported as Cached/Unchanged -
        // an unrelated source edit must not cause the config pipeline stage to re-execute.
        step2.ShouldNotBeEmpty();
        step2
           .SelectMany(s => s.Outputs)
           .ShouldAllBe(o => o.Reason == IncrementalStepRunReason.Cached || o.Reason == IncrementalStepRunReason.Unchanged);
    }

    private sealed class InMemoryAdditionalText(string path, string text) : AdditionalText
    {
        public override string Path { get; } = path;

        public override SourceText? GetText(CancellationToken cancellationToken = default) => SourceText.From(text);
    }

    private sealed class SingleFileAnalyzerConfigOptionsProvider(
        string path,
        IReadOnlyDictionary<string, string> fileOptions,
        IReadOnlyDictionary<string, string> globalOptions
    ) : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GlobalOptions { get; } = new DictionaryAnalyzerConfigOptions(globalOptions);

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new DictionaryAnalyzerConfigOptions(new Dictionary<string, string>());

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            textFile.Path == path ? new DictionaryAnalyzerConfigOptions(fileOptions) : new DictionaryAnalyzerConfigOptions(new Dictionary<string, string>());
    }

    private sealed class DictionaryAnalyzerConfigOptions(IReadOnlyDictionary<string, string> options) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value)
        {
            if (options.TryGetValue(key, out var v))
            {
                value = v;
                return true;
            }

            value = "";
            return false;
        }
    }
}
