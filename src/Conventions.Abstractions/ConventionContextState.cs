using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions;

internal partial class ConventionContextState
{
    private readonly List<object?> _conventions = [null];
    private readonly List<Type> _exceptConventions = [];
    private readonly List<Assembly> _exceptAssemblyConventions = [];
    public ServiceProviderFactoryAdapter? ServiceProviderFactory { get; set; }

    public void AppendConventions(params IEnumerable<object> conventions)
    {
        _conventions.AddRange(conventions);
    }

    public void PrependConventions(params IEnumerable<object> conventions)
    {
        _conventions.InsertRange(0, conventions);
    }

    public void ExceptConventions(params IEnumerable<Type> types)
    {
        _exceptConventions.AddRange(types);
    }

    public void ExceptConventions(params IEnumerable<Assembly> assemblies)
    {
        _exceptAssemblyConventions.AddRange(assemblies);
    }

    public List<object?> GetConventions() => _conventions;

    [LoggerMessage(1337, LogLevel.Debug, "Scanning for conventions in assemblies: {Assemblies}")]
    static partial void ScanningForConventionsInAssemblies(ILogger logger, IEnumerable<string?> assemblies);

    [LoggerMessage(1337 + 1, LogLevel.Debug, "Skipping conventions in assemblies: {Assemblies}")]
    static partial void SkippingConventionsInAssemblies(ILogger logger, IEnumerable<string?> assemblies);

    [LoggerMessage(1337 + 2, LogLevel.Debug, "Skipping existing convention types: {Types}")]
    static partial void SkippingExistingConventionTypes(ILogger logger, IEnumerable<string?> types);

    [LoggerMessage(1337 + 3, LogLevel.Debug, "Skipping explicitly included convention types: {Types}")]
    static partial void SkippingExplicitConventionTypes(ILogger logger, IEnumerable<string?> types);

    [LoggerMessage(1337 + 4, LogLevel.Debug, "Found conventions in Assembly {Assembly} ({Conventions})")]
    static partial void FoundConventionsInAssembly(ILogger logger, string? assembly, IEnumerable<string?> conventions);

    [LoggerMessage(1337 + 5, LogLevel.Trace, "Scanning => Prefilter: {Assembly} / {Type}")]
    static partial void TraceScanningPrefilter(ILogger logger, string? assembly, string? type);

    [LoggerMessage(1337 + 6, LogLevel.Trace, "Scanning => Postfilter: {Assembly} / {Type}")]
    static partial void TraceScanningPostFilter(ILogger logger, string? assembly, string? type);

    internal IEnumerable<IConventionMetadata> CalculateConventions(
        ConventionContextBuilder builder,
        IConventionFactory factory,
        ILogger? logger
    )
    {
        logger ??= NullLogger.Instance;
        var conventions = factory.LoadConventions(builder);

        if (_exceptAssemblyConventions.Count > 0)
            SkippingConventionsInAssemblies(logger, _exceptAssemblyConventions.Select(x => x.GetName().Name));

        if (_exceptConventions.Count > 0) SkippingExplicitConventionTypes(logger, _exceptConventions.Select(x => x.FullName));

        return conventions
              .Where(z => _exceptConventions.All(x => x != z.Convention.GetType()))
              .Where(z => _exceptAssemblyConventions.All(x => x != z.Convention.GetType().Assembly));
    }
}
