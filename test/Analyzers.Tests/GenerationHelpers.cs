using System.Collections.Immutable;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public static class GenerationHelpers
{
#pragma warning disable CA1810
    static GenerationHelpers()
#pragma warning restore CA1810
    {
        // this "core assemblies hack" is from https://stackoverflow.com/a/47196516/4418060
        var coreAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var coreAssemblyNames = new[]
        {
            "mscorlib.dll",
            "netstandard.dll",
            "System.dll",
            "System.Core.dll",
#if NETCOREAPP
            "System.Private.CoreLib.dll",
#endif
            "System.Runtime.dll",
        };
        var coreMetaReferences =
            coreAssemblyNames.Select(x => MetadataReference.CreateFromFile(Path.Combine(coreAssemblyPath, x)));
        MetadataReferences = coreMetaReferences.ToImmutableArray();
        AllGenerators = typeof(ConventionAttributesGenerator).Assembly.GetTypes()
                                                             .Where(
                                                                  z => typeof(IIncrementalGenerator).IsAssignableFrom(z)
                                                                    && z.GetCustomAttributes<GeneratorAttribute>().Any()
                                                              ).ToImmutableArray();
    }

    public static ImmutableArray<Type> AllGenerators { get; }

    internal const string CrLf = "\r\n";
    internal const string Lf = "\n";
    internal const string DefaultFilePathPrefix = "Test";
    internal const string CSharpDefaultFileExt = "cs";
    internal const string TestProjectName = "TestProject";

    // internal static readonly string NormalizedPreamble = NormalizeToLf(Preamble.GeneratedByATool + Lf);

    internal static readonly ImmutableArray<PortableExecutableReference> MetadataReferences;

    public static async Task AssertGeneratedAsExpected<T>(
        IEnumerable<Assembly> metadataReferences,
        string source,
        string expected,
        string? expectedFileHint = null,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(new[] { source }, metadataReferences, Array.Empty<MetadataReference>(), properties);
        // normalize line endings to just LF
        var generatedText = generatedTree
                           .Where(z => string.IsNullOrWhiteSpace(expectedFileHint) || z.FilePath.Contains(expectedFileHint, StringComparison.OrdinalIgnoreCase))
                           .Select(z => NormalizeToLf(z.GetText().ToString()));
        // and append preamble to the expected
        var expectedText = NormalizeToLf(expected).Trim();
        generatedText.Last().Should().Be(expectedText);
    }


    public static async Task AssertGeneratedAsExpected<T>(
        IEnumerable<MetadataReference> metadataReferences,
        string source,
        string expected,
        string? expectedFileHint = null,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(new[] { source }, Array.Empty<Assembly>(), metadataReferences, properties);
        // normalize line endings to just LF
        var generatedText = generatedTree
                           .Where(z => string.IsNullOrWhiteSpace(expectedFileHint) || z.FilePath.Contains(expectedFileHint, StringComparison.OrdinalIgnoreCase))
                           .Select(z => NormalizeToLf(z.GetText().ToString()));
        // and append preamble to the expected
        var expectedText = NormalizeToLf(expected).Trim();
        generatedText.Last().Should().Be(expectedText);
    }

    public static async Task AssertGeneratedAsExpected<T>(
        IEnumerable<Assembly> metadataReferences,
        IEnumerable<string> sources,
        IEnumerable<string> expected,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(sources, metadataReferences, Array.Empty<MetadataReference>(), properties);
        // normalize line endings to just LF
        var generatedText = generatedTree.Select(z => NormalizeToLf(z.GetText().ToString())).ToArray();
        // and append preamble to the expected
        var expectedText = expected.Select(z => NormalizeToLf(z).Trim()).ToArray();

        generatedText.Should().HaveCount(expectedText.Length);
        foreach (var (generated, expectedTxt) in generatedText.Zip(expectedText, (generated, expected) => ( generated, expected )))
        {
            generated.Should().Be(expectedTxt);
        }
    }

    public static async Task AssertGeneratedAsExpected<T>(
        IEnumerable<MetadataReference> metadataReferences,
        IEnumerable<string> sources,
        IEnumerable<string> expected,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(sources, Array.Empty<Assembly>(), metadataReferences, properties);
        // normalize line endings to just LF
        var generatedText = generatedTree.Select(z => NormalizeToLf(z.GetText().ToString())).ToArray();
        // and append preamble to the expected
        var expectedText = expected.Select(z => NormalizeToLf(z).Trim()).ToArray();

        generatedText.Should().HaveCount(expectedText.Length);
        foreach (var (generated, expectedTxt) in generatedText.Zip(expectedText, (generated, expected) => ( generated, expected )))
        {
            generated.Should().Be(expectedTxt);
        }
    }

    public static async Task AssertGeneratedAsExpected<T>(
        string source,
        string expected,
        string? expectedFileHint = null,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(new[] { source }, Array.Empty<Assembly>(), Array.Empty<MetadataReference>(), properties)
           ;
        // normalize line endings to just LF
        var generatedText = generatedTree
                           .Where(z => string.IsNullOrWhiteSpace(expectedFileHint) || z.FilePath.Contains(expectedFileHint, StringComparison.OrdinalIgnoreCase))
                           .Select(z => NormalizeToLf(z.GetText().ToString()));
        // and append preamble to the expected
        var expectedText = NormalizeToLf(expected).Trim();
        generatedText.Last().Should().Be(expectedText);
    }

    public static async Task<string> Generate<T>(
        IEnumerable<Assembly> metadataReferences,
        string source,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(new[] { source }, metadataReferences, Array.Empty<MetadataReference>(), properties);
        // normalize line endings to just LF
        var generatedText = generatedTree.Select(z => NormalizeToLf(z.GetText().ToString()));
        // and append preamble to the expected
        return generatedText.Last();
    }

    public static async Task<string> Generate<T>(
        IEnumerable<MetadataReference> metadataReferences,
        string source,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(new[] { source }, Array.Empty<Assembly>(), metadataReferences, properties);
        // normalize line endings to just LF
        var generatedText = generatedTree.Select(z => NormalizeToLf(z.GetText().ToString()));
        // and append preamble to the expected
        return generatedText.Last();
    }

    public static async Task<string[]> Generate<T>(
        IEnumerable<Assembly> metadataReferences,
        IEnumerable<string> sources,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(sources, metadataReferences, Array.Empty<MetadataReference>(), properties);
        // normalize line endings to just LF
        var generatedText = generatedTree.Select(z => NormalizeToLf(z.GetText().ToString()));
        // and append preamble to the expected
        return generatedText.ToArray();
    }

    public static async Task<string[]> Generate<T>(
        IEnumerable<MetadataReference> metadataReferences,
        IEnumerable<string> sources,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(sources, Array.Empty<Assembly>(), metadataReferences, properties);
        // normalize line endings to just LF
        var generatedText = generatedTree.Select(z => NormalizeToLf(z.GetText().ToString()));
        // and append preamble to the expected
        return generatedText.ToArray();
    }

    public static async Task<string> Generate<T>(
        string source,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var generatedTree = await GenerateAsync<T>(new[] { source }, Array.Empty<Assembly>(), Array.Empty<MetadataReference>(), properties)
           ;
        // normalize line endings to just LF
        var generatedText = generatedTree.Select(z => NormalizeToLf(z.GetText().ToString()));
        // and append preamble to the expected
        return generatedText.Last();
    }

    public static async Task<IEnumerable<SyntaxTree>> GenerateAsync<T>(
        IEnumerable<string> sources,
        IEnumerable<Assembly> metadataReferences,
        IEnumerable<MetadataReference> compilationReferences,
        IDictionary<string, string?>? properties
    )
        where T : new()
    {
        // Assert.Empty(diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning));

        var (outputCompilation, startingSyntaxTress) = await InnerGenerateCompilationAsync<T>(sources, metadataReferences, compilationReferences, properties);

        // the syntax tree added by the generator will be the last one in the compilation
        return outputCompilation.SyntaxTrees.TakeLast(
            outputCompilation.SyntaxTrees.Count() - startingSyntaxTress
        );
    }

    public static Task<GeneratorDriverRunResult> GenerateAll(
        string source,
        IEnumerable<Assembly>? metadataReferences = null,
        IEnumerable<MetadataReference>? compilationReferences = null,
        IDictionary<string, string?>? properties = null
    )
    {
        return GenerateAll(new[] { source }, metadataReferences, compilationReferences, properties);
    }

    public static async Task<GeneratorDriverRunResult> GenerateAll(
        IEnumerable<string> sources,
        IEnumerable<Assembly>? metadataReferences = null,
        IEnumerable<MetadataReference>? compilationReferences = null,
        IDictionary<string, string?>? properties = null
    )
    {
        var references = ( metadataReferences ?? Array.Empty<Assembly>() )
                        .Concat(
                             new[]
                             {
                                 typeof(ActivatorUtilities).Assembly,
                                 typeof(ConventionAttribute).Assembly,
                                 typeof(ConventionContext).Assembly,
                                 typeof(IConventionContext).Assembly,
                                 typeof(IServiceProvider).Assembly,
                             }
                         )
                        .Distinct()
                        .Select(x => MetadataReference.CreateFromFile(x.Location))
                        .Concat(compilationReferences ?? Array.Empty<MetadataReference>())
                        .ToArray();
        var project = CreateProject(references, TestProjectName, sources.ToArray());

        var compilation = (CSharpCompilation)( await project.GetCompilationAsync() )!;
        if (compilation is null)
        {
            throw new InvalidOperationException("Could not compile the sources");
        }

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            AllGenerators.Select(z => (Activator.CreateInstance(z) as IIncrementalGenerator)!).Select(z => z.AsSourceGenerator()).ToImmutableArray(),
            ImmutableArray<AdditionalText>.Empty,
            compilation.SyntaxTrees[0].Options as CSharpParseOptions,
            new OptionsProvider(properties ?? new Dictionary<string, string?>())
        );

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        return driver.GetRunResult();
    }

    private static async Task<(Compilation compilation, int trees)> InnerGenerateCompilationAsync<T>(
        IEnumerable<string> sources,
        IEnumerable<Assembly> metadataReferences,
        IEnumerable<MetadataReference> compilationReferences,
        IDictionary<string, string?>? properties,
        string? projectName = null
    )
        where T : new()
    {
        var references = metadataReferences
                        .Concat(
                             new[]
                             {
                                 typeof(ActivatorUtilities).Assembly,
                                 typeof(ConventionAttribute).Assembly,
                                 typeof(ConventionContext).Assembly,
                                 typeof(IConventionContext).Assembly,
                                 typeof(IServiceProvider).Assembly,
                             }
                         )
                        .Distinct()
                        .Select(x => MetadataReference.CreateFromFile(x.Location))
                        .Concat(compilationReferences)
                        .ToArray();
        var project = CreateProject(references, projectName ?? TestProjectName, sources.ToArray());

        var compilation = (CSharpCompilation?)await project.GetCompilationAsync();
        if (compilation is null)
        {
            throw new InvalidOperationException("Could not compile the sources");
        }

        var startingSyntaxTress = compilation.SyntaxTrees.Length;

        var diagnostics = compilation.GetDiagnostics();
        Assert.Empty(diagnostics.Where(z => z.Id != "CS8632").Where(x => x.Severity >= DiagnosticSeverity.Warning));

        var generator = new T();
        var driver =
            generator is ISourceGenerator sourceGenerator ? CSharpGeneratorDriver.Create(
                new[] { sourceGenerator }, optionsProvider: new OptionsProvider(properties ?? new Dictionary<string, string?>())
            ) :
            generator is IIncrementalGenerator incrementalGenerator ? CSharpGeneratorDriver.Create(
                new[] { incrementalGenerator }.Select(GeneratorExtensions.AsSourceGenerator),
                optionsProvider: new OptionsProvider(properties ?? new Dictionary<string, string?>())
            ) :
            throw new NotSupportedException("Generator type not supported");

        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out diagnostics);
        // Assert.Empty(diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning));

        // the syntax tree added by the generator will be the last one in the compilation
        return ( outputCompilation, startingSyntaxTress );
    }

    public static async Task<MetadataReference> GenerateCompilationAsync<T>(
        IEnumerable<string> sources,
        IEnumerable<Assembly> metadataReferences,
        IEnumerable<MetadataReference> compilationReferences,
        string projectName,
        IDictionary<string, string?>? properties = null
    )
        where T : new()
    {
        var compilation = ( await InnerGenerateCompilationAsync<T>(sources, metadataReferences, compilationReferences, properties, projectName) ).compilation;

        var memoryStream = new MemoryStream();
        var result = compilation.Emit(memoryStream, options: new EmitOptions(outputNameOverride: projectName));
        if (!result.Success) throw new Exception("Error compiling compilation");
        memoryStream.Seek(0, SeekOrigin.Begin);
        return MetadataReference.CreateFromStream(memoryStream, filePath: projectName);
    }

    public static Project CreateProject(IEnumerable<MetadataReference>? metadataReferences, string projectName, params string[] sources)
    {
        var projectId = ProjectId.CreateNewId(projectName);
        var solution = new AdhocWorkspace()
                      .CurrentSolution
                      .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
                      .WithProjectCompilationOptions(
                           projectId,
                           new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                       )
                      .WithProjectAssemblyName(projectId, projectName)
                      .WithProjectParseOptions(
                           projectId,
                           new CSharpParseOptions(preprocessorSymbols: new[] { "SOMETHING_ACTIVE" })
                       )
                      .AddMetadataReferences(projectId, MetadataReferences.Concat(metadataReferences ?? Array.Empty<MetadataReference>()));

        var count = 0;
        foreach (var source in sources)
        {
            var newFileName = DefaultFilePathPrefix + count + "." + CSharpDefaultFileExt;
            var documentId = DocumentId.CreateNewId(projectId, newFileName);
            solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
            count++;
        }

        var project = solution.GetProject(projectId);
        if (project is null)
        {
            throw new InvalidOperationException($"The ad hoc workspace does not contain a project with the id {projectId.Id}");
        }

        return project;
    }

    private sealed class OptionsProvider(IDictionary<string, string?> properties) : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        {
            return GlobalOptions;
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            return GlobalOptions;
        }

        public override AnalyzerConfigOptions GlobalOptions { get; } = new OptionsObject(properties);
    }

    private sealed class OptionsObject : AnalyzerConfigOptions
    {
        private readonly IDictionary<string, string?> _properties;

        public OptionsObject(IDictionary<string, string?> properties)
        {
            _properties = properties.ToDictionary(z => $"build_property.{z.Key}", z => z.Value);
        }

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            return _properties.TryGetValue(key, out value);
        }
    }

    public static string NormalizeToLf(string input)
    {
        return input.Replace(CrLf, Lf, StringComparison.Ordinal);
    }

    public static async Task<IEnumerable<MetadataReference>> CreateDeps()
    {
        var c1 = await Class1();
        var c2 = await Class2();
        var c3 = await Class3(c1);
        return new[] { c1, c2, c3 };
    }

    public static Task<MetadataReference> Class1()
    {
        return GenerateCompilationAsync<ConventionAttributesGenerator>(
            new[]
            {
                @"using Rocket.Surgery.Conventions;
using Sample.DependencyOne;

[assembly: ExportConventions(Namespace = ""Dep1"", ClassName = ""Dep1Exports"")]
[assembly: Convention(typeof(Class1))]

namespace Sample.DependencyOne;

public class Class1 : IConvention
{
}
"
            }, ImmutableArray<Assembly>.Empty, ImmutableArray<MetadataReference>.Empty,
            "SampleDependencyOne"
        );
    }

    public static Task<MetadataReference> Class2()
    {
        return GenerateCompilationAsync<ConventionAttributesGenerator>(
            new[]
            {
                @"using Rocket.Surgery.Conventions;
using Sample.DependencyTwo;

[assembly: ExportConventions(Namespace = null, ClassName = ""Dep2Exports"")]
[assembly: Convention(typeof(Class2))]

namespace Sample.DependencyTwo;

public class Class2 : IConvention
{
}"
            }, ImmutableArray<Assembly>.Empty, ImmutableArray<MetadataReference>.Empty,
            "SampleDependencyTwo"
        );
    }

    public static Task<MetadataReference> Class3(MetadataReference class1)
    {
        return GenerateCompilationAsync<ConventionAttributesGenerator>(
            new[]
            {
                @"using Rocket.Surgery.Conventions;
using Sample.DependencyOne;
using Sample.DependencyThree;

[assembly: Convention(typeof(Class3))]

namespace Sample.DependencyThree;

public class Class3 : IConvention
{
    public Class1? Class1 { get; set; }
}
"
            }, ImmutableArray<Assembly>.Empty, ImmutableArray.Create(class1),
            "SampleDependencyThree"
        );
    }
}
