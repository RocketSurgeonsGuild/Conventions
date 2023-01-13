using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions;

internal static class ConventionContextHelpers
{
    private static readonly Action<ILogger, IEnumerable<string>, Exception?> ScanningForConventionsInAssemblies =
        LoggerMessage.Define<IEnumerable<string>>(LogLevel.Debug, 1337, "Scanning for conventions in assemblies: {Assemblies}");

    private static readonly Action<ILogger, IEnumerable<string>, Exception?> SkippingConventionsInAssemblies =
        LoggerMessage.Define<IEnumerable<string>>(LogLevel.Debug, 1337, "Skipping conventions in assemblies: {Assemblies}");

    private static readonly Action<ILogger, IEnumerable<string>, Exception?> SkippingExistingConventionTypes =
        LoggerMessage.Define<IEnumerable<string>>(LogLevel.Debug, 1337, "Skipping existing convention types: {Types}");

    private static readonly Action<ILogger, string, IEnumerable<string>, Exception?> FoundConventionsInAssembly =
        LoggerMessage.Define<string, IEnumerable<string>>(LogLevel.Debug, 1337, "Found conventions in Assembly {Assembly} ({@Conventions})");

    private static readonly Action<ILogger, string, string, Exception?> TraceScanningPrefilter =
        LoggerMessage.Define<string, string>(LogLevel.Trace, 1337, "Scanning => Prefilter: {Assembly} / {Type}");

    private static readonly Action<ILogger, string, string, Exception?> TraceScanningPostFlter =
        LoggerMessage.Define<string, string>(LogLevel.Trace, 1337, "Scanning => Postfilter: {Assembly} / {Type}");

    private static IEnumerable<IConvention> GetAssemblyConventions(
        ConventionContextBuilder builder,
        IAssemblyCandidateFinder assemblyCandidateFinder,
        ILogger? logger
    )
    {
        logger ??= NullLogger.Instance;
        var assemblies = assemblyCandidateFinder.GetCandidateAssemblies(
            "Rocket.Surgery.Conventions.Abstractions",
            "Rocket.Surgery.Conventions.Attributes",
            "Rocket.Surgery.Conventions"
        ).ToArray();

        var prependedConventionTypes = new Lazy<HashSet<Type>>(
            () => new HashSet<Type>(
                builder._prependedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct()
            )
        );
        var appendedConventionTypes = new Lazy<HashSet<Type>>(
            () => new HashSet<Type>(
                builder._appendedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct()
            )
        );

        ScanningForConventionsInAssemblies(logger, assemblies.Select(x => x.GetName().Name!), null);
        if (builder._exceptAssemblyConventions.Count > 0)
        {
            SkippingConventionsInAssemblies(
                logger,
                builder._exceptAssemblyConventions.Select(x => x.GetName().Name!),
                null
            );
        }

        SkippingExistingConventionTypes(
            logger,
            prependedConventionTypes.Value.Concat(appendedConventionTypes.Value).Select(x => x.FullName!),
            null
        );

        return assemblies.Except(builder._exceptAssemblyConventions)
                         .SelectMany(z => GetConventionsFromAssembly(builder, z, logger))
                         .Where(
                              type => !prependedConventionTypes.Value.Contains(type.GetType()) &&
                                      !appendedConventionTypes.Value.Contains(type.GetType())
                          );
    }

    public static IEnumerable<IConvention> GetConventionsFromAssembly(ConventionContextBuilder builder, Assembly assembly, ILogger? logger)
    {
        logger ??= NullLogger.Instance;
        var types = assembly.GetCustomAttributes<ExportedConventionsAttribute>()
                            .SelectMany(x => x.ExportedConventions)
                            .Union(assembly.GetCustomAttributes<ConventionAttribute>().Select(z => z.Type))
                            .Distinct()
                            .Select(type => ActivatorUtilities.CreateInstance(builder.Properties, type))
                            .Cast<IConvention>()
                            .ToList();

        FoundConventionsInAssembly(
            logger,
            assembly.GetName().Name!,
            types.Select(z => z.GetType().FullName!),
            null
        );

        foreach (var item in types
                            .Select(
                                 x =>
                                 {
                                     TraceScanningPrefilter(
                                         logger,
                                         assembly.GetName().Name!,
                                         x.GetType().FullName!,
                                         null
                                     );


                                     return x;
                                 }
                             )
                            .Select(
                                 x =>
                                 {
                                     TraceScanningPostFlter(
                                         logger,
                                         assembly.GetName().Name!,
                                         x.GetType().FullName!,
                                         null
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

        var includedConventions = builder._includeAssemblyConventions.SelectMany(assembly => GetConventionsFromAssembly(builder, assembly, logger))
                                         .Concat(builder._includeConventions.OfType<IConvention>());


        if (builder._conventionProviderFactory != null)
        {
            return new ConventionProvider(
                builder.GetHostType(),
                builder._conventionProviderFactory(builder.Properties),
                includedConventions,
                builder._prependedConventions,
                builder._appendedConventions
            );
        }

        var contributionTypes = builder._useAttributeConventions
            ? GetAssemblyConventions(builder, assemblyCandidateFinder, logger)
               .Where(z => builder._exceptConventions.All(x => x != z.GetType()))
            : Enumerable.Empty<IConvention>();
        return new ConventionProvider(
            builder.GetHostType(), contributionTypes.Concat(includedConventions), builder._prependedConventions, builder._appendedConventions
        );
    }

    internal static IAssemblyCandidateFinder defaultAssemblyCandidateFinderFactory(object? source, ILogger? logger)
    {
        return source switch
        {
            AppDomain appDomain              => new AppDomainAssemblyCandidateFinder(appDomain, logger),
            IEnumerable<Assembly> assemblies => new DefaultAssemblyCandidateFinder(assemblies, logger),
            _                                => throw new NotSupportedException("Unknown source when trying to create IAssemblyCandidateFinder")
        };
    }

    internal static IAssemblyProvider defaultAssemblyProviderFactory(object? source, ILogger? logger)
    {
        return source switch
        {
            AppDomain appDomain              => new AppDomainAssemblyProvider(appDomain, logger),
            IEnumerable<Assembly> assemblies => new DefaultAssemblyProvider(assemblies, logger),
            _                                => throw new NotSupportedException("Unknown source when trying to create IAssemblyCandidateFinder")
        };
    }
}
