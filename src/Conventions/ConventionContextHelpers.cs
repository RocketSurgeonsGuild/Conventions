using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions;

internal static partial class ConventionContextHelpers
{
    public static IEnumerable<IConvention> GetConventionsFromAssembly(ConventionContextBuilder builder, Assembly assembly, ILogger? logger)
    {
        logger ??= NullLogger.Instance;

        object selector([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)] Type type)
        {
            return ActivatorUtilities.CreateInstance(builder.Properties, type);
        }

        var types = assembly
                   .GetCustomAttributes<ExportedConventionsAttribute>()
                   .SelectMany(x => x.ExportedConventions)
                   .Union(assembly.GetCustomAttributes<ConventionAttribute>().Select(z => z.Type))
                   .Distinct()
                   .Select(selector)
                   .Cast<IConvention>()
                   .ToList();

        FoundConventionsInAssembly(
            logger,
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            assembly.GetName().Name!,
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            types.Select(z => z.GetType().FullName!)
        );

        foreach (var item in types
                            .Select(
                                 x =>
                                 {
                                     TraceScanningPrefilter(
                                         logger,
                                         // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                         assembly.GetName().Name!,
                                         // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                         x.GetType().FullName!
                                     );


                                     return x;
                                 }
                             )
                            .Select(
                                 x =>
                                 {
                                     TraceScanningPostFilter(
                                         logger,
                                         // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                         assembly.GetName().Name!,
                                         // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                         x.GetType().FullName!
                                     );

                                     return x;
                                 }
                             ))
        {
            yield return item;
        }
    }

    /// <summary>
    ///     Method used to create a convention provider
    /// </summary>
    /// <returns></returns>
    internal static IConventionProvider CreateProvider(
        ConventionContextBuilder builder,
        IAssemblyProvider assemblyProvider,
        ILogger? logger
    )
    {
        for (var i = 0; i < builder._conventions.Count; i++)
        {
            if (builder._conventions[i] is Type type) builder._conventions[i] = ActivatorUtilities.CreateInstance(builder.Properties, type);
        }

        var includedConventions = builder
                                 ._includeAssemblyConventions.SelectMany(assembly => GetConventionsFromAssembly(builder, assembly, logger))
                                 .ToArray();
        if (includedConventions.Any())
        {
            builder._conventions.InsertRange(builder._conventions.FindIndex(z => z is null) + 1, includedConventions);
        }

        if (builder._conventionProviderFactory != null)
        {
            builder._conventions.InsertRange(builder._conventions.FindIndex(z => z is null), GetStaticConventions(builder, logger));
        }
        else if (builder._useAttributeConventions)
        {
            builder._conventions.InsertRange(builder._conventions.FindIndex(z => z is null), GetAssemblyConventions(builder, assemblyProvider, logger));
        }

        return new ConventionProvider(builder.GetHostType(), builder.Categories.ToImmutableHashSet(ConventionCategory.ValueComparer), builder._conventions);
    }

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


    private static IEnumerable<IConventionMetadata> GetStaticConventions(
        ConventionContextBuilder builder,
        ILogger? logger
    )
    {
        logger ??= NullLogger.Instance;
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var conventions = builder._conventionProviderFactory!.LoadConventions(builder);

        if (builder._exceptAssemblyConventions.Count > 0)
            SkippingConventionsInAssemblies(logger, builder._exceptAssemblyConventions.Select(x => x.GetName().Name));

        if (builder._exceptConventions.Count > 0) SkippingExplicitConventionTypes(logger, builder._exceptConventions.Select(x => x.FullName));

        return conventions
              .Where(z => builder._exceptConventions.All(x => x != z.Convention.GetType()))
              .Where(z => builder._exceptAssemblyConventions.All(x => x != z.Convention.GetType().Assembly));
    }

    private static IEnumerable<IConvention> GetAssemblyConventions(
        ConventionContextBuilder builder,
        IAssemblyProvider assemblyProvider,
        ILogger? logger
    )
    {
        logger ??= NullLogger.Instance;
        var assemblies = assemblyProvider
                        .GetAssemblies(z => z.FromAssemblyDependenciesOf<IConvention>())
                        .ToImmutableArray();

        ScanningForConventionsInAssemblies(logger, assemblies.Select(x => x.GetName().Name));
        if (builder._exceptAssemblyConventions.Count > 0)
            SkippingConventionsInAssemblies(logger, builder._exceptAssemblyConventions.Select(x => x.GetName().Name));

        if (builder._exceptConventions.Count > 0) SkippingExplicitConventionTypes(logger, builder._exceptConventions.Select(x => x.FullName));

        return assemblies
              .Except(builder._exceptAssemblyConventions)
              .SelectMany(z => GetConventionsFromAssembly(builder, z, logger))
              .Where(z => builder._exceptConventions.All(x => x != z.GetType()))
//              .Where(type => !builder._conventions.Contains(type.GetType()))
            ;
    }
}
