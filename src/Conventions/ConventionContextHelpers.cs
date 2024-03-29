using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Reflection;

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
        IAssemblyCandidateFinder assemblyCandidateFinder,
        ILogger? logger
    )
    {
        for (var i = 0; i < builder._prependedConventions.Count; i++)
        {
            if (builder._prependedConventions[i] is Type type)
            {
                builder._prependedConventions[i] = ActivatorUtilities.CreateInstance(builder.Properties, type);
            }
        }

        for (var i = 0; i < builder._appendedConventions.Count; i++)
        {
            if (builder._appendedConventions[i] is Type type)
            {
                builder._appendedConventions[i] = ActivatorUtilities.CreateInstance(builder.Properties, type);
            }
        }

        for (var i = 0; i < builder._includeConventions.Count; i++)
        {
            if (builder._includeConventions[i] is Type type)
            {
                builder._includeConventions[i] = ActivatorUtilities.CreateInstance(builder.Properties, type);
            }
        }

        var includedConventions = builder
                                 ._includeAssemblyConventions.SelectMany(assembly => GetConventionsFromAssembly(builder, assembly, logger))
                                 .Concat(builder._includeConventions.OfType<IConvention>());


        if (builder._conventionProviderFactory != null)
        {
            return new ConventionProvider(
                builder.GetHostType(),
                GetStaticConventions(builder, logger),
                includedConventions,
                builder._prependedConventions,
                builder._appendedConventions
            );
        }

        return new ConventionProvider(
            builder.GetHostType(),
            (
                builder._useAttributeConventions
                    ? GetAssemblyConventions(builder, assemblyCandidateFinder, logger)
                    : Enumerable.Empty<IConvention>() )
           .Concat(includedConventions),
            builder._prependedConventions,
            builder._appendedConventions
        );
    }

    internal static IAssemblyCandidateFinder DefaultAssemblyCandidateFinderFactory(object? source, ILogger? logger)
    {
        return source switch
               {
                   AppDomain appDomain              => new AppDomainAssemblyCandidateFinder(appDomain, logger),
                   IEnumerable<Assembly> assemblies => new DefaultAssemblyCandidateFinder(assemblies, logger),
                   _                                => throw new NotSupportedException("Unknown source when trying to create IAssemblyCandidateFinder"),
               };
    }

    internal static IAssemblyProvider DefaultAssemblyProviderFactory(object? source, ILogger? logger)
    {
        return source switch
               {
                   AppDomain appDomain              => new AppDomainAssemblyProvider(appDomain, logger),
                   IEnumerable<Assembly> assemblies => new DefaultAssemblyProvider(assemblies, logger),
                   _                                => throw new NotSupportedException("Unknown source when trying to create IAssemblyCandidateFinder"),
               };
    }

    [LoggerMessage(1337, LogLevel.Debug, "Scanning for conventions in assemblies: {Assemblies}")]
    static partial void ScanningForConventionsInAssemblies(ILogger logger, IEnumerable<string> assemblies);

    [LoggerMessage(1337 + 1, LogLevel.Debug, "Skipping conventions in assemblies: {Assemblies}")]
    static partial void SkippingConventionsInAssemblies(ILogger logger, IEnumerable<string> assemblies);

    [LoggerMessage(1337 + 2, LogLevel.Debug, "Skipping existing convention types: {Types}")]
    static partial void SkippingExistingConventionTypes(ILogger logger, IEnumerable<string> types);

    [LoggerMessage(1337 + 3, LogLevel.Debug, "Skipping explicitly included convention types: {Types}")]
    static partial void SkippingExplicitConventionTypes(ILogger logger, IEnumerable<string> types);

    [LoggerMessage(1337 + 4, LogLevel.Debug, "Found conventions in Assembly {Assembly} ({Conventions})")]
    static partial void FoundConventionsInAssembly(ILogger logger, string assembly, IEnumerable<string> conventions);

    [LoggerMessage(1337 + 5, LogLevel.Trace, "Scanning => Prefilter: {Assembly} / {Type}")]
    static partial void TraceScanningPrefilter(ILogger logger, string assembly, string type);

    [LoggerMessage(1337 + 6, LogLevel.Trace, "Scanning => Postfilter: {Assembly} / {Type}")]
    static partial void TraceScanningPostFilter(ILogger logger, string assembly, string type);


    private static IEnumerable<IConventionWithDependencies> GetStaticConventions(
        ConventionContextBuilder builder,
        ILogger? logger
    )
    {
        logger ??= NullLogger.Instance;
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var conventions = builder._conventionProviderFactory!(builder.Properties);

        var prependedConventionTypes = new Lazy<HashSet<Type>>(
            () => new(
                builder._prependedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct()
            )
        );
        var appendedConventionTypes = new Lazy<HashSet<Type>>(
            () => new(
                builder._appendedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct()
            )
        );

        if (builder._exceptAssemblyConventions.Count > 0)
        {
            SkippingConventionsInAssemblies(
                logger,
                builder._exceptAssemblyConventions.Select(x => x.GetName().Name!)
            );
        }

        if (builder._exceptConventions.Count > 0)
        {
            SkippingExplicitConventionTypes(
                logger,
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                builder._exceptConventions.Select(x => x.FullName!)
            );
        }

        SkippingExistingConventionTypes(
            logger,
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            prependedConventionTypes.Value.Concat(appendedConventionTypes.Value).Select(x => x.FullName!)
        );

        return conventions
              .Where(z => builder._exceptConventions.All(x => x != z.Convention.GetType()))
              .Where(z => builder._exceptAssemblyConventions.All(x => x != z.Convention.GetType().Assembly));
    }

    private static IEnumerable<IConvention> GetAssemblyConventions(
        ConventionContextBuilder builder,
        IAssemblyCandidateFinder assemblyCandidateFinder,
        ILogger? logger
    )
    {
        logger ??= NullLogger.Instance;
        var assemblies = assemblyCandidateFinder
                        .GetCandidateAssemblies(
                             "Rocket.Surgery.Conventions.Abstractions",
                             "Rocket.Surgery.Conventions.Attributes",
                             "Rocket.Surgery.Conventions"
                         )
                        .ToArray();

        var prependedConventionTypes = new Lazy<HashSet<Type>>(
            () => new(
                builder._prependedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct()
            )
        );
        var appendedConventionTypes = new Lazy<HashSet<Type>>(
            () => new(
                builder._appendedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct()
            )
        );

        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        ScanningForConventionsInAssemblies(logger, assemblies.Select(x => x.GetName().Name!));
        if (builder._exceptAssemblyConventions.Count > 0)
        {
            SkippingConventionsInAssemblies(
                logger,
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                builder._exceptAssemblyConventions.Select(x => x.GetName().Name!)
            );
        }

        if (builder._exceptConventions.Count > 0)
        {
            SkippingExplicitConventionTypes(
                logger,
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                builder._exceptConventions.Select(x => x.FullName!)
            );
        }

        SkippingExistingConventionTypes(
            logger,
            prependedConventionTypes.Value.Concat(appendedConventionTypes.Value).Select(x => x.FullName!)
        );

        return assemblies
              .Except(builder._exceptAssemblyConventions)
              .SelectMany(z => GetConventionsFromAssembly(builder, z, logger))
              .Where(z => builder._exceptConventions.All(x => x != z.GetType()))
              .Where(
                   type => !prependedConventionTypes.Value.Contains(type.GetType())
                    && !appendedConventionTypes.Value.Contains(type.GetType())
               );
    }
}